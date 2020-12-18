using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProfile.Entity.ModelView
{
    public class BudgetRecordModelView
    {

        public int ID { get; set; }
        public decimal Money { get; set; }
        public string RawData { get; set; }
        public IEnumerable<RecordTag> Tags { get; set; }
        public string FrontDescription
        {
            get
            {
                string __description = _description;
                foreach (var tag in Tags.ToList())
                {
                    __description = __description.Replace("{{" + tag.ID + "}}",
$@"<tag title='{tag.Title}' class='tagify__tag tagify--noAnim' id='{tag.ID}'>
    <div>
        <span class='tagify__tag-text'>{tag.Title}</span>
    </div>
</tag>");
                }

                return __description;
            }
        }
        public string _description;
        public string Description
        {
            get
            {
                return _description;
                //string __description = _description;

                //foreach (var tag in Tags)
                //{
                //    __description = __description.Replace("{{" + tag.ID + "}}", JsonConvert.SerializeObject(tag));
                //}

                //return __description;
            }
            set
            {
                _description = value;
            }
        }
        public DateTime DateTimeOfPayment { get; set; }
        public DateTime? DateTimeCreate { get; set; }
        public DateTime? DateTimeEdit { get; set; }
        public DateTime? DateTimeDelete { get; set; }
        /// <summary>
        /// Consider when count or not
        /// </summary>
        public bool IsConsider { get; set; }

        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public int AreaID { get; set; }
        public string AreaName { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyRate { get; set; }
        public int? CurrencyNominal { get; set; }
        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public string ImageLink { get; set; }


        public bool IsSaved { get; set; } = true;
        public bool IsCorrect { get; set; } = true;
        public bool IsShowForFilter { get; set; } = true;
        public bool IsDeleted { get; set; }
        public bool IsShowForCollection { get; set; }
        public string Tag { get { return RawData; } }

        public string CurrencySpecificCulture { get; set; }
        public string CurrencyCodeName { get; set; }
        public string CssIcon { get; set; }
        public string CssBackground { get; set; }
        public string CssColor { get; set; }
        public int? SectionTypeID { get; set; }
    }

    /// <summary>
    /// For save from client
    /// </summary>
    public class RecordModelView
    {
        public int ID { get; set; }
        public string Tag { get; set; }
        public decimal Money { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public bool IsSaved { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? DateTimeOfPayment { get; set; }
        private string _description;
        public string Description
        {
            get
            {
                string __description = _description;

                for (int i = 0; i < Tags.Count; i++)
                {
                    __description = __description.Replace("{{" + Tags[i].ID + "}}", JsonConvert.SerializeObject(Tags[i]));
                }

                return __description;
            }
            set
            {
                _description = value;
            }
        }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyRate { get; set; }
        public int? CurrencyNominal { get; set; }

        public List<RecordTag> Tags { get; set; }
    }

    public class RecordTag
    {

        /// <summary>
        /// if ID <= 0 = new , if ID > 0 - old
        /// </summary>
        public int ID { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public DateTime DateCreate { get; set; }
        public string Image { get; set; }
        public string IconCss { get; set; }
        public bool IsNew { get { return this.ID <= 0; } }
        public bool ToBeEdit { get; set; }
    }

    public class RecordsModelView
    {
        public DateTime DateTimeOfPayment { get; set; }
        public bool IsShowInCollection { get; set; }
        public List<RecordModelView> Records { get; set; }
    }
}
