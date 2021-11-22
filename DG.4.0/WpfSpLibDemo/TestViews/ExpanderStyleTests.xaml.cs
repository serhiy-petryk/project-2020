using System.Windows;
using WpfSpLib.Controls;

namespace WpfSpLibDemo.TestViews
{
    /// <summary>
    /// Interaction logic for ExpanderStyleTests.xaml
    /// </summary>
    public partial class ExpanderStyleTests : Window
    {
        public ExpanderStyleTests()
        {
            InitializeComponent();
        }

        private void OnOpenDialogClick(object sender, RoutedEventArgs e)
        {
            var s = "ng Message long Message long Message long Message long Message long Message long Message long Message ";
            for (var k = 0; k < 3; k++)
            {
                s = k + s + k + s;
            }

            s += " !!! END!";
            new DialogMessage(DialogMessage.DialogBoxKind.Error)
            {
                Caption = "Caption",
                Message =
                    "long Message long Message long Message long Message long Message long Message long Message long Message ",
                Buttons = new[] { "OK", "Cancel" },
                Details = s
            }.ShowDialog();
        }

        private void OnOpenBigDialogClick(object sender, RoutedEventArgs e)
        {
            var db = new DialogMessage(DialogMessage.DialogBoxKind.Info)
            {
                Caption = "Caption",
                Message = "Message",
                Buttons = new[] { "OK", "Cancel" },
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
Error Number:4060, State:1, Class:11\"
            };
            var a1 = db.ShowDialog();
        }
    }
}
