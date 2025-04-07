using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JJaniszewski.WeatherApp.Application.Models;

public class WeatherApiResponse
{
    public Coord Coord { get; set; } = new ();
    public List<Weather> Weather { get; set; } = new ();
    public string Base { get; set; } = string.Empty;
    public Main Main { get; set; } = new ();
    public int Visibility { get; set; }
    public Wind Wind { get; set; } = new ();
    public Rain? Rain { get; set; }
    public Snow? Snow { get; set; }
    public Clouds Clouds { get; set; } = new ();

    [JsonPropertyName("dt")]
    public long Timestamp { get; set; }

    public DateTimeOffset DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp);
    public Sys Sys { get; set; } = new ();
    public int Timezone { get; set; }
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Cod { get; set; }
}

public class Coord
{
    public double Lon { get; set; }
    public double Lat { get; set; }
}

public class Weather
{
    public int Id { get; set; }
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class Main
{
    public double Temp { get; set; }
    public double Feels_Like { get; set; }
    public double Temp_Min { get; set; }
    public double Temp_Max { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public int? Sea_Level { get; set; }
    public int? Grnd_Level { get; set; }
}

public class Wind
{
    public double Speed { get; set; }
    public int Deg { get; set; }
    public double? Gust { get; set; }
}

public class Rain
{
    [JsonPropertyName("1h")]
    public double? OneHour { get; set; }
}

public class Snow
{
    [JsonPropertyName("1h")]
    public double? OneHour { get; set; }
}

public class Clouds
{
    public int All { get; set; }
}

public class Sys
{
    public int Type { get; set; }
    public int Id { get; set; }
    public double? Message { get; set; }
    public string Country { get; set; } = string.Empty;
    public long Sunrise { get; set; }
    public long Sunset { get; set; }
}
