using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace BankSystem
{
    /// <summary>
    /// Логика взаимодействия для CreateCountWindow.xaml
    /// </summary>
    public partial class CreateCountWindow : Window
    {
        public Deposit deposit;

        public CreateCountWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbxFirstName.Text == "" || tbxLastName.Text == "" || tbxContribution.Text == "" || tbxDate.Text == "")
            {
                MessageBox.Show("Нужно заполнить все поля ввода!");
                return;
            }
            Regex regex = new Regex(@"[^\d]");
            if (regex.IsMatch(tbxContribution.Text))
            {
                MessageBox.Show("В поле \"Взнос\" нужно ввести число!");
                return;
            }

            deposit.FirstName = tbxFirstName.Text;
            deposit.LastName = tbxLastName.Text;
            deposit.Count = Convert.ToDouble(tbxContribution.Text);

            ComboBoxItem selectedItem = (ComboBoxItem)cbCategory.SelectedItem;
            string cat = selectedItem.Content.ToString();

            deposit.Category = cat;

            if ((bool)cbxCapitalization.IsChecked)
            {
                deposit.Capitalization = true;
            }
            else
            {
                deposit.Capitalization = false;
            }
            Close();
        }

        /// <summary>
        /// Обработчик двойного клика на поле с датой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxDate_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string[] splited_date;

            var cal = new CalendarDialog();
            cal.ShowDialog();

            if (cal.selectedDate != "")
            {
                tbxDate.Text = cal.selectedDate;

                splited_date = cal.selectedDate.Split('.');
                deposit.dateTime = new DateTime(Convert.ToInt32(splited_date[2]), 
                    Convert.ToInt32(splited_date[1]), Convert.ToInt32(splited_date[0]));
            }
        }
    }
}
