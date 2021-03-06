using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using System.Linq;

namespace MyProfile.Code.Hubs
{
    public class HubManager
    {
        private BaseRepository repository;

        public HubManager(BaseRepository repository)
        {
            this.repository = repository;
        }

        public int ResetAllHubConnects()
        {
            int countItems = 0;
            try
            {
                var hubConnect = repository.GetAll<HubConnect>()
                      .ToList();

                if (hubConnect != null)
                {
                    countItems = hubConnect.Count;
                    repository.DeleteRange(hubConnect, true);
                }
            }
            catch (System.Exception ex)
            {
                //???
            }
            return countItems;
        }
    }
}
