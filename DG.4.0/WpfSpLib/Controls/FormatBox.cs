using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfSpLib.Controls
{
    public class FormatBox: Control//, INotifyPropertyChanged
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_Popup") is Popup popup)
                popup.Opened += Popup_Opened;
            if (GetTemplateChild("cbNotDefined") is CheckBox cbNotDefined)
                cbNotDefined.Background = Brushes.Red;
        }

        private void Popup_Opened(object sender, System.EventArgs e)
        {

        }


        #region ===========  Events =============
        #endregion
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
            typeof(string), typeof(FormatBox), new FrameworkPropertyMetadata(null));

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
