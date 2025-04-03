using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Threading;
using WpfSpLib.Common;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;
using WpfSpLibDemo.TestViews;

namespace WpfSpLibDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: IHasDialogHost
    {
        public double Value { get; set; } = 2.4;

        private const int NumberOfTestSteps = 5;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            cbCulture.ItemsSource = CultureInfos;
            cbCulture.SelectedValue = Thread.CurrentThread.CurrentUICulture;

            ControlHelper.HideInnerBorderOfDatePickerTextBox(this, true);
            InitMemoryLeakTest();

            LocComboBox.ItemsSource = Samples.TestLocModel.TestLocModelData;
        }

        private static readonly string[] _cultures = { "", "sq", "uk", "en", "en-GB", "km", "yo", "de" };

        public List<CultureInfo> CultureAllInfos { get; set; } = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(c => c.DisplayName).ToList();
        public List<CultureInfo> CultureInfos { get; set; } = CultureInfo
            .GetCultures(CultureTypes.InstalledWin32Cultures).Where(c => Array.IndexOf(_cultures, c.Name) != -1)
            .OrderBy(c => c.DisplayName).ToList();

        private void CbCulture_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var newCulture = e.AddedItems[0] as CultureInfo;
                LocalizationHelper.SetLanguage(newCulture);
            }
        }

        private void MwiStartup_OnClick(object sender, RoutedEventArgs e) => new MwiStartup().Show();
        private void MwiStartupDemo_OnClick(object sender, RoutedEventArgs e) => new MwiStartupDemo().Show();
        private void TabDemo_OnClick(object sender, RoutedEventArgs e) => new TabDemo().Show();
        private void MwiBootstrapColorTests_OnClick(object sender, RoutedEventArgs e) => new MwiBootstrapColorTests().Show();
        private void MwiTests_OnClick(object sender, RoutedEventArgs e) => new MwiTests().Show();
        private void ResizableControlTests_OnClick(object sender, RoutedEventArgs e) => new ResizableControlTests().Show();
        private void TimePickerTest_OnClick(object sender, RoutedEventArgs e) => new TimePickerTests().Show();
        private void ObjectEditorTest_OnClick(object sender, RoutedEventArgs e) => new ObjectEditorTests().Show();
        private void WatermarkTest_OnClick(object sender, RoutedEventArgs e) => new WatermarkTests().Show();
        private void DatePickerEffectTest_OnClick(object sender, RoutedEventArgs e) => new DatePickerEffectTests().Show();
        private void RippleEffectTest_OnClick(object sender, RoutedEventArgs e) => new RippleEffectTests().Show();
        private void CalculatorTest_OnClick(object sender, RoutedEventArgs e) => new CalculatorTests().Show();
        private void DropDownButtonTest_OnClick(object sender, RoutedEventArgs e) => new DropDownButtonTests().Show();
        private void NumericBoxTest_OnClick(object sender, RoutedEventArgs e) => new NumericBoxTests().Show();
        private void KeyboardTest_OnClick(object sender, RoutedEventArgs e) => new VirtualKeyboardTests().Show();
        private void ColorControlTest_OnClick(object sender, RoutedEventArgs e) => new ColorControlTests().Show();
        private void BootstrapButtonTests_OnClick(object sender, RoutedEventArgs e) => new BootstrapButtonTests().Show();
        private void ChromeTest_OnClick(object sender, RoutedEventArgs e) => new ChromeTests().Show();
        private void ButtonStyleTests_OnClick(object sender, RoutedEventArgs e) => new ButtonStyleTests().Show();
        private void FormControlStyleTests_OnClick(object sender, RoutedEventArgs e) => new FormControlStylesTests().Show();
        private void ExpanderStyleTests_OnClick(object sender, RoutedEventArgs e) => new ExpanderStyleTests().Show();
        private void DragDropTests_OnClick(object sender, RoutedEventArgs e) => new DragDropTests().Show();

        private void FocusEffectTests_OnClick(object sender, RoutedEventArgs e) => new FocusEffectTests().Show();
        private void TextBoxTests_OnClick(object sender, RoutedEventArgs e) => new TextBoxTests().Show();

        private void CountryFlagList_OnClick(object sender, RoutedEventArgs e) => new CountryFlagList().Show();
        private void FlagTests_OnClick(object sender, RoutedEventArgs e) => new FlagTests().Show();
        private void ImageConverterTests_OnClick(object sender, RoutedEventArgs e) => new ImageConvertorTests().Show();
        private void DataGridTest_OnClick(object sender, RoutedEventArgs e) => new DataGridTest().Show();

        private void OnTestButtonClick(object sender, RoutedEventArgs e)
        {
        }

        private async void MemoryUsageInfoOnClick(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            await Task.Delay(2000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            //
            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

            var a1 = GC.GetTotalMemory(true);
            if (Debugger.IsAttached)
                Debug.Print($"Memory usage: {a1:N0}");
            else
                MessageBox.Show($"Memory usage: {a1:N0}");

            var t = Tips.TryGetType("MS.Internal.WeakEventTable");
            var fi = t.GetField("_currentTable", BindingFlags.NonPublic | BindingFlags.Static);
            var fiData = t.GetField("_dataTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var table = fi.GetValue(null);
            var data = fiData.GetValue(table) as Hashtable;
            if (Debugger.IsAttached)
                Debug.Print($"Weak refs: {data.Count}");
            else
                MessageBox.Show($"Weak refs: {data.Count}");
        }

        private Hashtable weakRefDataCopy;
        private void OnSaveWeakRefsClick(object sender, RoutedEventArgs e)
        {
            var t = Tips.TryGetType("MS.Internal.WeakEventTable");
            // var pi = t.GetProperty("CurrentWeakEventTable", BindingFlags.NonPublic | BindingFlags.Static);
            var fi = t.GetField("_currentTable", BindingFlags.NonPublic | BindingFlags.Static);
            var fiData = t.GetField("_dataTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var fiEventName = t.GetField("_eventNameTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var fiManager = t.GetField("_managerTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var table = fi.GetValue(null);
            var data = fiData.GetValue(table) as Hashtable;
            var eventName = fiEventName.GetValue(table) as Hashtable;
            var mamager = fiManager.GetValue(table) as Hashtable;
            // if (weakRefDataCopy == null)
                weakRefDataCopy = data.Clone() as Hashtable;
            Debug.Print($"Weak Data: {data.Count}");
        }

        private void OnCompareWeakRefsClick(object sender, RoutedEventArgs e)
        {
            var t = Tips.TryGetType("MS.Internal.WeakEventTable");
            // var pi = t.GetProperty("CurrentWeakEventTable", BindingFlags.NonPublic | BindingFlags.Static);
            var fi = t.GetField("_currentTable", BindingFlags.NonPublic | BindingFlags.Static);
            var fiData = t.GetField("_dataTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var fiEventName = t.GetField("_eventNameTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var fiManager = t.GetField("_managerTable", BindingFlags.NonPublic | BindingFlags.Instance);
            var table = fi.GetValue(null);
            var data = fiData.GetValue(table) as Hashtable;
            var eventName = fiEventName.GetValue(table) as Hashtable;
            var mamager = fiManager.GetValue(table) as Hashtable;
            var temp = data.Clone() as Hashtable;

            var diffKeys = temp.Keys.OfType<object>().Except(weakRefDataCopy.Keys.OfType<object>()).ToList();
            var diffData = new Hashtable();
            foreach (var key in temp.Keys)
            {
                if (diffKeys.Contains(key))
                    diffData.Add(key, temp[key]);
            }
            var aa1 = diffKeys.Select(a => a.GetType());
            var aa2 = diffKeys.Select(a=> GetStringOfEventKey(a)).ToList();
            var aa3 = aa2.GroupBy(a => a).Select(a => new {a.Key, Count = a.Count() });
            if (Debugger.IsAttached)
                Debug.Print($"New WeakRefs: {diffKeys.Count}");
            else
                MessageBox.Show($"New WeakRefs: {diffKeys.Count}");
        }

        private string GetStringOfEventKey(object eventKey)
        {
            var o = GetValuesOfEventKey(eventKey);
            return $"{o.Item1.GetType().Name}, {(o.Item2 == null ? "Null" : o.Item2.GetType().Name)}";
        }
        private Tuple<object, object> GetValuesOfEventKey(object eventKey)
        {
            var t = eventKey.GetType();
            var piManager = t.GetProperty("Manager", BindingFlags.Instance | BindingFlags.NonPublic);
            var piSource = t.GetProperty("Source", BindingFlags.Instance | BindingFlags.NonPublic);
            var manager = piManager.GetValue(eventKey);
            var source = piSource.GetValue(eventKey);
            return new Tuple<object, object>(manager, source);
        }

        private void OnCleanupWeakRefTableClick(object sender, RoutedEventArgs e)
        {
            CleanupWeakEventTable();
        }
        private void CleanupWeakEventTable()
        {
            var t = Tips.TryGetType("MS.Internal.WeakEventTable");
            // var t = typeof(BindingOperations);
            var mi = t.GetMethod("Cleanup", BindingFlags.NonPublic | BindingFlags.Static);
            mi.Invoke(null, null);
        }

        #region ============  Memory leak tests  =========
        //============================
        //============================
        private async Task RunTests(Func<Task> test, string testName)
        {
            for (var k = 0; k < NumberOfTestSteps; k++)
            {
                await test();

                await Task.Delay(1000);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                CleanupWeakEventTable();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                CleanupWeakEventTable();

                var a12 = GC.GetTotalMemory(true);

                Debug.Print($"Test{k}: {a12:N0}, {testName}");

                await Task.Delay(1000);
            }
        }

        private async Task RunSimpleTest(Type wndType)
        {
            await RunTests(async () =>
            {
                var wnd = Activator.CreateInstance(wndType) as Window;
                wnd.Show();
                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;
                wnd.Close();
            }, wndType.Name);
        }

        private Dictionary<string, Func<Task>> _memoryLeakTests;

        private void InitMemoryLeakTest()
        {
            _memoryLeakTests = new Dictionary<string, Func<Task>>
            {
                {"MwiStartup",  async () => await RunSimpleTest(typeof(MwiStartupDemo))},
                {"MwiStartupThemeSelector",  MwiStartupThemeSelectorMemoryTest},
                {"MwiBootstrapColor",  async () => await RunSimpleTest(typeof(MwiBootstrapColorTests))},
                {"PopupResizeControl",  PopupResizeControlMemoryTest},
                {"MwiChild",  BootstrapColorMemoryTest},
                {"ResizableControl",  ResizableControlMemoryTest},
                {"DatePickerEffect",  async () => await RunSimpleTest(typeof(DatePickerEffectTests))},
                {"WatermarkEffect",  async () => await RunSimpleTest(typeof(WatermarkTests))},
                {"ButtonStyles",  async () => await RunSimpleTest(typeof(ButtonStyleTests))},
                {"RippleEffect",  async () => await RunSimpleTest(typeof(RippleEffectTests))},
                {"Calculator",  async () => await RunSimpleTest(typeof(CalculatorTests))},
                {"NumericBox",  async () => await RunSimpleTest(typeof(NumericBoxTests))},
                {"TimePicker",  async () => await RunSimpleTest(typeof(TimePickerTests))},
                {"ColorControl",  async () => await RunSimpleTest(typeof(ColorControlTests))},
                {"KnownColorsOfColorControl",  KnownColorsOfColorControlMemoryTest},
            };

            foreach (var kvp in _memoryLeakTests)
            {
                var btn = new Button {Margin = new Thickness(5), Content = kvp.Key};
                btn.Click += OnMemoryLeakTestClick;
                MemoryLeakTests.Children.Add(btn);
            }
        }

        //==============
        private async Task MwiStartupThemeSelectorMemoryTest()
        {
            await RunTests(async () =>
            {
                var wnd = new MwiStartupDemo();
                wnd.Show();

                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Start();
                timer.Tick += (sender2, args) =>
                {
                    timer.Stop();
                    var a3 = wnd.TopControl.GetVisualChildren().OfType<MwiContainer>().FirstOrDefault();
                    var a4 = AdornerLayer.GetAdornerLayer(a3);
                    var selectorHost = a4.GetVisualChildren().OfType<MwiChild>().FirstOrDefault();
                    // var selectorHost = Keyboard.FocusedElement as MwiChild;
                    selectorHost?.CmdClose.Execute(null);
                };
                wnd.TopControl.CmdSelectTheme.Execute(null);

                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                wnd.Close();
            }, "MwiStartupThemeSelector");
        }

        private async Task PopupResizeControlMemoryTest()
        {
            await RunTests(new Func<Task>(async () =>
            {
                var wnd = new TextBoxTests();
                wnd.Show();

                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                var a1 = wnd.TestTextBox.GetVisualChildren().OfType<ToggleButton>().FirstOrDefault(a => a.Name.EndsWith("Keyboard"));
                if (a1 != null)
                    a1.IsChecked = true;

                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                if (a1 != null)
                    a1.IsChecked = false;

                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                wnd.Close();
            }), "PopupResizeControl");
        }

        private async Task BootstrapColorMemoryTest()
        {
            var wnd = new MwiBootstrapColorTests();
            wnd.Show();
            await Task.Delay(300);
            await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;
            await wnd.RunTest(NumberOfTestSteps);
            await Task.Delay(300);
            await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;
            wnd.Close();
        }

        private async Task ResizableControlMemoryTest()
        {
            var wnd = new ResizableControlTests();
            wnd.Show();
            await Task.Delay(300);
            await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;
            await wnd.AutomateAsync(NumberOfTestSteps);
            await Task.Delay(300);
            await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;
            wnd.Close();
        }

        private async Task KnownColorsOfColorControlMemoryTest()
        {
            await RunTests(async () =>
            {
                var wnd = new ColorControlTests();
                wnd.Show();

                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                var a1 = wnd.ColorControl.GetVisualChildren().OfType<TabControl>().FirstOrDefault();
                a1.SelectedIndex = 2;
                await Task.Delay(300);
                await wnd.Dispatcher.InvokeAsync(() => { }, DispatcherPriority.ApplicationIdle).Task;

                wnd.Close();
            }, "KnownColorsOfColorControl");
        }

        //==================
        private void OnMemoryLeakTestClick(object sender, RoutedEventArgs e)
        {
            var key = (string)((ContentControl)sender).Content;
            var fn = _memoryLeakTests[key];
            fn();
        }

        private async void OnRunAllTestsClick(object sender, RoutedEventArgs e)
        {
            // foreach (var kvp in _memoryLeakTests.Reverse())
            foreach (var kvp in _memoryLeakTests)
                await kvp.Value();
        }
        #endregion

        private void OnRunAllTestsAsyncClick(object sender, RoutedEventArgs e)
        {
            foreach (var kvp in _memoryLeakTests)
                kvp.Value();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var mb = new DialogBox(DialogBox.DialogBoxKind.Info)
                { Caption = "Caption", Message = "Message", Buttons = new[] {"OK", "CCCCC fewf"},
                    Details = @"System.Data.SqlClient.SqlException (0x80131904): Cannot open database 'TempSoft' requested by the login. The login failed.
Login failed for user 'DESKTOP-PC\User'.
   at System.Data.SqlClient.SqlInternalConnectionTds..ctor(DbConnectionPoolIdentity identity, SqlConnectionString connectionOptions, SqlCredential credential, Object providerInfo, String newPassword, SecureString newSecurePassword, Boolean redirectedUserInstance, SqlConnectionString userConnectionOptions, SessionData reconnectSessionData, Boolean applyTransientFaultHandling, String accessToken)
   at System.Data.SqlClient.SqlConnectionFactory.CreateConnection(DbConnectionOptions options, DbConnectionPoolKey poolKey, Object poolGroupProviderInfo, DbConnectionPool pool, DbConnection owningConnection, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionFactory.CreatePooledConnection(DbConnectionPool pool, DbConnection owningObject, DbConnectionOptions options, DbConnectionPoolKey poolKey, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionPool.CreateObject(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at System.Data.ProviderBase.DbConnectionPool.UserCreateRequest(DbConnection owningObject, DbConnectionOptions userOptions, DbConnectionInternal oldConnection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, UInt32 waitForMultipleObjectsTimeout, Boolean allowCreate, Boolean onlyOneCheckConnection, DbConnectionOptions userOptions, DbConnectionInternal & connection)
   at System.Data.ProviderBase.DbConnectionPool.TryGetConnection(DbConnection owningObject, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal & connection)
   at System.Data.ProviderBase.DbConnectionFactory.TryGetConnection(DbConnection owningConnection, TaskCompletionSource`1 retry, DbConnectionOptions userOptions, DbConnectionInternal oldConnection, DbConnectionInternal & connection)
   at System.Data.ProviderBase.DbConnectionInternal.TryOpenConnectionInternal(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   at System.Data.ProviderBase.DbConnectionClosed.TryOpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory, TaskCompletionSource`1 retry, DbConnectionOptions userOptions)
   at System.Data.SqlClient.SqlConnection.TryOpen(TaskCompletionSource`1 retry)
   at System.Data.SqlClient.SqlConnection.Open()
   at DGCore.DB.DbUtils.Connection_Open(DbConnection connection) in E:\Apps\project - 2020\DG.4.0\DGCore\DB\DbUtils.cs:line 69
   at DGCore.DB.DbUtils.GetSchemaTable(DbCommand cmd) in E:\Apps\project - 2020\DG.4.0\DGCore\DB\DbUtils.cs:line 134
   at DGCore.DB.DbSchemaTable..ctor(DbCommand cmd, String connectionKey, Boolean isTable) in E:\Apps\project - 2020\DG.4.0\DGCore\DB\DbSchemaTable.cs:line 62
   at DGCore.DB.DbSchemaTable.GetSchemaTable(DbCommand cmd, String connectionKey) in E:\Apps\project - 2020\DG.4.0\DGCore\DB\DbSchemaTable.cs:line 20
   at DGCore.DB.DbCmd.GetSchemaTable() in E:\Apps\project - 2020\DG.4.0\DGCore\DB\DbCmd.cs:line 84
   at DGCore.Filters.FilterList..ctor(DbCmd cmd, Type itemType, Dictionary`2 columnAttributes) in E:\Apps\project - 2020\DG.4.0\DGCore\Filters\FilterList.cs:line 22
   at DGCore.Misc.DataDefinition.get_WhereFilter() in E:\Apps\project - 2020\DG.4.0\DGCore\Misc\DataDefiniton.cs:line 63
   at DGView.Views.MwiLeftPanelView.<> c__DisplayClass18_0.< ActivateMenuOption > b__4() in E:\Apps\project - 2020\DG.4.0\DGView\Views\MwiLeftPanelView.xaml.cs:line 197
ClientConnectionId:19c8f343 - eda9 - 495e-a1ea - b9c83adf12e5
Error Number:4060, State:1, Class:11\"};
            var a1= mb.ShowDialog();
            Debug.Print(a1);
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            var s = "ng Message long Message long Message long Message long Message long Message long Message long Message ";
            for (var k = 0; k < 3; k++)
            {
                s = k + s + k + s;
            }

            s += " !!! END!";
            new DialogBox(DialogBox.DialogBoxKind.Error)
            {
                Caption = "Caption",
                Message =
                    "long Message long Message long Message long Message long Message long Message long Message long Message ",
                Buttons = new[] { "OK", "Cancel" },
                Details = s
            }.ShowDialog();
        }

        public FrameworkElement GetDialogHost() => DialogHost;

        private void ChangeLanguage_OnClick(object sender, RoutedEventArgs e)
        {
            if (Equals(LocalizationHelper.CurrentCulture, new CultureInfo("en")))
                LocalizationHelper.SetLanguage(new CultureInfo("uk"));
            else
                LocalizationHelper.SetLanguage(new CultureInfo("en"));

            var a1 = Application.Current.Resources["Loc:MwiStartup.Label.ScaleSlider"];
        }
    }
}
