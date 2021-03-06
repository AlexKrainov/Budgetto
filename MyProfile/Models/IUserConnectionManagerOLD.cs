using System.Collections.Generic;

namespace MyProfile.Models
{
    public interface IUserConnectionManagerOLD
    {
        void KeepUserConnection(string userId, string connectionId);
        void RemoveUserConnection(string connectionId);
        List<string> GetUserConnections(string userId);
    }
}
