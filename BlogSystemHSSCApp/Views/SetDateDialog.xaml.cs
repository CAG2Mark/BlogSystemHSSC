using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace BlogSystemHSSC.Views
{
    /// <summary>
    /// Interaction logic for SetDateDialog.xaml
    /// </summary>
    public partial class SetDateDialog : Window, IBindableBase
    {

        private DateTime time;
        public DateTime Time
        {
            get => time;
            set => Set(ref time, value);
        }

        public DateTime NewTime { get; set; }

        static int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
        static int[] monthDaysLeap = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        public SetDateDialog()
        {
            this.Time = DateTime.Now;
            DataContext = Time;
            InitializeComponent();
        }

        public SetDateDialog(DateTime time)
        {
            this.Time = time;
            DataContext = Time;
            InitializeComponent();
        }

        private void ClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void setNewTime()
        {
            NewTime = new DateTime((int)YearSelector.CurrentValue, (int)MonthSelector.CurrentValue, (int)DaySelector.CurrentValue,
                (int)HourSelector.CurrentValue, (int)MinuteSelector.CurrentValue, 0);
        }

        #region IBindable members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Set<T>(ref T reference, T value, [CallerMemberName] string propertyName = null)
        {
            // set the reference value.
            reference = value;
            // call PropertyChanged on the property.
            OnPropertyChanged(propertyName);
        }

        #endregion

        private void MonthYearChanged(object sender, EventArgs e)
        {
            DaySelector.MaxValue = ((YearSelector.CurrentValue % 4 == 0) ? monthDays : monthDaysLeap)[(int)MonthSelector.CurrentValue - 1];
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            setNewTime();
        }
    }
}
