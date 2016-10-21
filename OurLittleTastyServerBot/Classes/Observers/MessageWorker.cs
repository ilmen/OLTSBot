using System;
using System.Net;
using System.Text;
using OurLittleTastyServerBot.Classes.Configuration;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class MessageWorker : AbstractObserver<MessageEventArgs>
    {
        private readonly string _sendMessageUrl;

        public MessageWorker(BotSettings botSettings)
        {
            _sendMessageUrl = string.Format(botSettings.TelegramApiUrlPattern, botSettings.BotToken, "sendMessage");
        }

        public override void OnNext(MessageEventArgs value)
        {
            var chatId = value.Record.ChatOuterId;

            _logger.WriteDebug("MessageWorker: Отправляется сообщение. ChatId: " + chatId + ". Текст: " + value.Message);

            SendMessage2(chatId, value.Message);
        }

        private void SendMessage2(Int32 chatId, string message)
        {
            var safeMessage = message.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").Replace("\"", "&quot;");

            var request = (HttpWebRequest) WebRequest.Create(_sendMessageUrl);
            request.Method = "POST";
            request.UserAgent = "TelegramBot";
            request.Accept = "text/html, application/xml;q=0.9, application/xhtml+xml";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            request.SendChunked = false;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;

            var postDataString = "chat_id=" + chatId + "&text=" + safeMessage;
            var postDataBytes = Encoding.UTF8.GetBytes(postDataString);

            var json = "";
            try
            {
                var requestStream = request.GetRequestStream();
                requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                requestStream.Close();

                // ответ сервера
                var response = (HttpWebResponse) request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var read = new Byte[response.ContentLength];
                    responseStream.Read(read, 0, read.Length);
                    responseStream.Close();
                    response.Close();
                    json = Encoding.UTF8.GetString(read);
                }
            }
            catch (WebException we)
            {
                Console.Write(we.Response);

                var dataStream = we.Response.GetResponseStream();
                if (dataStream != null)
                {
                    var read = new Byte[we.Response.ContentLength];
                    dataStream.Read(read, 0, read.Length);
                    dataStream.Close();
                    we.Response.Close();
                    json = Encoding.UTF8.GetString(read);
                }
            }
            //finally
            //{
            //    if (!String.IsNullOrEmpty(json))
            //    {
            //        var result = JsonConvert.DeserializeObject<Body>(json);
            //        if (result.ok)
            //        {
            //            return result.result.message_id;
            //        }
            //        else
            //        {
            //            Console.Write(result.description);

            //            return 0;
            //        }
            //    }
            //}
        }
    }
}