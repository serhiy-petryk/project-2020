﻿using System.Windows;

namespace DGView.WiP
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
