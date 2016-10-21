using System;
using Newtonsoft.Json;

namespace OurLittleTastyServerBot.Classes
{
    public class WeatherProvider
    {
        private string GetWeatherUrl(string city)
        {
            // YQL запрос к https://developer.yahoo.com/weather/
            // Текста запроса: select * from weather.forecast where woeid in (select woeid from geo.places(1) where text="novosibirsk")

            return String.Format("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22{0}%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys", city);
        }

        public Result<Weather> GetCurrentWeather(string city)
        {
            var url = GetWeatherUrl("novosibirsk");
            var result = RequestHelper.Get(url);
            if (result.IsFailured) return Result.Fail<Weather>(result);

            ServerWeather serverWeather;
            try
            {
                var json = result.Value;
                serverWeather = JsonConvert.DeserializeObject<ServerWeather>(json);
            }
            catch (Exception e)
            {
                return Result.Fail<Weather>("Ошибка парсинга погоды, полученой от сервера!", e);
            }

            var weather = Weather.Parse(serverWeather);
            if (weather.IsFailured) return Result.Fail<Weather>(weather);
            return Result.Ok(weather.Value);
        }
    }

    public class Weather
    {
        public static Result<Weather> Parse(ServerWeather condition)
        {
            try
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                var weather = new Weather();
                weather.Temperature = condition.Query.Results.Channel.Item.Condition.Temperature + " " + condition.Query.Results.Channel.Units.Temperature;
                weather.WindSpeed = condition.Query.Results.Channel.Wind.Speed + " " + condition.Query.Results.Channel.Units.Speed;
                weather.Sunrise = condition.Query.Results.Channel.Astronomy.Sunrise;
                weather.Sunset = condition.Query.Results.Channel.Astronomy.Sunset;
                weather.Humidity = condition.Query.Results.Channel.Atmosphere.Humidity + " %";
                weather.SummaryDescription = condition.Query.Results.Channel.Item.Condition.Description;

                return Result.Ok(weather);
            }
            catch (Exception e)
            {
                return Result.Fail<Weather>("Неожиданная ошибка парсинга погоды!", e);
            }
        }

        private Weather()
        {
            // прячем
        }

        public string SummaryDescription { get; set; }

        public string Humidity { get; set; }

        public string Sunset { get; set; }

        public string Sunrise { get; set; }

        public string Temperature { get; set; }

        public string WindSpeed { get; set; }
    }

    #region Классы для десериализации ответа сервера

    public class ServerWeather
    {
        [JsonProperty("query")]
        public Query Query { get; set; }
    }

    public class Query
    {
        [JsonProperty("results")]
        public Results Results { get; set; }
    }

    public class Results
    {
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }

    public class Channel
    {
        [JsonProperty("units")]
        public Units Units { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("atmosphere")]
        public Atmosphere Atmosphere { get; set; }

        [JsonProperty("astronomy")]
        public Astronomy Astronomy { get; set; }

        [JsonProperty("item")]
        public ChannelItem Item { get; set; }
    }

    public class ChannelItem
    {
        [JsonProperty("condition")]
        public Condition Condition { get; set; }
    }

    public class Condition
    {
        [JsonProperty("temp")]
        public Int32 Temperature { get; set; }

        [JsonProperty("text")]
        public string Description { get; set; }
    }

    public class Atmosphere
    {
        [JsonProperty("humidity")]
        public Int32 Humidity { get; set; }

        [JsonProperty("pressure")]
        public float Pressure { get; set; }
    }

    public class Astronomy
    {
        [JsonProperty("sunrise")]
        public string Sunrise { get; set; }

        [JsonProperty("sunset")]
        public string Sunset { get; set; }
    }

    public class Wind
    {
        [JsonProperty("speed")]
        public Int32 Speed { get; set; }
    }

    public class Units
    {
        [JsonProperty("distance")]
        public string Distance { get; set; }

        [JsonProperty("pressure")]
        public string Pressure { get; set; }

        [JsonProperty("speed")]
        public string Speed { get; set; }

        [JsonProperty("temperature")]
        public string Temperature { get; set; }
    }

    #endregion
}
