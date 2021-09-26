using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BankSystem
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public ObservableCollection<Deposit> deposits;
        public Deposit deposit;

        public EditWindow()
        {
            InitializeComponent();
        }

        private void btnDeleteCount_Click(object sender, RoutedEventArgs e)
        {
            if (deposits.Count != 0)
            {
                if (Deposit.FreeCountNumbers == null)
                {
                    Deposit.FreeCountNumbers = new List<int>();
                }
                Deposit.FreeCountNumbers.Add(deposit.CountNumber);
                deposits.Remove(deposit);
            }   
            Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            lbxTransfer.Items.Clear();

            lblCountNumber.Content = "№ счета: " + deposit.CountNumber.ToString();
            lblCount.Content = "Баланс: " + deposit.Count.ToString();

            foreach (var item in deposits)
            {
                if (item.CountNumber == deposit.CountNumber) continue;                

                if (item.Category == deposit.Category)
                {
                    string s;
                    s = "Имя: " + item.FirstName + "; Фамилия: " + item.LastName +
                        "; Номер счета: " + item.CountNumber;
                    lbxTransfer.Items.Add(s);
                }
            }
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            Deposit recipient = null;

            if (tbxTransfer.Text == "")
            {
                MessageBox.Show("Нужно указать сумму перевода!");
                return;
            }

            Regex regex = new Regex(@"[^\d]");
            if (regex.IsMatch(tbxTransfer.Text))
            {
                MessageBox.Show("В поле \"Сумма перевода\" нужно ввести число!");
                return;
            }

            string str = lbxTransfer.SelectedItem.ToString();
            regex = new Regex(@"(.*; Номер счета: )(.*)");
            Match match = regex.Match(str);
            if (match != null) str = match.Groups[2].ToString();

            int cn = Convert.ToInt32(str);
            recipient = Deposit.FindCountByNumber(deposits, cn);

            if (recipient != null)
            {
                int transfer = Convert.ToInt32(tbxTransfer.Text);
                deposit.Count -= transfer;
                recipient.Count += transfer;
            }

            Close();
        }
    }
}
