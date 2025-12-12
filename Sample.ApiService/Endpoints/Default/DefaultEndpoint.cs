using FastEndpoints;

namespace Sample.ApiService.Endpoints.Default;

public class DefaultEndpoint : Ep.NoReq.Res<string>
{
    public override void Configure()
    {
        Get("/");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct) 
        => await Send.OkAsync("API service is running. Navigate to /weatherforecast to see sample data.", ct);
}