using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using SignalR;
using SignalR.Hubs;
using SignalR.Hosting.AspNet;
using SignalR.Infrastructure;
using SignalR_Notifications.Models;


namespace SignalR_Notifications.SignalRHubs
{
    public class NotificationsHub : Hub
    {
        private static int Id = 5;
        private readonly ConcurrentDictionary<int, Notification> _notifications = new ConcurrentDictionary<int, Notification>();
        private Timer _timer;
        private readonly int _updateInterval = 10000;

        public NotificationsHub()
        {
            LoadRecentNotifications();

           _timer = new Timer(AddNewNotification, null, _updateInterval, _updateInterval);
        }

        public IEnumerable<Notification> RecentNotifications()
        {
            return _notifications.Values;
        }

        public void AddNewNotification(object state)
        {
            var newId = Id++;
            var newNotification = new Notification { Id = newId, Title = "New Message Notification...", Description = "Some random mesage, bla bla bla", Importance = Importance.High, TimeCreated = DateTime.Now.ToShortTimeString() };
            _notifications.TryAdd(newId, newNotification);

            BroadcastNewNotification(newNotification);
        }

        private void BroadcastNewNotification(Notification notification)
        {
            GetClients().addNotification(notification);
        }


        private void LoadRecentNotifications()
        {
            new List<Notification>
            {
                new Notification { Id = 1, Title = "Man United and Man City target Mats Hummels...", Description = "Inter Milan are considering a summer move for Manchester City's 26-year-old defender Aleksandar Kolarov.", Importance = Importance.High, TimeCreated = DateTime.Now.AddHours(-9).ToShortTimeString() },
                new Notification { Id = 2, Title = "Spurs star Gareth Bale...", Description = "Bolton are set to hijack Celtic's move for out-of-favour Leicester defender Matt Mills, 25, who is rated at £3m.", Importance = Importance.Low, TimeCreated = DateTime.Now.AddHours(-8).ToShortTimeString()  },
                new Notification { Id = 3, Title = "Tottenham midfielder Luka Modric...", Description = "Tottenham midfielder Luka Modric, 26, is on his way to Manchester United with Sir Alex Ferguson offering £25m.", Importance = Importance.Low, TimeCreated = DateTime.Now.AddHours(-6).ToShortTimeString()  },
                new Notification { Id = 4, Title = "Manchester United have agreed...", Description = "Manchester United have agreed a £3m fee for 18-year-old Chilean striker Angelo Henriquez.", Importance = Importance.Medium, TimeCreated = DateTime.Now.AddHours(-4).ToShortTimeString() },
                new Notification { Id = 5, Title = "Newcastle have entered the race to sign...", Description = "Birmingham's Ben Foster, 29, is Queen's Park Rangers' top target in their search for a new first-choice goalkeeper.", Importance = Importance.Low, TimeCreated = DateTime.Now.AddHours(-1).ToShortTimeString() }
                
            }.ForEach(notification => _notifications.TryAdd(notification.Id, notification));
        }

        private static dynamic GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<NotificationsHub>().Clients;
        }
    }
}