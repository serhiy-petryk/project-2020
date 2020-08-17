using System;
using System.ComponentModel;
using System.Text;

namespace DGCore.Filters
{
    //===============  Class FilterLineItemCollection  ==============
    public class FilterLineSubitemCollection : BindingList<FilterLineSubitem>
    {
        FilterLineBase _owner;

        public FilterLineSubitemCollection(FilterLineBase owner) => _owner = owner;

        protected override void InsertItem(int index, FilterLineSubitem item)
        {
            try
            {
                item.Owner = _owner;
                base.InsertItem(index, item);
            }
            catch (Exception ex)
            {
            }
        }
    }

    //===============  Class FilterLineItem  ==============
    public class FilterLineSubitem : INotifyPropertyChanged, IDataErrorInfo
    {
        Common.Enums.FilterOperand _operand;
        object _value1;
        object _value2;

        public string GetStringPresentation()
        {
            var s = GetShortStringPresentation();
            return s == null ? null : "[" + Owner.DisplayName + "] " + s;
        }

        public string GetShortStringPresentation()
        {// без названия поля
            if (IsValid)
            {
                var sOperand = TypeDescriptor.GetConverter(typeof(Common.Enums.FilterOperand)).ConvertToString(_operand);
                var i = Common.Enums.FilterOperandTypeConverter.GetParameterQuantity(_operand);
                if (i < 1)
                    return sOperand;
                else if (i < 2)
                {
                    var sValue1 = TypeDescriptor.GetConverter(_value1.GetType()).ConvertToString(_value1);
                    return sOperand + " " + sValue1;
                }
                else
                {
                    var sValue1 = TypeDescriptor.GetConverter(_value1.GetType()).ConvertToString(_value1);
                    var sValue2 = TypeDescriptor.GetConverter(_value2.GetType()).ConvertToString(_value2);
                    return sOperand + " " + sValue1 + " i " + sValue2;
                }
            }
            return null;
        }

        [DisplayName("Операнд")]
        public Common.Enums.FilterOperand FilterOperand
        {
            get => _operand;
            set
            {
                _operand = value;
                var i = Common.Enums.FilterOperandTypeConverter.GetParameterQuantity(_operand);
                if (i < 2) _value2 = null;
                if (i < 1) _value1 = null;
                RefreshUI();
            }
        }

        public object Value1
        {
            get
            {
                return _value1;
            }
            set
            {
                if (Owner is FilterLine_Item)
                    _value1 = Utils.Tips.ConvertTo(value, Owner.PropertyType, ((FilterLine_Item)Owner)._pd.Converter);
                else
                    _value1 = Utils.Tips.ConvertTo(value, Owner.PropertyType, null);
                var i = Common.Enums.FilterOperandTypeConverter.GetParameterQuantity(_operand);
                if (_value1 == null)
                {
                    if (i > 0)
                        _operand = Common.Enums.FilterOperand.None;
                }
                else
                {
                    if (i < 1)
                        _operand = Common.Enums.FilterOperand.Equal;
                }
                RefreshUI();
            }
        }
        public object Value2
        {
            get => _value2;
            set
            {
                if (Owner is FilterLine_Item)
                    _value2 = Utils.Tips.ConvertTo(value, Owner.PropertyType, ((FilterLine_Item) Owner)._pd.Converter);
                else
                    _value2 = Utils.Tips.ConvertTo(value, Owner.PropertyType, null);
                var i = Common.Enums.FilterOperandTypeConverter.GetParameterQuantity(_operand);
                if (_value2 == null)
                {
                    if (i == 2)
                        _operand = Common.Enums.FilterOperand.Equal;
                }
                else
                {
                    if (i < 2)
                        _operand = Common.Enums.FilterOperand.Between;
                }
                RefreshUI();
            }
        }
        [Browsable(false)]
        public FilterLineBase Owner { get; set; }

        [Browsable(false)]
        public string Error
        {
            get
            {
                var sb = new StringBuilder();
                var i = Common.Enums.FilterOperandTypeConverter.GetParameterQuantity(_operand);
                if (i > 0 && _value1 == null) sb.Append("Вкажіть вираз №1");
                if (i < 1 && _value1 != null) sb.Append("Зітріть вираз №1");
                if (i > 1 && _value2 == null) sb.Append("Вкажіть вираз №2");
                if (i < 2 && _value2 != null) sb.Append("Зітріть вираз №2");
                return sb.ToString();
            }
        }

        [Browsable(false)]
        public string this[string columnName]
        {
            get
            {
                var i = Common.Enums.FilterOperandTypeConverter.GetParameterQuantity(_operand);
                if (i > 0 && _value1 == null && columnName == "Value1") return "Вкажіть вираз №1";
                if (i < 1 && _value1 != null && columnName == "Value1") return "Зітріть вираз №1";
                if (i > 1 && _value2 == null && columnName == "Value2") return "Вкажіть вираз №2";
                if (i < 2 && _value2 != null && columnName == "Value2") return "Зітріть вираз №2";
                return null;
            }
        }

        [Browsable(false)]
        public bool IsValid => _operand != Common.Enums.FilterOperand.None && String.IsNullOrEmpty(Error);

        [Browsable(false)]
        public bool IsError => !string.IsNullOrEmpty(Error);

        public override string ToString() => GetStringPresentation();

        private void RefreshUI()
        {
            OnPropertiesChanged(new [] { nameof(FilterOperand), nameof(Value1), nameof(Value2), nameof(IsError), nameof(IsValid)});
        }

        //===========  INotifyPropertyChanged  =======================
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
