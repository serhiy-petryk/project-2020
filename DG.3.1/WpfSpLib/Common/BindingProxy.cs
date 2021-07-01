﻿// see https://stackoverflow.com/questions/52145711/how-can-you-set-a-dynamicresource-in-code-behind-if-the-target-is-not-a-framewor
// https://stackoverflow.com/questions/33816511/how-can-you-bind-to-a-dynamicresource-so-you-can-use-a-converter-or-stringformat

using System.Windows;

namespace WpfSpLib.Common
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(object), typeof(BindingProxy), new FrameworkPropertyMetadata(default));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }

    //===================================================================
    public class LocalizationProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new LocalizationProxy();

        public static readonly DependencyProperty InputValueProperty = DependencyProperty.Register(nameof(InputValue),
            typeof(object), typeof(LocalizationProxy), new FrameworkPropertyMetadata(null, OnInputValueChanged));
        public object InputValue
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        private static void OnInputValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proxy = (LocalizationProxy)d;
            if (proxy.Argument0 == null)
                proxy.Value = e.NewValue;
            else
                proxy.Value = string.Format((string)e.NewValue, proxy.Argument0);
        }

        //==========
        public static readonly DependencyProperty Argument0Property = DependencyProperty.Register("Argument0",
            typeof(string), typeof(LocalizationProxy), new FrameworkPropertyMetadata(null, OnArgument0Changed));
        public string Argument0
        {
            get => (string)GetValue(Argument0Property);
            set => SetValue(Argument0Property, value);
        }
        private static void OnArgument0Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var proxy = (LocalizationProxy)d;
            if (e.NewValue == null)
                proxy.Value = proxy.InputValue;
            else
                proxy.Value = string.Format((string)proxy.InputValue, e.NewValue);
        }
        //==============
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(object), typeof(LocalizationProxy), new FrameworkPropertyMetadata(default));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }

    //==========================================================================
    public class DynamicBinding : Freezable
    {
        public DynamicBinding()
        {
            var dynamicResourceExtension = new DynamicResourceExtension(string.Empty);
            Dummy = dynamicResourceExtension.ProvideValue(null);
        }

        protected override Freezable CreateInstanceCore() => new DynamicBinding();

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(object), typeof(DynamicBinding), new FrameworkPropertyMetadata(default));
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static readonly DependencyProperty DummyProperty = DependencyProperty.Register(
            nameof(Dummy), typeof(object), typeof(DynamicBinding), new FrameworkPropertyMetadata(default));
        private object Dummy
        {
            get => GetValue(DummyProperty);
            set => SetValue(DummyProperty, value);
        }
    }
}
