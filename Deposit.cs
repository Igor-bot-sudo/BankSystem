using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem
{
    public class Deposit
    {
        // Имя клиента
        public string FirstName { get; set; }

        // Фамилия клиента
        public string LastName { get; set; }

        // Размер первоначального взноса
        public double Count { get; set; }

        // Текущий счет
        public double CurrentCount { get; set; }

        // Номер счета
        public int CountNumber { get; set; }

        // Вклад с капитализацией или нет
        public bool Capitalization { get; set;}

        // Тип вклада (юридическое лицо, обычный клиент, VIP-клиент)
        public string Category { get; set; }

        // Дата открытия счета
        public DateTime dateTime { get; set; }

        // Список свободных номеров счетов
        public static List<int> FreeCountNumbers { get; set; }

        // Максимальный текущий номер счета
        public static int MaxCountNumber { get; set; }

        /// <summary>
        /// Метод поиска депозита по его номеру
        /// </summary>
        /// <param name="deposits">Список депозитов</param>
        /// <param name="cn">Номер депозита</param>
        /// <returns></returns>
        public static Deposit FindCountByNumber(ObservableCollection<Deposit> deposits, int cn)
        {
            Deposit d = null;
            foreach (var item in deposits)
            {
                if (item.CountNumber == cn)
                {
                    d = item;
                    break;
                }
            }
            return d;
        }

        /// <summary>
        /// Метод перерасчета баланса
        /// </summary>
        /// <param name="currentBalance">Текущий баланс</param>
        /// <param name="months">Количество месяцев с момента открытия счета</param>
        /// <param name="yearRate">Годовая процентная ставка</param>
        /// <param name="capitalization">Капитализация</param>
        /// <returns></returns>
        public static double Recalculate(double currentBalance, int months, double yearRate, bool capitalization)
        {
            double balance = 0;
            int yearCount = months / 30;

            if (capitalization)
            {
                double monthRate = yearRate / 12;
                double s = currentBalance;
                for (int i = 0; i < months; i++)
                {
                    s += s * monthRate;          
                }
                balance = s;
            }
            else
            {
                balance = currentBalance + yearCount * yearRate;
            }

            return balance;
        }
    }
}
