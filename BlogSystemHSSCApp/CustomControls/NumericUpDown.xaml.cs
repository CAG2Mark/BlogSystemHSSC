using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlogSystemHSSC.CustomControls
{
    /*
     * PLEASE NOTE:
     * 
     * This control is imported from a previous project I made and adapted to fit this project.
    */


    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public enum NumberValueType
        {
            Integer = 1,
            Decimal = 2,
            Percentage = 3,
            Pixels = 4,
            Degrees = 5
        }
        #region dependencyProperties

        public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(Convert.ToDouble(1)));


        public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(Convert.ToDouble(10)));

        public static readonly DependencyProperty CurrentValueProperty =
DependencyProperty.Register("CurrentValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(Convert.ToDouble(1), (o, e) => ((NumericUpDown)o).SetVal()));

        public static readonly DependencyProperty ValueTypeProperty =
DependencyProperty.RegisterAttached("ValueType", typeof(NumberValueType), typeof(NumericUpDown), new PropertyMetadata(NumberValueType.Integer));

        public static readonly DependencyProperty UpDownButtonsVisibleProperty =
            DependencyProperty.Register("UpDownButtonsVisible", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));

        public double MaxValue
        {
            get => (Double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public double MinValue
        {
            get => (Double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double CurrentValue
        {
            get => (Double)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }

        public bool buttonsVisible
        {
            get => (bool)GetValue(UpDownButtonsVisibleProperty);
            set => SetValue(UpDownButtonsVisibleProperty, value);
        }

        public NumberValueType ValueType
        {
            get => (NumberValueType)GetValue(ValueTypeProperty);
            set => SetValue(ValueTypeProperty, value);
        }

        #endregion



        public NumericUpDown()
        {
            InitializeComponent();

        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
            SetVal();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (buttonsVisible == false)
            {
                RightGrid.Visibility = Visibility.Collapsed;
                RightColumnDefinition.Width = new GridLength(0);

                MainGrid.Resources["textBoxThickness"] = new Thickness(1, 1, 1, 1);
                MainGrid.Resources["textBoxCornerRadius"] = new CornerRadius(4, 4, 4, 4);
            }

            if (loaded == false)
            {
                SetVal();
            }

            loaded = true;
        }
        bool loaded = false;

        public event EventHandler ValueChanged;

        public void SetVal() // sets the current value to the textbox
        {
            // Fires the onValueChanged event when the value is changed

            if (loaded)
            {
                ValueChanged?.Invoke(this, new EventArgs());
            }

            if (ValueType == NumberValueType.Percentage)
            {
                double currentValPercent = Math.Round(CurrentValue * 100);
                NumberTextBox.Text = currentValPercent.ToString() + "%";
            }
            else if (ValueType == NumberValueType.Pixels)
            {
                NumberTextBox.Text = CurrentValue.ToString() + "px";
            }
            else if (ValueType == NumberValueType.Degrees)
            {
                NumberTextBox.Text = CurrentValue.ToString() + "°";
            }
            else
            {
                NumberTextBox.Text = CurrentValue.ToString();
            }
        }

        public void NumberUpPressed(object sender, RoutedEventArgs e)
        {
            NumberUp();

        }

        public void NumberUp()
        {
            if (ValueType == NumberValueType.Integer || ValueType == NumberValueType.Degrees)
            {
                if (CurrentValue < MaxValue)
                {
                    CurrentValue++;
                    SetVal();
                }
            }
            else if (ValueType == NumberValueType.Decimal || ValueType == NumberValueType.Pixels)
            {
                var newVal = Math.Round(CurrentValue + 1);
                if (newVal <= MaxValue)
                {
                    CurrentValue = newVal;
                    SetVal();
                }
                else
                {
                    CurrentValue = MaxValue;
                    SetVal();
                }
            }
            else if (ValueType == NumberValueType.Percentage)
            {
                if (CurrentValue < MaxValue)
                {
                    CurrentValue = CurrentValue + 0.01;
                    SetVal();
                }
            }
        }

        public void NumberDownPressed(object sender, RoutedEventArgs e)
        {
            NumberDown();
        }

        public void NumberDown()
        {
            if (ValueType == NumberValueType.Integer || ValueType == NumberValueType.Degrees)
            {
                if (CurrentValue > MinValue)
                {
                    CurrentValue--;
                    SetVal();
                }
            }
            else if (ValueType == NumberValueType.Decimal || ValueType == NumberValueType.Pixels)
            {
                var newVal = Math.Round(CurrentValue - 1);
                if (newVal >= MinValue)
                {
                    CurrentValue = newVal;
                    SetVal();
                }
                else
                {
                    CurrentValue = MinValue;
                    SetVal();

                }
            }
            else if (ValueType == NumberValueType.Percentage)
            {
                if (CurrentValue > MinValue)
                {
                    CurrentValue = CurrentValue - 0.01;
                    SetVal();
                }
            }
        }

        public void VerifyNumber()
        {

            if (NumberTextBox.Text == "")
            {
                CurrentValue = 0;
                SetVal();
            }
            else
            {
                try
                {
                    switch (ValueType)
                    {
                        case NumberValueType.Integer:
                            {
                                double curVal1 = Convert.ToDouble(NumberTextBox.Text); // this var is mainly used for comparison
                                double curVal = Math.Round(curVal1);
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    SetVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    SetVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    SetVal();
                                }

                                break;
                            }
                        case NumberValueType.Decimal:
                            {
                                double curVal = Convert.ToDouble(NumberTextBox.Text); // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    SetVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    SetVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    SetVal();
                                }

                                break;
                            }
                        case NumberValueType.Percentage:
                            {
                                string curValStr = NumberTextBox.Text.Replace("%", "");
                                double curVal = Convert.ToDouble(curValStr) / 100; // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    SetVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    SetVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    SetVal();
                                }

                                break;
                            }
                        case NumberValueType.Pixels:
                            {
                                string curValStrTemp = NumberTextBox.Text.Replace("p", "");
                                string curValStr = curValStrTemp.Replace("x", "");
                                double curVal = Convert.ToDouble(curValStr); // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    SetVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    SetVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    SetVal();
                                }

                                break;

                            }
                        case NumberValueType.Degrees:
                            {
                                string curValStr = NumberTextBox.Text.Replace("°", "");

                                double curVal = Convert.ToDouble(curValStr); // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    SetVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    SetVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    SetVal();
                                }

                                break;
                            }

                    }
                }
                catch (Exception)
                {
                    NumberTextBox.Text = tempValue;
                }
            }
        }
        private string tempValue;

        public void SetValue(object sender, RoutedEventArgs e)
        {
            VerifyNumber();
        }

        public void SetTempValue(object sender, RoutedEventArgs e)
        {

            tempValue = NumberTextBox.Text;
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                VerifyNumber();
            }

        }

        private readonly Regex _regex = new Regex("[0-9.,-]+"); //regex that matches disallowed text
        private readonly Regex _regexInt = new Regex("[0-9-]+"); //regex that matches disallowed text
        private readonly Regex _regexPercent = new Regex("[0-9,.-^%]*$"); //regex that matches disallowed text
        private readonly Regex _regexDegree = new Regex("^[0-9-+°+-]*$");
        private readonly Regex _regexPixel = new Regex("^[0-9-+,.p-p+x-x]*$");
        private bool IsTextAllowed(string text)
        {
            switch (ValueType)
            {
                case NumberValueType.Decimal:
                    return _regex.IsMatch(text);
                case NumberValueType.Integer:
                    return _regexInt.IsMatch(text);
                case NumberValueType.Percentage:
                    return _regexPercent.IsMatch(text);
                case NumberValueType.Degrees:
                    return _regexDegree.IsMatch(text);
                case NumberValueType.Pixels:
                    {
                        return _regexPixel.IsMatch(text);
                    }

            }
            return false;
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            e.Handled = !IsTextAllowed(e.Text);
        }

        #region styling

        private string _resourceName = "UniversalBorderColor";
        private bool _isSelected = false;

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _isSelected = true;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorDark2Brush"];
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _isSelected = false;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorBrush"];
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_isSelected) return;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorDark1Brush"];
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_isSelected) return;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorBrush"];
        }

        #endregion

        private void previewKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) NumberUp();
            if (e.Key == Key.Down) NumberDown();
        }
    }
}
