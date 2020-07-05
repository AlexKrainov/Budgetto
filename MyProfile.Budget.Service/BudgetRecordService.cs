using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
	public class BudgetRecordService
	{
		private IBaseRepository repository;
		private CollectionUserService collectionUserService;

		public BudgetRecordService(IBaseRepository repository)
		{
			this.repository = repository;
			this.collectionUserService = new CollectionUserService(repository);
		}

		public async Task<RecordModelView> GetByID(int id)
		{
			return await repository.GetAll<BudgetRecord>(x => x.ID == id && x.UserID == UserInfo.UserID)
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
						CurrencyID = record.CurrencyID,
						CurrencyNominal = record.CurrencyNominal,
						CurrencyRate = record.CurrencyRate
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
					UserID = UserInfo.UserID,
					Total = budgetRecord.Money,
					RawData = budgetRecord.RawData,
					CurrencyID = budgetRecord.CurrencyID,
					CurrencyRate = budgetRecord.CurrencyRate,
					CurrencyNominal = budgetRecord.CurrencyNominal ?? 1,
				}, true);
			}
			catch (Exception ex)
			{
				return false;
			}

			return true;
		}


		public IList<IGrouping<int, TmpBudgetRecord>> GetBudgetRecordsGroup(
			DateTime from,
			DateTime to,
			Func<TmpBudgetRecord, int> groupBy,
			Expression<Func<BudgetRecord, bool>> expression = null)
		{
			return _getBudgetRecords(from, to, expression)
				.Select(x => new TmpBudgetRecord
				{
					Total = x.Total,
					DateTimeOfPayment = x.DateTimeOfPayment,
					SectionID = x.BudgetSectionID,
					SectionName = x.BudgetSection.Name,
					SectionTypeID = x.BudgetSection.SectionTypeID,
					AreaID = x.BudgetSection.BudgetArea.ID,
					AreaName = x.BudgetSection.BudgetArea.Name,
					CollectionSectionIDs = x.BudgetSection.CollectiveSections.Select(u => u.ChildSectionID ?? 0).ToList(),
				})
			  .GroupBy(groupBy)
			  .ToList();
		}

		public IList<IGrouping<DateTime, TmpBudgetRecord>> GetBudgetRecordsGroupByDate(
			DateTime from,
			DateTime to,
			Expression<Func<BudgetRecord, bool>> expression = null)
		{
			return _getBudgetRecords(from, to, expression)
				.Select(x => new TmpBudgetRecord
				{
					Total = x.Total,
					DateTimeOfPayment = x.DateTimeOfPayment,
					SectionID = x.BudgetSectionID,
					SectionName = x.BudgetSection.Name,
					AreaID = x.BudgetSection.BudgetArea.ID,
					AreaName = x.BudgetSection.BudgetArea.Name,
					CollectionSectionIDs = x.BudgetSection.CollectiveSections.Select(u => u.ChildSectionID ?? 0).ToList(),
				})
			  .GroupBy(x => x.DateTimeOfPayment.Date)
			  .ToList();
		}

		private IQueryable<BudgetRecord> _getBudgetRecords(
			DateTime from,
			DateTime to,
			Expression<Func<BudgetRecord, bool>> expression = null)
		{
			Guid currentUserID = UserInfo.Current.ID;
			List<Guid> allCollectiveUserIDs = collectionUserService.GetAllCollectiveUserIDs();
			var predicate = PredicateBuilder.True<BudgetRecord>();

			predicate = predicate.And(x => allCollectiveUserIDs.Contains(x.UserID)
				  && from <= x.DateTimeOfPayment && to >= x.DateTimeOfPayment
				  && x.IsDeleted == false
				  && (x.UserID != currentUserID ? x.IsHideForCollection == false : true));

			if (expression != null)
			{
				predicate = predicate.And(expression);
			}
			return repository.GetAll(predicate);
		}


		public async Task<IList<BudgetRecordModelView>> GetBudgetRecordsByFilter(CalendarFilterModels filter)
		{
			var expression = await getExpressionByCalendarFilter(filter);

			return await repository
			  .GetAll(expression)
			  .Select(x => new BudgetRecordModelView
			  {
				  ID = x.ID,
				  DateTimeCreate = x.DateTimeCreate,
				  DateTimeEdit = x.DateTimeEdit,
				  Description = x.Description,
				  IsConsider = x.IsHide,
				  RawData = x.RawData,
				  Money = x.Total,
				  DateTimeOfPayment = x.DateTimeOfPayment,
				  SectionID = x.BudgetSectionID,
				  SectionName = x.BudgetSection.Name,
				  AreaID = x.BudgetSection.BudgetArea.ID,
				  AreaName = x.BudgetSection.BudgetArea.Name
			  })
			  .OrderByDescending(x => x.DateTimeOfPayment.Date)
			  .ToListAsync();
		}


		public async Task<decimal> GetTotalSpendsForLimitByFilter(CalendarFilterModels filter)
		{
			var expression = await getExpressionByCalendarFilter(filter);

			return await repository
			  .GetAll(expression)
			  .SumAsync(x => x.Total);
		}

		private async Task<Expression<Func<BudgetRecord, bool>>> getExpressionByCalendarFilter(CalendarFilterModels filter)
		{
			Guid currentUserID = UserInfo.Current.ID;
			var expression = PredicateBuilder.True<BudgetRecord>();

			if (filter.IsConsiderCollection)
			{
				List<Guid> allCollectiveUserIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();
				expression = expression.And(x => allCollectiveUserIDs.Contains(x.UserID));
			}
			else
			{
				expression = expression.And(x => x.UserID == currentUserID);
			}

			expression = expression.And(x => filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
				  && filter.Sections.Contains(x.BudgetSectionID)
				  && (x.UserID != currentUserID ? x.IsHideForCollection == false : true)
				  && x.IsDeleted == false);
			return expression;
		}

	}
}
