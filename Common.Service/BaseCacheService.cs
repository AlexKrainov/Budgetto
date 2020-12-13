using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Service
{
    public class BaseCacheService
    {
        private IMemoryCache memoryCache;
        private const string _prefix_all = "All_";
        private const string _prefix_one = "One_";
        public BaseCacheService(
            IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        public List<T> GetList<T>()
        {
            return memoryCache.Get<List<T>>(GetKeyList<T>());
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Удаляет из кэша все объекты модели
        /// Также удаляет, если сохранено ModelView такие как:
        /// TripModelView
        /// 
        /// </summary>
        public void RemoveAllList()
        {
            var z = typeof(MyProfile_DBContext);
            foreach (var item in z.GetProperties())
            {
                Type t = Type.GetType(item.Name);
                memoryCache.Remove(_prefix_all + item.Name);
            }

           // memoryCache.Remove(_prefix_all + typeof(TripModelView).Name);

        }

        public void RemoveList<T>()
        {
            memoryCache.Remove(GetKeyList<T>());
        }

        public void Set<T>(T item)
        {
            throw new NotImplementedException();
        }

        public void SetList<T>(List<T> items)
        {
            memoryCache.Set<List<T>>(GetKeyList<T>(), items);
        }


        private string GetKeyList<T>()
        {
            return _prefix_all + typeof(T).Name;
        }
        private string GetKey<T>()
        {
            return _prefix_one + typeof(T).Name;
        }
    }
}
