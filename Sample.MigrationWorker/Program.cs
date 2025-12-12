using Sample.Infrastructure;
using Sample.MigrationWorker;
using Sample.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<MigrationWorker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tp => tp.AddSource(MigrationWorker.ActivitySourceName));

builder.AddNpgsqlDbContext<CoreDbContext>("core-db", configureDbContextOptions: options => {
    // Recommended way of seeding data.
    if(builder.Environment.IsDevelopment())
        options.UseAsyncSeeding(async (context, _, ct) => await MigrationWorker.ExecuteDatabaseSeedingAsync(context, ct));
});

var host = builder.Build();
host.Run();