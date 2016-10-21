using System;

namespace OurLittleTastyServerBot.Classes.Telegram.Models
{
    public class JsonWebhookInfo
    {
        public string Url { get; set; }

        public bool has_custom_certificate { get; set; }
            
        public Int32 pending_update_count { get; set; }     // Количество обновлений, ожидающих доставки
            
        public Int32? last_error_date { get; set; }         // Опционально. Unix-время самой последней ошибки доставки обновления на указанный вебхук
            
        public string last_error_message { get; set; }      // Опционально. Описание в человекочитаемом формате последней ошибки доставки обновления на указанный вебхук
    }
}