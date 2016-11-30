using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using OurLittleTastyServerBot.Classes;
using OurLittleTastyServerBot.Classes.Configuration;
using OurLittleTastyServerBot.Classes.Events;
using OurLittleTastyServerBot.Classes.Telegram.Models;

namespace OurLittleTastyServerBot
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    static class Program
    {
        private static void Main(string[] args)
        {
            // TODO Во всех неизменяемых коллекциях (UpdateRecordRepository и EventBroker) добавить volatiole и ImmutableInterlocked, Interlocked
            // TODO Реализовать статусы обработки и лог обработки сообщения как часть сообщения
            // TODO Использование EventBroker приводит к множественным вложенным вызовам EventBroker.Publish, т.е. при сложных цепочках обработки сообщений есть шанс переполнить буффер. Можно попробовать не плодить вложенность при отпавке сообщения, а разрулить это с помощью очереди обработки сообщений в EventBroker.Publish
            // TODO Попытаться реализовать бота как фраемворк для их создания (детерменированный список команд, из парсеров из текста, обработчиков команд)
            // TODO C# 6.0 избавиться от String.Format
            // TODO C# 6.0 избавиться от (foo != null ? foo : null)
            // TODO C# 6.0 избавиться от { get; private set; }
            
            var logger = Classes.Logging.Logger.Singleton.GetInstance();

            #region Reading bot settings

            var settingsProvider = new JsonBotSettingsProvider();
            var settingsResult = settingsProvider.GetSettings();
            if (settingsResult.IsFailured)
            {
                logger.WriteError("Can't start bot! Getting bot settings failed: " + Environment.NewLine + settingsResult.ErrorText, settingsResult.Error);
                Console.ReadLine();
                return;
            }
            var botSettings = settingsResult.Value;
            logger.WriteInfo("Bot settings read completely");

            #endregion

            #region Getting bot info

            var botInfo = CheckBotSettings(botSettings);
            if (botInfo.IsFailured)
            {
                logger.WriteError("Can't start bot! Getting bot info failed: " + Environment.NewLine + botInfo.ErrorText, botInfo.Error);
                Console.ReadLine();
                return;
            }
            logger.WriteInfo("Bot info taken");

            #endregion

            #region Deleting webhook

            // гарантируем что привязка WebHook'а отключена (иначе запрос getUpdates не будет работать)
            var deleteWebHook = DeleteWebHook(botSettings);
            if (deleteWebHook.IsFailured)
            {
                logger.WriteError("Can't start bot! Deleting webhook failed: " + Environment.NewLine + botInfo.ErrorText, botInfo.Error);
                Console.ReadLine();
                return;
            }
            logger.WriteInfo("Webhook disabled");

            #endregion

            var provider = new EventBrokerProvider();
            var eventBroker = provider.GetInstance();

            var bot = new Bot(botSettings, eventBroker);

            logger.WriteInfo("Bot \"" + botInfo.Value.username + "\" started");

            bot.Start();

            Console.ReadLine();
        }

        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private static Result<User> CheckBotSettings(BotSettings settings)
        {
            var checkUrl = string.Format(settings.TelegramApiUrlPattern, settings.BotToken, "getMe");
            var checkResult = RequestHelper.Get(checkUrl);
            if (checkResult.IsFailured) return checkResult.FailCastTo<User>();

            try
            {
                var result = JsonConvert.DeserializeObject<JsonResult<User>>(checkResult.Value);
                if (!result.Ok) return Result.Fail<User>("Service has rejected a information bot request!");
                
                return Result.Ok(result.Result);
            }
            catch (Exception e)
            {
                return Result.Fail<User>("Parsing a information bot error! Check correctness of bot settings.", e);
            }
        }

        private static Result DeleteWebHook(BotSettings settings)
        {
            // проверяем установлен ли webhook
            var checkUrl = string.Format(settings.TelegramApiUrlPattern, settings.BotToken, "getWebhookInfo");
            var checkResult = RequestHelper.Get(checkUrl);
            if (checkResult.IsFailured) return checkResult;

            var result = JsonConvert.DeserializeObject<JsonResult<JsonWebhookInfo>>(checkResult.Value);
            if (!result.Ok) return Result.Fail("Service has rejected a webhookinfo request!");
            if (String.IsNullOrEmpty(result.Result.Url)) return Result.Ok();  // webhook не установлен

            // webhook обнаружен - удаляем
            var deleteHookUrl = string.Format(settings.TelegramApiUrlPattern, settings.BotToken, "setWebhook");
            RequestHelper.Get(deleteHookUrl);

            return Result.Ok();     // webhook удален
        }
        
        #region Код отправки сообщений

        //private static void SendMessage(Int32 chatId, string text)
        //{
        //    var url = string.Format(TELEGRAM_URL, BOT_TOKEN, "sendMessage");
        //    var data = "chat_id=" + chatId + "&text=" + text;
        //    RequestHelper.Post(url, data);
        //}

        //private static void SendMessage2(Int32 chatId, string message)
        //{
        //    var safeMessage = message.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").Replace("\"", "&quot;");

        //    var url = string.Format(TELEGRAM_URL, BOT_TOKEN, "sendMessage");

        //    var request = (HttpWebRequest) WebRequest.Create(url);
        //    request.Method = "POST";
        //    request.UserAgent = "TelegramBot";
        //    request.Accept = "text/html, application/xml;q=0.9, application/xhtml+xml";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.KeepAlive = true;
        //    request.SendChunked = false;
        //    request.ServicePoint.Expect100Continue = false;
        //    request.ProtocolVersion = HttpVersion.Version11;

        //    var postDataString = "chat_id=" + chatId + "&text=" + safeMessage;
        //    var postDataBytes = Encoding.UTF8.GetBytes(postDataString);

        //    var json = "";
        //    try
        //    {
        //        var requestStream = request.GetRequestStream();
        //        requestStream.Write(postDataBytes, 0, postDataBytes.Length);
        //        requestStream.Close();

        //        // ответ сервера
        //        var response = (HttpWebResponse) request.GetResponse();
        //        var responseStream = response.GetResponseStream();
        //        if (responseStream != null)
        //        {
        //            var read = new Byte[response.ContentLength];
        //            responseStream.Read(read, 0, read.Length);
        //            responseStream.Close();
        //            response.Close();
        //            json = Encoding.UTF8.GetString(read);
        //        }
        //    }
        //    catch (WebException we)
        //    {
        //        Console.Write(we.Response);

        //        var dataStream = we.Response.GetResponseStream();
        //        if (dataStream != null)
        //        {
        //            var read = new Byte[we.Response.ContentLength];
        //            dataStream.Read(read, 0, read.Length);
        //            dataStream.Close();
        //            we.Response.Close();
        //            json = Encoding.UTF8.GetString(read);
        //        }
        //    }
        //    //finally
        //    //{
        //    //    if (!String.IsNullOrEmpty(json))
        //    //    {
        //    //        var result = JsonConvert.DeserializeObject<Body>(json);
        //    //        if (result.ok)
        //    //        {
        //    //            return result.result.message_id;
        //    //        }
        //    //        else
        //    //        {
        //    //            Console.Write(result.description);

        //    //            return 0;
        //    //        }
        //    //    }
        //    //}
        //}

        #endregion

    }
}
