using MyProfile.Entity.ModelView.Currency;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProfile.Entity.ModelView
{
    public class BudgetRecordModelView
    {

        public long ID { get; set; }
        public decimal Money { get; set; }
        public string RawData { get; set; }
        public IEnumerable<RecordTag> Tags { get; set; }
        public string Description { get; set; }
        public DateTime DateTimeOfPayment { get; set; }
        public DateTime? DateTimeCreate { get; set; }
        public DateTime? DateTimeEdit { get; set; }
        public DateTime? DateTimeDelete { get; set; }
        /// <summary>
        /// Consider when count or not
        /// </summary>
        public bool IsConsider { get; set; }

        public long SectionID { get; set; }
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
        public long? AccountID { get; set; }
        public decimal Cashback { get; set; }
        public AccountModelView Account { get; set; }
        public BudgetSectionModelView Section { get; set; }
        public CurrencyLightModelView Currency { get; set; }
    }

    /// <summary>
    /// For save from client
    /// </summary>
    public class RecordModelView
    {
        public long ID { get; set; }
        public string Tag { get; set; }
        public decimal Money { get; set; }
        public long SectionID { get; set; }
        public string SectionName { get; set; }
        public bool IsSaved { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime? DateTimeOfPayment { get; set; }
        public string Description { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyRate { get; set; }
        public int? CurrencyNominal { get; set; }
        public long AccountID { get; set; }
        public AccountModelView Account { get; set; }

        public IEnumerable<RecordTag> Tags { get; set; }

        public bool isAnyError { get; set; }
        public string Error { get; set; }
    }

    public class RecordTag
    {

        /// <summary>
        /// if ID <= 0 = new , if ID > 0 - old
        /// </summary>
        public long ID { get; set; }
        public string Title { get; set; }
        public string Value { get { return Title; } }
        public DateTime DateCreate { get; set; }
        public bool IsNew { get { return this.ID <= 0; } }
        public bool IsShow { get; set; } = true;
        public bool ToBeEdit { get; set; }
        public bool IsDeleted { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        //public IOrderedEnumerable<TagSectionModelView> Sections { get; set; }
        //public IQueryable<TagSectionModelView> SectionIDs { get; set; }
    }

    public class RecordsModelView
    {
        public DateTime DateTimeOfPayment { get; set; }
        public bool IsShowInCollection { get; set; }
        public List<RecordModelView> Records { get; set; }
        public List<RecordTag> NewTags { get; set; }
    }
    public class TagSectionModelView
    {
        public long ID { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public bool IsShow { get; set; } = true;
    }
    public class AccountModelView
    {
        public int AccountType { get; set; }
        public string BankImage { get; set; }
        public string Name { get; set; }
        public string AccountIcon { get; set; }
        public string CurrencyIcon { get; set; }
        public long? CardID { get; set; }
        public string CardName { get; set; }
        public string CardLogo { get; set; }
        public long? ID { get; set; }

        public decimal? NewBalance { get; set; }
        public decimal? OldBalance { get; set; }
    }

    public class HistoryRecordModelView
    {
        public DateTime GroupDate { get; set; }
        public IEnumerable<BudgetRecordModelView> Records { get; set; }
    }
}
