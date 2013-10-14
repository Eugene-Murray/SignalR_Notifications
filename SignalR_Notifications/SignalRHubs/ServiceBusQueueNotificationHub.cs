using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.ServiceBus.Messaging;
using SignalR;
using SignalR.Hubs;
using SignalR_Notifications.Models;

namespace SignalR_Notifications.SignalRHubs
{
    public class ServiceBusQueueNotificationHub : Hub
    {
        private readonly ConcurrentDictionary<int, Notification> _notifications = new ConcurrentDictionary<int, Notification>();
        
        public ServiceBusQueueNotificationHub()
        {
            LoadNotifications();

            AddNewNotifications();
        }

        public void AddNewNotifications()
        {
            while (true)
            {
                try
                {
                    var brokerdMessage = QueueManager.ReceiveMessage();
                    if (brokerdMessage == null)
                    {
                        //no more messages in the queue
                        break;
                    }
                    else
                    {
                        var message = new Notification
                        {
                            Id = Convert.ToInt32(brokerdMessage.MessageId),
                            Title = brokerdMessage.GetBody<string>().Take(10).ToString(),
                            Description =
                                brokerdMessage.GetBody<string>(),
                            Importance = Importance.High,
                            TimeCreated = DateTime.Now.ToString()
                        };
                        BroadcastNewNotification(message);

                        brokerdMessage.Complete();
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
                        QueueManager.HandleTransientErrors(e);
                    }
                }

            }
            QueueManager.queueClient.Close();
        }

        private void BroadcastNewNotification(Notification notification)
        {
            GetClients().addNotification(notification);
        }

        public IEnumerable<Notification> QueuedNotifications()
        {
            return _notifications.Values;
        }

        private void LoadNotifications()
        {
            var notificationList = new List<Notification>();

            while (true)
            {
                try
                {
                    var brokerdMessage = QueueManager.ReceiveMessage();
                    if (brokerdMessage == null)
                    {
                        //no more messages in the queue
                        break;
                    }
                    else
                    {
                        var message = new Notification
                            {
                                Id = Convert.ToInt32(brokerdMessage.MessageId),
                                Title = brokerdMessage.GetBody<string>().Take(10).ToString(),
                                Description =
                                    brokerdMessage.GetBody<string>(),
                                Importance = Importance.High,
                                TimeCreated = DateTime.Now.ToString()
                            };
                        notificationList.Add(message);
                        notificationList.ForEach(notification => _notifications.TryAdd(notification.Id, notification));

                        brokerdMessage.Complete();
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
                        QueueManager.HandleTransientErrors(e);
                    }
                }

            }
            QueueManager.queueClient.Close();
        }

        private static dynamic GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<ServiceBusQueueNotificationHub>().Clients;
        }
    }
}