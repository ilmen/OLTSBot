using System;
using System.Linq;

namespace OurLittleTastyServerBot.Classes.Db.Models
{
    public class Cost
    {
        public static Result<Cost> Parse(string message)
        {
            try
            {
                // TODO FIXME: добавить вывод подсказки о формате

                var line = message.Replace("/addcost ", String.Empty);
                var words = line.Split(' ');
                if (words.Length < 2) return Result.Fail<Cost>("Неверный формат команды!");

                var item = words.Take(words.Length - 1).Aggregate((x, y) => x + " " + y);
                var stringValue = words.Last();
                var value = Convert.ToInt32(stringValue);

                var result = new Cost(item, value);
                return Result.Ok(result);
            }
            catch (Exception e)
            {
                return Result.Fail<Cost>("Cost parsing error!", e);
            }
        }

        public Cost(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public string Name
        { get; private set; }

        public Int32 Value
        { get; private set; }

        public override string ToString()
        {
            return String.Format("{0} ({1} р)", Name, Value);
        }
    }
}