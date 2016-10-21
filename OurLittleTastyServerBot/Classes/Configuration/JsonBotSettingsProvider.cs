using System;
using System.IO;
using Newtonsoft.Json;

namespace OurLittleTastyServerBot.Classes.Configuration
{
    public class JsonBotSettingsProvider
    {
        private readonly string _jsonpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        public Result<BotSettings> GetSettings()
        {
            try
            {
                if (!File.Exists(_jsonpath))
                {
                    var defaultSetting = GetDefault();
                    var defaultJson = JsonConvert.SerializeObject(defaultSetting);
                    File.WriteAllText(_jsonpath, defaultJson);
                }

                var json = File.ReadAllText(_jsonpath);
                var settings = JsonConvert.DeserializeObject<BotSettings>(json);
                return Result.Ok(settings);
            }
            catch (Exception e)
            {
                return Result.Fail<BotSettings>("Check correctness of bot settings file: \"" + _jsonpath + "\"", e);
            }
        }

        private BotSettings GetDefault()
        {
            return new BotSettings("https://api.telegram.org/bot{0}/{1}", "your:bot-token");
        }
    }
}