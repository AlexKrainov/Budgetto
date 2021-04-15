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
        public IEnumerable<ToDoListModelView> Lists { get; set; }


        public bool Selected { get; set; } = false;
        public bool IsShow { get; set; } = true;
        public bool IsOwner { get; set; } = true;
        public string UserName { get; set; }
        public string ImageLink { get; set; }
    }

    public class ToDoListModelView
    {
        public int ID { get; set; }
        public int FolderID { get; set; }
        public string Title { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsShowInCollective { get; set; }
        public IEnumerable<ToDoListItemModelView> Items { get; set; }
        public ToDoListItemModelView EditItem { get; set; } = new ToDoListItemModelView();

        public bool Selected { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsShow { get; set; } = true;
        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public string ImageLink { get; set; }
        public DateTime DateEdit { get; set; }
        public DateTime DateCreate { get; set; }
        public bool IsNewToday { get; set; }
        public bool IsEditToday { get; set; }
        public List<int> _periodTypeIDs { get; set; }
        public List<int> PeriodTypeIDs
        {
            get
            {
                if (_periodTypeIDs == null)
                {
                    _periodTypeIDs = new List<int>();

                    if (IsShow_BudgetMonth)
                    {
                        _periodTypeIDs.Add((int)PeriodTypesEnum.Month);
                    }
                    if (IsShow_BudgetYear)
                    {
                        _periodTypeIDs.Add((int)PeriodTypesEnum.Year);
                    }
                    return _periodTypeIDs;
                }
                else
                {
                    return _periodTypeIDs;
                }

            }
            set
            {
                _periodTypeIDs = value;
            }
        }
        public bool IsShow_BudgetMonth { get; set; }
        public bool IsShow_BudgetYear { get; set; }
        public decimal Percent { get; set; }
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
        public int Order { get; set; }
    }
}
