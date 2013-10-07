using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            LoadRecentNotifications();
        }

        private void BroadcastNewNotification(Notification notification)
        {
            GetClients().addNotification(notification);
        }

        public IEnumerable<Notification> RecentNotifications()
        {
            return _notifications.Values;
        }

        private void LoadNotifications()
        {
            var notificationList = new List<Notification>();
            
            
                
            //}.ForEach(notification => _notifications.TryAdd(notification.Id, notification));
            var brokerdMessage = QueueManager.ReceiveMessages();
            //var message = new Notification { Id = Convert.ToInt32(brokerdMessage.MessageId), Title = "Man United and Man City target Mats Hummels...", Description = "Inter Milan are considering a summer move for Manchester City's 26-year-old defender Aleksandar Kolarov.", Importance = Importance.High, TimeCreated = DateTime.Now.AddHours(-9).ToShortTimeString() };

            notificationList.ForEach();
        }

        private static dynamic GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<ServiceBusQueueNotificationHub>().Clients;
        }
    }
}