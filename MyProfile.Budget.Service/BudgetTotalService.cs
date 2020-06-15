using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.TotalBudgetView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
	public class BudgetTotalService
	{
		private IBaseRepository repository;
		private BudgetRecordService budgetRecordService;

		public BudgetTotalService(IBaseRepository repository)
		{
			this.repository = repository;
			this.budgetRecordService = new BudgetRecordService(repository);
		}

		public Tuple<List<decimal>, List<string>> GetChartTotalByMonth(DateTime from, DateTime to, SectionTypeEnum sectionTypeEnum)
		{
			var budgetRecordsGroup = budgetRecordService
				.GetBudgetRecords(
					from,
					to,
					x => x.DateTimeOfPayment.Month,
					x => x.BudgetSection.SectionTypeID == (int)sectionTypeEnum)
				.ToList();

			List<decimal> datas = new List<decimal>();
			List<string> labels = new List<string>();

			while (to > from)
			{
				labels.Add(from.Month + " " + from.Year);
				datas.Add(budgetRecordsGroup.Where(x => x.Key == from.Month).Sum(y => y.Sum(z => z.Total)));
				from = from.AddMonths(1);
			}

			return new Tuple<List<decimal>, List<string>> (datas, labels);
		}
	}
}
