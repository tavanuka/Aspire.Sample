using Refit;
using Sample.Shared.Endpoints.WeatherForecast.Responses;

namespace Sample.Web.Clients;

public interface IWeatherApiClient : IBackendApiClient
{
    [Get("api/weatherforecast")]
    Task<WeatherForecastResponse[]> GetWeatherForecast();
}