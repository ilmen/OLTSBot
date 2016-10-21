namespace OurLittleTastyServerBot.Classes.Telegram.Models
{
    public class JsonResult<T>
        where T : class
    {
        public bool Ok { get; set; }

        public string Description { get; set; }

        public T Result { get; set; }
    }
}