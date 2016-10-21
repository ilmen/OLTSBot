using OurLittleTastyServerBot.Classes.Db.Models;
using OurLittleTastyServerBot.Classes.Events;

namespace OurLittleTastyServerBot.Classes.Observers
{
    public class SimpleAddingCostWorker : CommandProcessor
    {
        public SimpleAddingCostWorker(EventBroker broker) : base(broker, EnCommand.AddSimpleCostRequest)
        {
        }

        protected override void Process(CommandEventArgs value)
        {
            var costResult = Cost.Parse(value.Record.Text);
            if (costResult.IsFailured)
            {
                _logger.WriteError(costResult.ErrorText, costResult.Error);
                return;
            }
            var cost = costResult.Value;

            _logger.WriteDebug("SimpleAddingCostWorker: Подавление покупки. " + cost);

            // TODO FIXME: Добавить сохранение в БД

            // TODO: Добавить категорию покупок как кнопки бота
            // TODO: Сменить формат строки на /addcost value name
            // TODO: Сделать имя покупки опциональным
            // TODO: Добавить время покупки
            // TODO: Запросить доступ к данным геолокации для привязки покупок к карте мира (а кто доверит ?)
            // TODO: Идея выводить кнопки для выбора источника денег (карта 1, карта 2, наличные)
            // TODO: Добавить команда "Сохранить чек", которая объединяет все покупки, с прошлого вызова этой команды, в один чек с коротким описанием (например, "ашан")
            // TODO: Сохранить время создания чека
            // TODO: Подумать о диалогом методе ввода траты
            // TODO: Принимать фотографии в виде отдельного чека, требовать ввода ИТОГО и опасания, попробовать парсить фото (?) (а кто доверит ?)
            // TODO: Добавить возможность добавление источников денег (а не категории ли это?)
            // TODO: Добавить возможность добавление категорий
            // TODO: Добавить команду "запомнить это место на карте как магазин ...", с радиусом на карте, с проверкой пересечения с предыдущими точками, о автоматическим объединением покупок по геоданным

            var evArgs = new MessageEventArgs("Покупка: " + cost, value.Record);
            _broker.Publish(evArgs);
        }
    }
}