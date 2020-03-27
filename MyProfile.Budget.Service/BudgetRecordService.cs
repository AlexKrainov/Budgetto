using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProfile.Budget.Service
{
	public class BudgetRecordService
	{
		private IBaseRepository repository;

		public BudgetRecordService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public bool Create(BudgetRecordModelView budgetRecord)
		{
			try
			{
				repository.Create<BudgetRecord>(new BudgetRecord
				{
					BudgetSectionID = budgetRecord.SectionID,
					DateTimeCreate = DateTime.Now.ToUniversalTime(),
					DateTimeEdit = DateTime.Now.ToUniversalTime(),
					DateTimeOfPayment = budgetRecord.DateTimeOfPayment,
					Description = budgetRecord.Description,
					IsConsider = budgetRecord.IsConsider,
					PersonID = UserInfo.PersonID,
					Total = budgetRecord.Money,
				}, true);
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

		public bool Create(RecordsModelView budgetRecord)
		{
			foreach (var record in budgetRecord.Records.Where(x => x.IsShow))
			{
				record.IsSaved = Create(new BudgetRecordModelView
				{
					SectionID = record.SectionID,
					Money = record.Money,
					//Description = record.SectionID
				});
			}

			return true;
		}
	}
}
