namespace Api;

public class WeatherForecast
{
    /// <summary>The calendar date covered by this forecast.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Temperature in Celsius, used as the stored source value.</summary>
    public int TemperatureC { get; set; }

    /// <summary>Calculated Fahrenheit value, derived from <see cref="TemperatureC"/>.</summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>A short human-readable description of the weather.</summary>
    public string? Summary { get; set; }
}
