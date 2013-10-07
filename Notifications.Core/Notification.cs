﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notifications.Core
{
    public class Notification
    {
        public string Id { get; set; }
        public Importance Importance { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TimeCreated { get; set; }
        public string User { get; set; }
        public string Group { get; set; }
    }

    public enum Importance
    {
        High,
        Medium,
        Low
    }
}
