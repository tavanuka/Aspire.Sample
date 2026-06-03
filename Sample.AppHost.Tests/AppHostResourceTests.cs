namespace Sample.AppHost.Tests;

public class AppHostResourceTests
{
    [Fact]
    public async Task AppHostConfiguresAllExpectedResources()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Assert - verify all expected resources exist
        string[] expectedResourceNames = ["postgres", "core-db", "cache", "migration-worker", "apiservice", "webfrontend"];

        foreach (var name in expectedResourceNames)
            Assert.Contains(model.Resources, r => r.Name == name);
    }

    [Theory]
    [InlineData("postgres", typeof(PostgresServerResource))]
    [InlineData("core-db", typeof(PostgresDatabaseResource))]
    [InlineData("cache", typeof(RedisResource))]
    [InlineData("migration-worker", typeof(ProjectResource))]
    [InlineData("apiservice", typeof(ProjectResource))]
    [InlineData("webfrontend", typeof(ProjectResource))]
    public async Task ResourceHasExpectedType(string resourceName, Type expectedType)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Act
        var resource = model.Resources.SingleOrDefault(r => r.Name == resourceName);

        // Assert
        Assert.NotNull(resource);
        Assert.IsType(expectedType, resource);
    }

    [Theory]
    [InlineData("migration-worker", "core-db")]
    [InlineData("apiservice", "core-db")]
    [InlineData("apiservice", "migration-worker")]
    [InlineData("webfrontend", "cache")]
    [InlineData("webfrontend", "apiservice")]
    public async Task ResourceHasReferenceToExpectedResource(string resourceName, string referencedResourceName)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Act
        var resource = model.Resources.Single(r => r.Name == resourceName);
        var references = resource.Annotations
            .OfType<ResourceRelationshipAnnotation>()
            .Select(a => a.Resource.Name);

        // Assert
        Assert.Contains(referencedResourceName, references);
    }

    [Theory]
    [InlineData("apiservice")]
    [InlineData("webfrontend")]
    public async Task ResourceHasHealthCheckConfigured(string resourceName)
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Act
        var resource = model.Resources.Single(r => r.Name == resourceName);
        var healthCheckAnnotation = resource.Annotations.OfType<HealthCheckAnnotation>().FirstOrDefault();

        // Assert
        Assert.NotNull(healthCheckAnnotation);
    }

    [Fact]
    public async Task WebFrontendHasExternalHttpEndpoints()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Act
        var webfrontend = model.Resources.Single(r => r.Name == "webfrontend");
        var endpoints = webfrontend.Annotations.OfType<EndpointAnnotation>();

        // Assert
        Assert.Contains(endpoints, e => e.IsExternal);
    }

    [Fact]
    public async Task PostgresHasPersistentLifetime()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Act
        var postgres = model.Resources.Single(r => r.Name == "postgres");
        var lifetimeAnnotation = postgres.Annotations.OfType<ContainerLifetimeAnnotation>().FirstOrDefault();

        // Assert
        Assert.NotNull(lifetimeAnnotation);
        Assert.Equal(ContainerLifetime.Persistent, lifetimeAnnotation.Lifetime);
    }

    [Fact]
    public async Task CacheHasClearCommand()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken);

        var model = app.Services.GetRequiredService<DistributedApplicationModel>();

        // Act
        var cache = model.Resources.Single(r => r.Name == "cache");
        var commands = cache.Annotations.OfType<ResourceCommandAnnotation>();

        // Assert
        Assert.Contains(commands, c => c.Name == "clear-cache");
    }
}