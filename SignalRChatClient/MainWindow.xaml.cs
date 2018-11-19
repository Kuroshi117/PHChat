#region snippet_MainWindowClass
using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatClient
{
    public partial class MainWindow : Window
    {
        HubConnection connection;
        
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                //WHEN DONE TESTING LOCALLY...
                //MAKE SURE NEXT LINE ENDS WITH THE URL AND HUB NAME THAT MATCHES
                //OF YOUR PUBLISHED CHATROOM PROJECT.
                
                .WithUrl("http://phalgorithmschat.azurewebsites.net/chat")
                .Build();

            #region snippet_ClosedRestart
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };
            #endregion
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            #region snippet_ConnectionOn
            connection.On<string, string>("broadcastMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                   var newMessage = $"{user}: {message}";
                   messagesList.Items.Add(newMessage);
                });
            });
            #endregion

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                connectButton.IsEnabled = false;
                sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            #region snippet_ErrorHandling
            try
            {
                #region snippet_InvokeAsync
                await connection.InvokeAsync("BroadcastMessage", 
                    userTextBox.Text, messageTextBox.Text);
                #endregion
            }
            catch (Exception ex)
            {                
                messagesList.Items.Add(ex.Message);                
            }
            #endregion
        }
    }
}
#endregion
