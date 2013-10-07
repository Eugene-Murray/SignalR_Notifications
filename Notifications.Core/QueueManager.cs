using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using System.Threading;
using Microsoft.ServiceBus.Notifications;
using Notifications.Core;

namespace SignalR_Notifications.SignalRHubs
{

    public static class QueueManager
    {
        private static QueueClient queueClient;
        private static string QueueName = "SignalR_Queue";
        private const Int16 maxTrials = 4;

        private static void CreateQueue()
        {
            var namespaceManager = NamespaceManager.Create();

            Trace.WriteLine("\nCreating Queue '{0}'...", QueueName);

            // Delete if exists
            if (namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.DeleteQueue(QueueName);
            }

            namespaceManager.CreateQueue(QueueName);
        }

        private static void SendMessage(BrokeredMessage brokeredMessage)
        {
            Trace.WriteLine("SendMessage()");
            
            var namespaceManager = NamespaceManager.Create();

            if (namespaceManager.QueueExists(QueueName))
            {
                try
                {
                    queueClient.Send(brokeredMessage);
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
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

        //private static void SendMessages()
        //{
        //    queueClient = QueueClient.Create(QueueName);

        //    List<BrokeredMessage> messageList = new List<BrokeredMessage>();

        //    messageList.Add(CreateSampleMessage("1", "First message information"));
        //    messageList.Add(CreateSampleMessage("2", "Second message information"));
        //    messageList.Add(CreateSampleMessage("3", "Third message information"));

        //    Trace.WriteLine("\nSending messages to Queue...");

        //    foreach (BrokeredMessage message in messageList)
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                queueClient.Send(message);
        //            }
        //            catch (MessagingException e)
        //            {
        //                if (!e.IsTransient)
        //                {
        //                    Trace.WriteLine(e.Message);
        //                    throw;
        //                }
        //                else
        //                {
        //                    HandleTransientErrors(e);
        //                }
        //            }
        //            Trace.WriteLine(string.Format("Message sent: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
        //            break;
        //        }
        //    }

        //}

        public static Notifications.Core.Notification ReceiveMessages()
        {
            Trace.WriteLine("\nReceiving message from Queue...");
            var notificationMessage = new Notifications.Core.Notification();
            while (true)
            {
                try
                {
                    //receive messages from Queue
                    BrokeredMessage message = queueClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        Trace.WriteLine(string.Format("Message received: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                        // Further custom message processing could go here...

                        notificationMessage.Id = message.MessageId;
                        notificationMessage.Importance = Importance.Medium;
                        notificationMessage.Description = message.GetBody<string>();
                        return notificationMessage;

                        message.Complete();
                    }
                    else
                    {
                        //no more messages in the queue
                        break;
                    }
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Trace.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        HandleTransientErrors(e);
                    }
                }
            }

            queueClient.Close();
        }

        private static BrokeredMessage CreateSampleMessage(string messageId, string messageBody)
        {
            BrokeredMessage message = new BrokeredMessage(messageBody);
            message.MessageId = messageId;
            return message;
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let's back-off for 2 seconds and retry
            Trace.WriteLine(e.Message);
            Trace.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }
    }
}