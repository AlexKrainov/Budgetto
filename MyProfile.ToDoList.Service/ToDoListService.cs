using MyProfile.Entity.Repository;
using System;

namespace MyProfile.ToDoList.Service
{
    public class ToDoListService
    {
        private IBaseRepository baseRepository;

        public ToDoListService(IBaseRepository baseRepository)
        {
            this.baseRepository = baseRepository;
        }
    }
}
