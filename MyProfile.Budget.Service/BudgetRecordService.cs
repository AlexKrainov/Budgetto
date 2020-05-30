using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
	public class BudgetRecordService
	{
		private IBaseRepository repository;

		public BudgetRecordService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public async Task<RecordModelView> GetByID(int id)
		{
			return await repository.GetAll<BudgetRecord>(x => x.ID == id && x.PersonID == UserInfo.PersonID)
				.Select(x => new RecordModelView
				{
					ID = id,
					IsCorrect = true,
					IsSaved = true,
					Money = x.Total,
					SectionID = x.BudgetSectionID,
					SectionName = x.BudgetSection.Name,
					Tag = x.RawData,
					DateTimeOfPayment = x.DateTimeOfPayment,
				})
				.FirstOrDefaultAsync();
		}

		public bool CreateOrUpdate(RecordsModelView budgetRecord)
		{
			budgetRecord.DateTimeOfPayment = new DateTime(budgetRecord.DateTimeOfPayment.Year, budgetRecord.DateTimeOfPayment.Month, budgetRecord.DateTimeOfPayment.Day, 13, 0, 0);
			foreach (var record in budgetRecord.Records.Where(x => x.IsCorrect))
			{
				if (record.ID <= 0)// create
				{
					record.IsSaved = Create(new BudgetRecordModelView
					{
						SectionID = record.SectionID,
						Money = record.Money,
						RawData = record.Tag,
						DateTimeOfPayment = budgetRecord.DateTimeOfPayment,
						Description = record.Description,
						//Description = record.SectionID
					});
				}
				else
				{//edit
					var dbRecord = repository.GetByID<BudgetRecord>(record.ID);

					dbRecord.BudgetSectionID = record.SectionID;
					dbRecord.Total = record.Money;
					dbRecord.RawData = record.Tag;
					dbRecord.DateTimeOfPayment = budgetRecord.DateTimeOfPayment;
					dbRecord.Description = record.Description;

					repository.Update(dbRecord, true);
				}
			}

			return true;
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
					IsHide = budgetRecord.IsConsider,
					PersonID = UserInfo.PersonID,
					Total = budgetRecord.Money,
					RawData = budgetRecord.RawData,
				}, true);
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}

	}
}
