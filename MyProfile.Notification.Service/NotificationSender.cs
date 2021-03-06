using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Notification.Service
{
    public class NotificationSender
    {
        private BaseRepository repository;

        public NotificationSender(BaseRepository repository)
        {
            this.repository = repository;
        }

    }
}
