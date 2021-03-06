using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Hubs
{
    public class NotificationHub : Hub
    {
        private IServiceScopeFactory _scopeFactory;

        public NotificationHub(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public void Send(string message, string userName)
        {
            var z = Clients.Client("LFjPY6zBFtCcG11CF60Aww_1");
            z.SendAsync("Receive", message, userName);
            //Clients.All.SendAsync("Receive", message, userName);
        }


        public override Task OnConnectedAsync()
        {
            try
            {
                var currentUser = UserInfo.Current;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();

                    repository.Create(new HubConnect
                    {
                        ConnectionID = Context.ConnectionId,
                        DateConnect = DateTime.Now,
                        UserConnectID = currentUser.ID
                    }, true);
                }
            }
            catch (Exception ex)
            {

            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            try
            {
                var currentUser = UserInfo.Current;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();

                    var hubConnect = repository.GetAll<HubConnect>(x => x.UserConnectID == currentUser.ID && x.ConnectionID == Context.ConnectionId)
                        .FirstOrDefault();

                    if (hubConnect != null)
                    {
                        repository.Delete(hubConnect, true);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
