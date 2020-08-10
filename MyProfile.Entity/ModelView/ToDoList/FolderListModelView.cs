using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.ToDoList
{
    public class FolderListModelView
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string CssIcon { get; set; }
        public IEnumerable<ToDoFolderList> Lists { get; set; }


        public bool Selected { get; set; } = false;
        public bool IsShow { get; set; } = true;
        public bool IsOwner { get; set; } = true;
        public string UserName { get; set; }
        public string ImageLink { get; set; }
    }

    public class ToDoFolderList
    {
        public int ID { get; set; }
        public int FolderID { get; set; }
        public int PeriodTypeID { get; set; }
        public string Title { get; set; }
        //public VisibleElement VisibleElement { get; set; }
        public bool IsShowInCollective { get; set; }
        public IEnumerable<ToDoListItemModelView> Items { get; set; }

        public bool Selected { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsShow { get; set; } = true;
        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public string ImageLink { get; set; }
    }

    public class ToDoListItemModelView
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public bool IsDone { get; set; }
        public bool IsDeleted { get; set; } = false;

        public bool IsOwner { get; set; }
        public string UserNameCreate { get; set; }
        public string ImageLinkCreate { get; set; }
        public string UserNameDone { get; set; }
        public string ImageLinkDone { get; set; }
    }
}
