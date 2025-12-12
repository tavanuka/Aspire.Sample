using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Sample.Infrastructure;
using Sample.Infrastructure.Seeding;
using System.Diagnostics;

namespace Sample.MigrationWorker;

/// <summary>
/// Jumpstarts the database seeding and migrations.
/// </summary>
public class MigrationWorker : BackgroundService
{
    public const string ActivitySourceName = "Sample.MigrationWorker";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ILogger<MigrationWorker> _logger;

    public MigrationWorker(IServiceProvider serviceProvider,
        IHostApplicationLifetime applicationLifetime,
        ILogger<MigrationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _applicationLifetime = applicationLifetime;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var activity = ActivitySource.StartActivity("Database migration", ActivityKind.Client);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreDbContext>();

            await EnsureDatabaseAsync(context, ct);
            await ExecuteMigrationAsync(context, ct);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            if (_logger.IsEnabled(LogLevel.Error))
                _logger.LogError(e, "An error occured while executing activity {Activity}", ActivitySource.Name);

            activity?.AddException(e);
            throw;
        }

        _applicationLifetime.StopApplication();
    }

    private async Task EnsureDatabaseAsync(CoreDbContext ctx, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking if database source exists");

        var dbCreator = ctx.GetService<IRelationalDatabaseCreator>();
        var strategy = ctx.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbCreator, static async (creator, ct) => {
            if (await creator.CanConnectAsync(ct))
                return;
            await creator.CreateAsync(ct);
        }, cancellationToken);
    }

    private async Task ExecuteMigrationAsync(CoreDbContext context, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing migrations");
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(context, static async (ctx, ct) => {
                await ctx.Database.MigrateAsync(ct);
            },
            cancellationToken);
    }

    public static async Task ExecuteDatabaseSeedingAsync(DbContext context, CancellationToken cancellationToken)
    {
        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(context,
            static async (ctx, ct) => {
                await PersonSeeding.SeedAsync(ctx, ct);
            },
            cancellationToken);
    }
}