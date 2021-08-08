using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DGCore.UserSettings;
using DGView.Temp;

namespace DGView.Views
{
    /// <summary>
    /// Interaction logic for DbFilterView.xaml
    /// </summary>
    public partial class DbFilterView : UserControl, INotifyPropertyChanged, IUserSettingSupport<List<Filter>>
    {
        public Action ApplyAction { get; private set; }
        private string _lastAppliedLayoutName;

        public DbFilterView()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void Bind(DGCore.Filters.FilterList newFilterList, string settingKey, Action applyAction, ICollection dataSource)
        {
            SettingKey = settingKey;
            ApplyAction = applyAction;
            FilterGrid.Bind(newFilterList, dataSource);
            UserSettingsUtils.Init(this, null);
            RefreshUI();
        }

        #region ==========  Event handlers  ==========
        private void LoadData_OnClick(object sender, RoutedEventArgs e) => ApplyAction?.Invoke();

        private void OpenSettingForm_OnClick(object sender, RoutedEventArgs e)
        {
            if (((ToggleButton)sender).IsChecked == false)
                Debug.Print($"OpenSettingForm_OnClick");
        }

        private void OpenSettingButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button && button.Resources.Values.OfType<ContextMenu>().FirstOrDefault() is ContextMenu cm)
            {
                cm.Items.Clear();
                foreach (var settingName in UserSettingsUtils.GetKeysFromDb(this))
                {
                    cm.Items.Add(new MenuItem
                    {
                        Header = settingName,
                        IsChecked = settingName == _lastAppliedLayoutName,
                        // DataContext = item,
                        Command = new WpfSpLib.Common.RelayCommand((p) =>
                        {
                            UserSettingsUtils.SetSetting(this, settingName);
                            _lastAppliedLayoutName = settingName;
                            RefreshUI();
                            FilterGrid.RefreshUI();
                        })
                    });
                }
            }
        }

        private void ClearFilter_OnClick(object sender, RoutedEventArgs e)
        {
            FilterGrid.FilterList.ClearFilter();
        }
        #endregion

        #region ============  INotifyPropertyChanged  ============
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertiesChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void RefreshUI()
        {
            OnPropertiesChanged(nameof(ApplyAction));
        }
        #endregion

        #region ===========  IUserSettingSupport<List<Filter>  ==============

        public string SettingKind => "DGV_DBFilter";
        public string SettingKey { get; private set; }

        List<Filter> IUserSettingSupport<List<Filter>>.GetSettings() =>
            ((IUserSettingSupport<List<Filter>>)FilterGrid.FilterList).GetSettings();

        List<Filter> IUserSettingSupport<List<Filter>>.GetBlankSetting() =>
            ((IUserSettingSupport<List<Filter>>)FilterGrid.FilterList).GetBlankSetting();

        void IUserSettingSupport<List<Filter>>.SetSetting(List<Filter> settings) =>
            ((IUserSettingSupport<List<Filter>>)FilterGrid.FilterList).SetSetting(settings);
        #endregion
    }
}
