using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для CalendarDialog.xaml
    /// </summary>
    public partial class CalendarDialog : Window
    {
        public string selectedDate;

        public CalendarDialog()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string input;
            string[] splited_input;

            if (WorkingCalendar.SelectedDate.ToString() == "") return;            

            splited_input = WorkingCalendar.SelectedDate.ToString().Split();
            input = splited_input[0];
            splited_input = input.Split('.');

            selectedDate = splited_input[0] + "." + splited_input[1] + "." + splited_input[2];

            Close();
        }
    }
}
