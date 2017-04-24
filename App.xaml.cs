using System;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NotePad_Sharp
{
    sealed partial class App : Application
    {
        public App() { InitializeComponent(); }

        Frame RootFrame => (Window.Current.Content as Frame)
            ?? (Window.Current.Content = new Frame()) as Frame;

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            var jumpList = e.TileId == "App" && !string.IsNullOrEmpty(e.Arguments);
            if (jumpList)
            {
                try
                {
                    var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(e.Arguments);
                    if (RootFrame.Content == null)
                        RootFrame.Navigate(typeof(MainPage), file);
                    else
                        FileReceived?.Invoke(this, file);
                }
                catch (Exception)
                { throw; }
            }
            else
                RootFrame.Navigate(typeof(MainPage), e.Arguments);
            Window.Current.Activate();
        }

        protected override void OnFileActivated(FileActivatedEventArgs args)
        {
            if (!args.Files.Any())
                return;
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                FileReceived?.Invoke(this, args.Files.First() as Windows.Storage.StorageFile);
            else
                RootFrame.Navigate(typeof(MainPage), args.Files.First());
            Window.Current.Activate();
        }

        public static event EventHandler<Windows.Storage.StorageFile> FileReceived;
    }
}
