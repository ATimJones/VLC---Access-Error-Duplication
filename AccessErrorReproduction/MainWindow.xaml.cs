using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LibVLCSharp.Shared;

namespace AccessErrorReproduction
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LibVLC _libVLC;
        const string Username = "username";
        const string Password = "password";
        public MainWindow()
        {
            Core.Initialize();
            _libVLC = new LibVLC(enableDebugLogs: true);
            _libVLC.SetLogFile("vlclog.txt");
            InitializeComponent();
            Task task = PostLogin();
        }

        public async Task PostLogin()
        {
            var tcs = new TaskCompletionSource<bool>();

            _libVLC.SetDialogHandlers((title, text) => Task.CompletedTask,
                (dialog, title, text, username, store, token) =>
                {
                    // show UI dialog
                    // On "OK" call PostLogin
                    dialog.PostLogin(Username, Password, false);
                    tcs.TrySetResult(true);
                    return Task.CompletedTask;
                },
                (dialog, title, text, type, cancelText, actionText, secondActionText, token) => Task.CompletedTask,
                (dialog, title, text, indeterminate, position, cancelText, token) => Task.CompletedTask,
                (dialog, position, text) => Task.CompletedTask);

            var mp = new LibVLCSharp.Shared.MediaPlayer(_libVLC)
            {
                //Media = new Media(_libVLC, "http://192.168.2.179/stream1", FromType.FromLocation) //no auth, works
                //Media = new Media(_libVLC, "http://192.168.2.133/stream1", FromType.FromLocation) //requires auth, fails
                Media = new Media(_libVLC, "http://httpbin.org/basic-auth/user/passwd", FromType.FromLocation) //same failure
            };

            mp.Play();


            await tcs.Task;
            Console.WriteLine(tcs.Task.Result);
        }
    }
}
