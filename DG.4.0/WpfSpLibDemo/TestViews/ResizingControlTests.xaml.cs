using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfSpLib.Controls;
using WpfSpLib.Helpers;
using WpfSpLibDemo.Samples;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for ResizableControl.xaml
    /// </summary>
    public partial class ResizableControlTests : Window, IHasDialogHost
    {
        public ResizableControlTests()
        {
            InitializeComponent();

            var resizableControl = new ResizableControl {Content = new ResizableContentTemplateSample{Content = "Content"}, Position = new Point(110, 110)};
            DialogHost.Children.Add(resizableControl);

            var resizableControl2 = new ResizableControl {Content = new ResizableSample(), Position = new Point(5, 390), ToolTip = "No Width/Height"};
            DialogHost.Children.Add(resizableControl2);

            var resizableControl3 = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Position = new Point(250,140),
                Width = 150,
                Height = 150,
                LimitPositionToPanelBounds = true,
                ToolTip = "Width/Height=150"
            };
            resizableControl3.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e) => DialogHost.Children.Remove(resizableControl3)));
            DialogHost.Children.Add(resizableControl3);
        }

        public async Task AutomateAsync(int numberOfTestSteps)
        {
            for (var k = 0; k < numberOfTestSteps; k++)
                await Automate_Step(k);
        }
        private async void Automate_OnClick(object sender, RoutedEventArgs e)
        {
            for (var k = 0; k < 5; k++)
                await Automate_Step(k);
        }

        private async Task Automate_Step(int step)
        {
            var control = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 150,
                Height = 150,
                LimitPositionToPanelBounds = true,
                ToolTip = "Width/Height=150"
            };
            control.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e1) => DialogHost.Children.Remove(control)));
            DialogHost.Children.Add(control);
            ControlHelper.SetFocus(control);

            await Task.Delay(1000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var a11 = GC.GetTotalMemory(true);

            // control.CommandBindings[0].Command.Execute(null);
            DialogHost.Children.Remove(control);

            await Task.Delay(1000);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var a12 = GC.GetTotalMemory(true);

            Debug.Print($"Test{step}: {a12:N0}");
        }

        private void AddContent_OnClick(object sender, RoutedEventArgs e)
        {
            var control = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 150,
                Height = 150,
                LimitPositionToPanelBounds = true,
                ToolTip = "Width/Height=150"
            };
            control.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e1) => DialogHost.Children.Remove(control)));
            DialogHost.Children.Add(control);
            ControlHelper.SetFocus(control);
        }

        private void AddWindowPanelSync_OnClick(object sender, RoutedEventArgs e)
        {
            var a1 = new DialogAdorner { CloseOnClickBackground = true };

            var content = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 250,
                Height = 250,
                LimitPositionToPanelBounds = true,
                ToolTip = "Width/Height=250"
            };
            a1.ShowContent(content);

            var content2 = new ResizableControl
            {
                Name = "Test",
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 150,
                Height = 150,
                LimitPositionToPanelBounds = true,
                ToolTip = "Width/Height=150"
            };
            a1.ShowContent(content2);
            Debug.Print($"AddWindowPanelSync_OnClick method finished");
        }

        private async void AddWindowPanelAsync_OnClick(object sender, RoutedEventArgs e)
        {
            var a1 = new DialogAdorner { CloseOnClickBackground = true };

            var content1 = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 250,
                Height = 250,
                LimitPositionToPanelBounds = true,
                ToolTip = "Content1 Width/Height=250"
            };
            await a1.ShowContentAsync(content1);

            var content2 = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 150,
                Height = 150,
                LimitPositionToPanelBounds = true,
                ToolTip = "Content2 Width/Height=150"
            };
            await a1.ShowContentAsync(content2);
            await a1.WaitUntilClosed();

            var content3 = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 200,
                Height = 250,
                LimitPositionToPanelBounds = true,
                ToolTip = "Content3 Width/Height=200/250"
            };

            a1.ShowContent(content3);
            await a1.WaitUntilClosed();

            Debug.Print($"AddWindowPanelAsync_OnClick method finished");
        }

        private void AddWindowPanelDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var a1 = new DialogAdorner { CloseOnClickBackground = true };

            var content1 = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 250,
                Height = 250,
                LimitPositionToPanelBounds = true,
                ToolTip = "Width/Height=250"
            };
            a1.ShowContentDialog(content1);
            Debug.Print($"AddWindowPanelDialog_OnClick method finished");
        }

        private void MessageSync_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
                {Host = DialogHost, Caption = "Caption", Message = "Test message 0 1 2 3 4 5"};
            box.Show();
            Debug.Print($"Message Sync");
        }

        private async void MessageAsync_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
            {
                Caption = "Caption", Message = "Test message",
                Buttons = new[] {"OK", "Cancel", "Right", "Left"}
            };
            var result = await box.ShowAsync();
            Debug.Print($"MessageAsync: {result}");
        }

        private void MessageDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
            {
                Caption = "Show Dialog",
                Message = "Test message",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var result = box.ShowDialog();
            Debug.Print($"MessageDialog: {result}");
        }

        private async void LongMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
            {
                Caption = "Caption",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var result = await box.ShowAsync();
        }

        private async void VeryLongMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
            {
                Caption = "Caption",
                Message = "Message text Message text Message text Message text Message text Message textMessage text Message text Message text Message text Message text Message textMessage text Message text Message text Message text Message text Message textMessage text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var result = await box.ShowAsync();
        }

        //===============================
        private void OnClickSyncMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox
            {
                Caption = "Show Sync",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left"}
            };
            box.Show();
        }
        private async void OnClickAsyncMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox
            {
                Caption = "Show Sync",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var a1 = await box.ShowAsync();
        }
        private void OnClickDialogBox(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox
            {
                Caption = "Show Dialog",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var a1 = box.ShowDialog();
        }
        private void OnClickQuestionMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text"
            };
            box.ShowDialog();
        }
        private void OnClickStopMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Stop)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text"
            };
            box.ShowDialog();
        }
        private void OnClickErrorMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Error)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" },
                Details = @"System.Data.SqlClient.SqlException (0x80131904): Cannot open database ""TempSoft"" requested by the login. The login failed.
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
ClientConnectionId:a2cae53b - 2205 - 4998 - 997a - e1c750769d2f
Error Number:4060, State:1, Class:11"
            };
            box.ShowDialog();
        }
        private async void OnClickWarningMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Warning)
            {
                Caption = "Caption of Dialog box",
                Message = "Message (Show Async)",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var aa = await box.ShowAsync();

            var box2 = new DialogBox(DialogBox.DialogBoxKind.Info)
            {
                Message = $"You pressed '{aa ?? "X"}' button",
                Buttons = new[] { "OK"}
            };
            await box2.ShowAsync();
        }
        private void OnClickInformationMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Info)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new [] {"OK"}
            };
            box.ShowDialog();
        }
        private void OnClickSuccessMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Success)
            {
                Caption = "Caption of Dialog box",
                Message = "Message (Show) ",
                Buttons = new[] { "OK", "Cancel" },
                IsCloseButtonVisible = false
            };
            var aa = box.ShowDialog();

            var box2 = new DialogBox(DialogBox.DialogBoxKind.Info)
            {
                Message = $"You pressed '{aa ?? "X" }' button",
                Buttons = new[] { "OK" }
            };
            box2.ShowDialog();
        }
        private void OnClickShortMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogBox(DialogBox.DialogBoxKind.Question)
            {
                Caption = "Show dialog",
                Message = "Test message 0 1 2 3 4"
            };
            var aa = box.ShowDialog();
        }

        private void AddWindowedContent_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new Window
            {
                Style = FindResource("HeadlessWindow") as Style,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                // Opacity = 0
            };
            var control = new ResizableControl
            {
                Content = new ResizableSample { Width = double.NaN, Height = double.NaN },
                Width = 250,
                Height = 250,
                LimitPositionToPanelBounds = false,
                ToolTip = "Width/Height=250"
            };
            control.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (s, e1) => ((Window)control.Parent).Close()));
            window.Content = control;
            window.Show();

        }

        public FrameworkElement GetDialogHost() => DialogHost;
    }
}
