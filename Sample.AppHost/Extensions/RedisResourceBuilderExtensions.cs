using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Sample.AppHost.Extensions;

public static class RedisResourceBuilderExtensions
{
    extension(IResourceBuilder<RedisResource> builder)
    {
        public IResourceBuilder<RedisResource> WithClearCommand()
        {
            var commandOptions = new CommandOptions
            {
                UpdateState = OnUpdateResourceState,
                IconName = "TrayItemRemove",
                IconVariant = IconVariant.Filled
            };

            return builder.WithCommand("clear-cache",
                "Clear cache",
                context => OnRunClearCacheCommandAsync(builder, context),
                commandOptions
            );
        }

    }
    
    /// <summary>
    /// Determines the state of the command that has been executed.
    /// </summary>
    private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Updating resource state: {ResourceSnapshot}", context.ResourceSnapshot);

        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }

    /// <summary>
    /// Command logic delegate to execute cache clear on a redis instance.
    /// </summary>
    private static async Task<ExecuteCommandResult> OnRunClearCacheCommandAsync(IResourceBuilder<RedisResource> builder, ExecuteCommandContext context)
    {
        var cs = await builder.Resource.GetConnectionStringAsync()
                 ?? throw new InvalidOperationException($"Unable to get the '{context.ResourceName}' connection string.");

        await using var c = await ConnectionMultiplexer.ConnectAsync(cs);

        var database = c.GetDatabase();
        await database.ExecuteAsync("FLUSHALL");

        return CommandResults.Success();
    }
}