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
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
                {Host = DialogHost, Caption = "Caption", Message = "Test message 0 1 2 3 4 5"};
            box.Show();
            Debug.Print($"Message Sync");
        }

        private async void MessageAsync_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
            {
                Caption = "Caption", Message = "Test message",
                Buttons = new[] {"OK", "Cancel", "Right", "Left"}
            };
            var result = await box.ShowAsync();
            Debug.Print($"MessageAsync: {result}");
        }

        private void MessageDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
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
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
            {
                Caption = "Caption",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var result = await box.ShowAsync();
        }

        private async void VeryLongMessage_OnClick(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
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
            var box = new DialogMessage
            {
                Caption = "Show Sync",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left"}
            };
            box.Show();
        }
        private async void OnClickAsyncMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage
            {
                Caption = "Show Sync",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var a1 = await box.ShowAsync();
        }
        private void OnClickDialogMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage
            {
                Caption = "Show Dialog",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var a1 = box.ShowDialog();
        }
        private void OnClickQuestionMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text"
            };
            box.ShowDialog();
        }
        private void OnClickStopMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Stop)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text"
            };
            box.ShowDialog();
        }
        private void OnClickErrorMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Error)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text",
                Details = "Error message details"
            };
            box.ShowDialog();
        }
        private async void OnClickWarningMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Warning)
            {
                Caption = "Caption of Dialog box",
                Message = "Message (Show Async)",
                Buttons = new[] { "OK", "Cancel", "Right", "Left" }
            };
            var aa = await box.ShowAsync();

            var box2 = new DialogMessage(DialogMessage.DialogBoxKind.Info)
            {
                Message = $"You pressed '{aa ?? "X"}' button",
                Buttons = new[] { "OK"}
            };
            await box2.ShowAsync();
        }
        private void OnClickInformationMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Info)
            {
                Caption = "Caption of Dialog box",
                Message = "Message text Message text Message text Message text Message text Message text",
                Buttons = new [] {"OK"}
            };
            box.ShowDialog();
        }
        private void OnClickSuccessMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Success)
            {
                Caption = "Caption of Dialog box",
                Message = "Message (Show) ",
                Buttons = new[] { "OK", "Cancel" },
                IsCloseButtonVisible = false
            };
            var aa = box.ShowDialog();

            var box2 = new DialogMessage(DialogMessage.DialogBoxKind.Info)
            {
                Message = $"You pressed '{aa ?? "X" }' button",
                Buttons = new[] { "OK" }
            };
            box2.ShowDialog();
        }
        private void OnClickShortMessage(object sender, RoutedEventArgs e)
        {
            var box = new DialogMessage(DialogMessage.DialogBoxKind.Question)
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
