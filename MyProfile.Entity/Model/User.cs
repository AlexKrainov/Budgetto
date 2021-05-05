using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NumberUser { get; set; }
        [Required]
        public string Name { get; set; }
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public bool IsConfirmEmail { get; set; }
        [Required]
        [MaxLength(44)]
        public string HashPassword { get; set; }
        [Required]
        [MaxLength(44)]
        public string SaltPassword { get; set; }
        public string ImageLink { get; set; }
        [Required]
        public DateTime DateCreate { get; set; }
        public DateTime? DateDelete { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// we have checkbox (IsAgreeToCollectiveBudget) and CollectiveBudgetID (can be null)
        /// 1) User click the ckeckbox
        /// 2) Show input with search email
        /// 3) If we found the email and user has checkbox and CollectiveBudgetID, we write this CollectiveBudgetID to user from (1)
        /// 4) If we don't find create CollectiveBudget
        /// 
        /// </summary>
        public bool IsAllowCollectiveBudget { get; set; }
        [ForeignKey("UserType")]
        public int UserTypeID { get; set; }
        [ForeignKey("Payment")]
        public long PaymentID { get; set; }
        [ForeignKey("Currency")]
        public int CurrencyID { get; set; }
        [ForeignKey("Resource")]
        public long? ResourceID { get; set; }
        [ForeignKey("OlsonTZ")]
        public int? OlsonTZID { get; set; }

        public virtual CollectiveBudgetUser CollectiveBudgetUser { get; set; }
        public virtual UserSettings UserSettings { get; set; }
        public virtual UserConnect UserConnect { get; set; }
        public virtual UserType UserType { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual OlsonTZID OlsonTZ { get; set; }

        public virtual IEnumerable<BudgetArea> BudgetAreas { get; set; }
        public virtual IEnumerable<Record> BudgetRecords { get; set; }
        //public virtual IEnumerable<BudgetSection> BudgetSections { get; set; }
        public virtual IEnumerable<Template> Templates { get; set; }
        public virtual IEnumerable<Chart> Charts { get; set; }
        public virtual IEnumerable<MailLog> MailLogs { get; set; }
        public virtual IEnumerable<CollectiveBudgetRequest> CollectiveBudgetRequests { get; set; }
        public virtual IEnumerable<CollectiveBudgetRequestOwner> CollectiveBudgetRequestOwners { get; set; }
        public virtual IEnumerable<ChatUser> ChatUsers { get; set; }
        public virtual IEnumerable<Reminder> Reminders { get; set; }
        public virtual IEnumerable<Account> Accounts { get; set; }
        public virtual IEnumerable<ToDoListFolder> ToDoListFolders { get; set; }
        public virtual IEnumerable<UserSummary> UserSummaries { get; set; }
        public virtual IEnumerable<UserSubScription> UserSubScriptions { get; set; }
        public virtual IEnumerable<UserEntityCounter> UserEntityCounters { get; set; }

        public User()
        {
            this.BudgetAreas = new HashSet<BudgetArea>();
            this.BudgetRecords = new HashSet<Record>();
            //this.BudgetSections = new HashSet<BudgetSection>();
            this.Templates = new HashSet<Template>();
            this.Charts = new HashSet<Chart>();
            this.MailLogs = new HashSet<MailLog>();
            this.CollectiveBudgetRequests = new HashSet<CollectiveBudgetRequest>();
            this.CollectiveBudgetRequestOwners = new HashSet<CollectiveBudgetRequestOwner>();
            this.ChatUsers = new HashSet<ChatUser>();
            this.Reminders = new HashSet<Reminder>();
            this.ToDoListFolders = new HashSet<ToDoListFolder>();
            this.Accounts = new HashSet<Account>();
            this.UserSummaries = new HashSet<UserSummary>();
            this.UserSubScriptions = new HashSet<UserSubScription>();
            this.UserEntityCounters = new HashSet<UserEntityCounter>();
        }
    }
}
