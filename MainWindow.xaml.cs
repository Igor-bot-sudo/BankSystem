using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BankSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Deposit> deposits = new ObservableCollection<Deposit>();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик двойного клика на элементе древовидного списка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Deposit deposit = null;

            if (DepositsView.SelectedItem != null)
            {
                string str = DepositsView.SelectedItem.ToString();

                Regex regex = new Regex(@"(.*; Номер счета: )(.*)(;.*)");
                Match match = regex.Match(str);
                if (match != null) str = match.Groups[2].ToString();

                if (str == "") return;                

                int cn = Convert.ToInt32(str);
                deposit = Deposit.FindCountByNumber(deposits, cn);

                if (deposit != null)
                {
                    EditWindow ew = new EditWindow();
                    ew.Dispatcher.Invoke(() =>
                    {
                        ew.deposits = deposits;
                        ew.deposit = deposit;
                    });
                    ew.ShowDialog();
                }

                RefreshTreeView();
            }
        }

        /// <summary>
        /// Обработчик события создания счета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppendButton_Click(object sender, RoutedEventArgs e)
        {
            Deposit deposit = new Deposit();

            CreateCountWindow ccw = new CreateCountWindow();
            ccw.Dispatcher.Invoke(() => 
            {
                ccw.deposit = deposit;
            });
            ccw.ShowDialog();

            if (deposit != null)
            {
                if (Deposit.FreeCountNumbers != null && Deposit.FreeCountNumbers.Count != 0)
                {
                    deposit.CountNumber = Deposit.FreeCountNumbers[0];
                    Deposit.FreeCountNumbers.RemoveAt(0);
                }
                else
                {
                    deposit.CountNumber = ++Deposit.MaxCountNumber;
                }

                TreeViewItem item = new TreeViewItem();
                item.Header = FillStringItem(deposit);

                if (deposit.Category == "Обычный клиент")
                {
                    tviOrdinaryClients.Items.Add(item);
                }
                else if (deposit.Category == "VIP-клиент")
                {
                    tviVIPClients.Items.Add(item);
                }
                else if (deposit.Category == "Юридическое лицо")
                {
                    tviJuridicalClients.Items.Add(item);
                }

                deposits.Add(deposit);
            }
            RefreshTreeView();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            TreeViewItem treeViewItem = null;
            foreach (var ovi in DepositsView.Items)
            {
                treeViewItem = ovi as TreeViewItem;
                treeViewItem.IsExpanded = true;
            }
        }

        /// <summary>
        /// Закрытие главного окна - сохранение данных всех депозитов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string json;
            using (StreamWriter sw = new StreamWriter("BankSystem.json", false, Encoding.UTF8))
            {
                json = JsonConvert.SerializeObject(Deposit.FreeCountNumbers);
                sw.WriteLine(json);
                json = JsonConvert.SerializeObject(Deposit.MaxCountNumber);
                sw.WriteLine(json);
                json = JsonConvert.SerializeObject(deposits);
                sw.WriteLine(json);
            }
        }

        /// <summary>
        /// Открытие главного окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            string json;

            if (File.Exists("BankSystem.json"))
            {
                using (StreamReader sr = new StreamReader("BankSystem.json"))
                {
                    while (!sr.EndOfStream)
                    {
                        json = sr.ReadLine();
                        if (Deposit.FreeCountNumbers == null)
                        {
                            Deposit.FreeCountNumbers = new List<int>();
                        }
                        Deposit.FreeCountNumbers = JsonConvert.DeserializeObject<List<int>>(json);
                        json = sr.ReadLine();
                        Deposit.MaxCountNumber = Convert.ToInt32(JsonConvert.DeserializeObject(json));
                        json = sr.ReadLine();
                        if (json != "")
                        {
                            deposits = JsonConvert.DeserializeObject<ObservableCollection<Deposit>>(json);
                            RefreshTreeView();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Метод формирования строки данных депозита для вставки в элемент древовидного списка
        /// </summary>
        /// <param name="deposit">Депозит</param>
        /// <returns></returns>
        private string FillStringItem(Deposit deposit)
        {
            string result = "Имя: " + deposit.FirstName + "; Фамилия: " + deposit.LastName +
                    "; Баланс: " + string.Format("{0:f2}", deposit.CurrentCount) +
                    "; Тип вклада: " + deposit.Category + "; Номер счета: " + deposit.CountNumber +
                    "; Дата открытия счета: " +  deposit.dateTime.ToString("d");

            if (deposit.Capitalization)
            {
                result += " - С капитализацией";
            }
            else
            {
                result += " - Без капитализации";
            }
            return result;
        }


        /// <summary>
        /// Метод обновления древовидного списка
        /// </summary>
        public void RefreshTreeView()
        {
            tviOrdinaryClients.Items.Clear();
            tviVIPClients.Items.Clear();
            tviJuridicalClients.Items.Clear();

            TimeSpan ts;

            foreach (var deposit in deposits)
            {
                TreeViewItem item = new TreeViewItem();

                ts = DateTime.Now - deposit.dateTime;
                int months = ts.Days / 30;

                if (deposit.Category == "Обычный клиент")
                {
                    // Ставка обычного клиента 7 % годовых
                    deposit.CurrentCount = Deposit.Recalculate(deposit.Count, months, 0.07, deposit.Capitalization);
                    item.Header = FillStringItem(deposit);
                    tviOrdinaryClients.Items.Add(item);
                }
                else if (deposit.Category == "VIP-клиент")
                {
                    // Ставка VIP-клиента 12 % годовых
                    deposit.CurrentCount = Deposit.Recalculate(deposit.Count, months, 0.12, deposit.Capitalization);
                    item.Header = FillStringItem(deposit);
                    tviVIPClients.Items.Add(item);
                }
                else if (deposit.Category == "Юридическое лицо")
                {
                    // Ставка юридического лица 18 % годовых
                    deposit.CurrentCount = Deposit.Recalculate(deposit.Count, months, 0.18, deposit.Capitalization);
                    item.Header = FillStringItem(deposit);
                    tviJuridicalClients.Items.Add(item);
                }
            }
        }
    }
}
