using System.Windows;
using System.Windows.Controls;

namespace WpfSpLib.Controls
{
    public class FormatBox: Control//, INotifyPropertyChanged
    {
        public string LanguageChangeHook
        {
            get
            {
                InternalSetText(Value);
                return null;
            }
        }

        private void InternalSetText(string newValue)
        {
            /*if (_textBox == null) return;
            if (newValue.HasValue)
                _textBox.Text = FormattedValue(newValue, StringFormat, Culture);
            else
                _textBox.Text = null;*/
        }

        //=============================
        // public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
          //  typeof(string), typeof(FormatBox), new FrameworkPropertyMetadata("F2", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value",
            typeof(string), typeof(FormatBox), new FrameworkPropertyMetadata("F2"));

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
