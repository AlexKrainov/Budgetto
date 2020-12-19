﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        public string Description { get; set; }
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
        public string Description { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyRate { get; set; }
        public int? CurrencyNominal { get; set; }

        public IEnumerable<RecordTag> Tags { get; set; }
    }

    public class RecordTag
    {

        /// <summary>
        /// if ID <= 0 = new , if ID > 0 - old
        /// </summary>
        public int ID { get; set; }
        public string Title { get; set; }
        public string Value { get { return Title; } }
        public DateTime DateCreate { get; set; }
        public bool IsNew { get { return this.ID <= 0; } }
        public bool ToBeEdit { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class RecordsModelView
    {
        public DateTime DateTimeOfPayment { get; set; }
        public bool IsShowInCollection { get; set; }
        public List<RecordModelView> Records { get; set; }
        public List<RecordTag> NewTags { get; set; }
    }
}
