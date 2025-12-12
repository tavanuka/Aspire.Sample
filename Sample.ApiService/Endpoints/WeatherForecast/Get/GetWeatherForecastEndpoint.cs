using FastEndpoints;
using FastEndpoints.Swagger;
using Sample.Shared.Endpoints.WeatherForecast.Responses;

namespace Sample.ApiService.Endpoints.WeatherForecast.Get;

public class GetWeatherForecastEndpoint : Ep.NoReq.Res<WeatherForecastResponse[]>
{
    private readonly string[] _summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
    public override void Configure()
    {
        Get("weatherforecast");
        AllowAnonymous();
        
        Description(d => d
            .WithName("GetWeatherForecast")
            .AutoTagOverride("WeatherForecast")
        );
        Summary(s => {
            s.Summary = "Gets a summary of weather forecast";
            s.Description = "Returns a 5-day weather forecast with random data for demonstration purposes.";
            s.Responses[200] = "Successful response with weather data";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecastResponse
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    _summaries[Random.Shared.Next(_summaries.Length)]
                ))
            .ToArray();

        await Send.OkAsync(forecast, ct);
    }
}