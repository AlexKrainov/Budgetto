using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Template.Service
{
	using Template = MyProfile.Entity.Model.Template;
	public class TemplateService
	{
		private IBaseRepository repository;

		public TemplateService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public async Task<TemplateViewModel> GetTemplateByID(Expression<Func<Template, bool>> predicate)
		{
			TemplateViewModel templateViewModel = new TemplateViewModel();

			if (predicate != null)
			{
				templateViewModel = await repository.GetAll<Template>(predicate)
						.Select(x => new TemplateViewModel
						{
							ID = x.ID,
							DateCreate = x.DateCreate,
							DateEdit = x.DateEdit,
							IsCountCollectiveBudget = x.IsCountCollectiveBudget,
							//LastSeenDateTime = x.CodeName,
							MaxRowInAPage = x.MaxRowInAPage,
							Name = x.Name,
							PeriodName = x.PeriodType.Name,
							PeriodTypeID = x.PeriodTypeID,
							Columns = x.TemplateColumns
								.Select(y => new Column
								{
									ID = y.ID,
									Name = y.Name,
									IsShow = y.IsShow,
									Formula = JsonConvert.DeserializeObject<List<FormulaItem>>(y.Formula) ?? new List<FormulaItem>(),
									Order = y.Order,
									TemplateColumnType = (TemplateColumnType)y.ColumnTypeID,
									TotalAction = (FooterActionType)y.FooterActionTypeID,
									PlaceAfterCommon = y.PlaceAfterCommon ?? 0,
									Format = y.Format,
									TemplateBudgetSections = y.TemplateBudgetSections
										.Select(z => new TemplateAreaType
										{
											ID = z.ID,
											SectionID = z.BudgetSection.ID,
											SectionName = z.BudgetSection.Name,
											SectionCodeName = z.BudgetSection.CodeName
										})
										.ToList()
								})
								.ToList()
						})
						.FirstOrDefaultAsync();
			}

			return templateViewModel;
		}
		public async Task<List<TemplateViewModel>> GetTemplates(Expression<Func<Template, bool>> predicate)
		{
			return await repository.GetAll<Template>(predicate)
						.Select(x => new TemplateViewModel
						{
							ID = x.ID,
							DateCreate = x.DateCreate,
							DateEdit = x.DateEdit,
							IsCountCollectiveBudget = x.IsCountCollectiveBudget,
							//LastSeenDateTime = x.CodeName,
							MaxRowInAPage = x.MaxRowInAPage,
							Name = x.Name,
							PeriodName = x.PeriodType.Name,
							PeriodTypeID = x.PeriodTypeID,
							Columns = x.TemplateColumns
								.Select(y => new Column
								{
									ID = y.ID,
									Name = y.Name,
									IsShow = y.IsShow,
									Formula = JsonConvert.DeserializeObject<List<FormulaItem>>(y.Formula) ?? new List<FormulaItem>(),
									Order = y.Order,
									TemplateColumnType = (TemplateColumnType)y.ColumnTypeID,
									TotalAction = (FooterActionType)y.FooterActionTypeID,
									PlaceAfterCommon = y.PlaceAfterCommon ?? 0,
									Format = y.Format,
									TemplateBudgetSections = y.TemplateBudgetSections
										.Select(z => new TemplateAreaType
										{
											ID = z.ID,
											SectionID = z.BudgetSection.ID,
											SectionName = z.BudgetSection.Name,
											SectionCodeName = z.BudgetSection.CodeName
										})
										.ToList()
								})
								.ToList()
						})
						.ToListAsync();
		}
		public TemplateViewModel SaveTemplate(TemplateViewModel template)
		{
			try
			{
				if (template.ID == 0)//create
				{
					Template templateDB = new Template();
					templateDB.PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
					templateDB.PeriodTypeID = template.PeriodTypeID;
					templateDB.CodeName = "test";
					templateDB.DateCreate = templateDB.DateEdit = DateTime.MinValue == template.DateCreate ? DateTime.Now : template.DateCreate;
					templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
					templateDB.MaxRowInAPage = 30;
					templateDB.Name = template.Name ?? "Template";

					repository.Create(templateDB, true);

					foreach (var column in template.Columns.OrderBy(x => x.Order))
					{
						var templateColumnDB = new TemplateColumn
						{
							IsShow = column.IsShow,
							Name = column.Name,
							Order = column.Order,
							TemplateID = templateDB.ID,
							Formula = JsonConvert.SerializeObject(column.Formula),
							ColumnTypeID = (int)column.TemplateColumnType,
							FooterActionTypeID = (int)column.TotalAction,
							PlaceAfterCommon = column.PlaceAfterCommon,
							Format = column.Format
						};
						repository.Create(templateColumnDB, true);

						foreach (var templateAreaType in column.TemplateBudgetSections)
						{
							repository.Create(new TemplateBudgetSection
							{
								BudgetSectionID = templateAreaType.SectionID,
								TemplateColumnID = templateColumnDB.ID
							}, true);
						}
					}
				}
				else
				{
					var templateBudgetSections = repository.GetAll<TemplateBudgetSection>(x => x.TemplateColumn.TemplateID == template.ID);
					repository.DeleteRange(templateBudgetSections, true);

					var columns = repository.GetAll<TemplateColumn>(x => x.TemplateID == template.ID);
					repository.DeleteRange(columns, true);

					Template templateDB = repository.GetByID<Template>(template.ID);

					templateDB.PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
					templateDB.PeriodTypeID = template.PeriodTypeID;
					templateDB.CodeName = "test";
					templateDB.DateEdit = DateTime.Now;
					templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
					templateDB.MaxRowInAPage = 30;
					templateDB.Name = template.Name ?? "NameTemplate";

					repository.Update(templateDB, true);

					foreach (var column in template.Columns.OrderBy(x => x.Order))
					{
						var templateColumnDB = new TemplateColumn
						{
							IsShow = column.IsShow,
							Name = column.Name,
							Order = column.Order,
							TemplateID = templateDB.ID,
							Formula = JsonConvert.SerializeObject(column.Formula),
							ColumnTypeID = (int)column.TemplateColumnType,
							FooterActionTypeID = (int)column.TotalAction,
							PlaceAfterCommon = column.PlaceAfterCommon,
							Format = column.Format
						};
						repository.Create(templateColumnDB, true);

						foreach (var templateAreaType in column.TemplateBudgetSections)
						{
							repository.Create(new TemplateBudgetSection
							{
								BudgetSectionID = templateAreaType.SectionID,
								TemplateColumnID = templateColumnDB.ID
							}, true);
						}
					}
				}
			}
			catch (Exception ex)
			{
				return null;
			}
			return template;
		}
	}
}
