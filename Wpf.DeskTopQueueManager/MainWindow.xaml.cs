using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Wpf.DeskTopQueueManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static QueueClient queueClient;
        private static string QueueName = "SignalR_Queue";
        private const Int16 maxTrials = 4;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendToQueue_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("SendMessage()");

            var namespaceManager = NamespaceManager.Create();

            if (namespaceManager.QueueExists(QueueName))
            {
                try
                {
                    BrokeredMessage message = new BrokeredMessage(messageBody);
                    message.MessageId = messageId;
                    
                    queueClient.Send(message);
                }
                catch (MessagingException ex)
                {
                    if (!ex.IsTransient)
                    {
                        Trace.WriteLine(ex.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }
                }
            }
            else
            {
                Trace.WriteLine("Queue does not exist: " + QueueName);
            }
        }

        public static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let's back-off for 2 seconds and retry
            Trace.WriteLine(e.Message);
            Trace.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }
    }
}
