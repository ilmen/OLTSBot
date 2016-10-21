using System;
using System.Net;
using System.Text;

namespace OurLittleTastyServerBot.Classes
{
    public static class RequestHelper
    {
        public static Result<string> Get(string url)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var response = webClient.DownloadString(url);
                    return Result.Ok(response);
                }
            }
            catch (Exception e)
            {
                return Result.Fail<string>("Ошибка GET запроса!", e);
            }
        }

        public static Result<string> Post(string url, string data, bool readResponse = false)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Accept = "text/html, application/xml;q=0.9, application/xhtml+xml";
            request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = true;
            request.SendChunked = false;
            request.ServicePoint.Expect100Continue = false;
            request.ProtocolVersion = HttpVersion.Version11;

            // post запрос
            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var requestStream = request.GetRequestStream();
                requestStream.Write(dataBytes, 0, dataBytes.Length);
                requestStream.Close();
            }
            catch (Exception e)
            {
                return Result.Fail<string>("Ошибка POST запроса!", e);
            }
            if (!readResponse) return Result.Ok<string>(null);

            // ответ сервера
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                if (responseStream == null) throw new Exception("Ошибка получения потока ответа от сервера!");
                 
                var read = new Byte[response.ContentLength];
                responseStream.Read(read, 0, read.Length);
                responseStream.Close();
                response.Close();
                var json = Encoding.UTF8.GetString(read);
                return Result.Ok(json);
            }
            catch (Exception e)
            {
                return Result.Fail<string>("Ошибка POST запроса: ошибка чтения ответ сервера!", e);
            }
        }
    }
}