using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StreamForRead.Classes;

namespace StreamForRead.Models
{
    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public abstract class BotSettingsFactory : IBotSettingsFactory
    {
        public Result<IBotSettings> GetToken()
        {
            var result = GetSettings();
            if (result.IsFailured) return result.FailCastTo<IBotSettings>();
            var settings = result.Value;

            var validation = settings.Validate();
            if (validation.IsFailured) return validation.FailCastTo<IBotSettings>();

            return Result.Ok<IBotSettings>(settings);
        }

        protected abstract Result<BotSettings> GetSettings();
    }

    public class BotSettingsFactoryFromJson : BotSettingsFactory
    {
        protected override Result<BotSettings> GetSettings()
        {
            const string settingsPath = "settings.json";

            if (!System.IO.File.Exists(settingsPath)) return Result.Fail<BotSettings>("Файл \"" + settingsPath + "\" не найден. Получение токена невозможно!");

            var json = System.IO.File.ReadAllText(settingsPath);

            try
            {
                var settings = Newtonsoft.Json.JsonConvert.DeserializeObject<BotSettings>(json);
                return Result.Ok(settings);
            }
            catch (Exception e)
            {
                return Result.Fail<BotSettings>("Ошибка десериализации файла \"" + settingsPath + "\"!", e);
            }
        }
    }

    public class BotSettingsFactoryFromAppConfig : BotSettingsFactory
    {
        protected override Result<BotSettings> GetSettings()
        {
            var token = Properties.Settings.Default.BotToken;
            if (String.IsNullOrEmpty(token)) Result.Fail<BotSettings>("Ошибка чтения настроек: токен не получен. Проверьте файл конфигурации!");

            var settings = new BotSettings {Token = token};
            return Result.Ok(settings);
        }
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    public class BotSettings : IBotSettings
    {
        public string Token { get; set; }

        public Result Validate()
        {
            if (String.IsNullOrEmpty(Token)) return Result.Fail("Token must be non-empty string!");
            if (Token.Length != 45) return Result.Fail("Wrong token length!");

            var tokenParts = Token.Split(':');
            if (tokenParts.Length != 2) return Result.Fail("Token must contain the symbol of ':'!");
            if (tokenParts[0].Length != 9) return Result.Fail("First part of token wrong length!");
            if (String.IsNullOrEmpty(tokenParts[0])) return Result.Fail("First part of token must be non-empty string!");
            if (tokenParts[0].Any(x => !Char.IsNumber(x))) return Result.Fail("First part of token must contain only numerics!");
            if (String.IsNullOrEmpty(tokenParts[1])) return Result.Fail("Second part of token must be non-empty string!");

            return Result.Ok();
        }
    }

    public interface IBotSettingsFactory
    {
        Result<IBotSettings> GetToken();
    }

    public interface IBotSettings
    {
        string Token { get; }
    }
}