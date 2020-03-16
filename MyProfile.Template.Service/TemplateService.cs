using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public TemplateViewModel GetTemplateByID(int? templateID)
		{
			TemplateViewModel templateViewModel = new TemplateViewModel();

			if (templateID != null)
			{
				templateViewModel = repository.GetAll<Template>(x => x.ID == templateID)
						.Select(x => new TemplateViewModel
						{
							ID = x.ID,
							DateCreate = x.DateCreate,
							DateEdit = x.DateEdit,
							IsCountCollectiveBudget = x.IsCountCollectiveBudget,
							//LastSeenDateTime = x.CodeName,
							MaxRowInAPage = x.MaxRowInAPage,
							Name = x.Name,
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
						.FirstOrDefault();

				//foreach (var column in templateViewModel.Columns)
				//{
				//	foreach (var templateArea in column.TemplateBudgetSections)
				//	{
				//		column.Formula.Add("sectionID=" + templateArea.SectionID);
				//	}
				//}
			}

			return templateViewModel;
		}

		public TemplateViewModel SaveTemplate(TemplateViewModel template)
		{
			try
			{
				if (template.ID == 0)//create
				{
					Template templateDB = new Template();
					templateDB.PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
					templateDB.PeriodTypeID = 1;
					templateDB.CodeName = "test";
					templateDB.DateCreate = templateDB.DateEdit = DateTime.MinValue == template.DateCreate ? DateTime.Now : template.DateCreate;
					templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
					templateDB.MaxRowInAPage = 30;
					templateDB.Name = template.Name ?? "Nalo";

					repository.Create(templateDB, true);

					foreach (var column in template.Columns)
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
					templateDB.PeriodTypeID = 1;
					templateDB.CodeName = "test";
					templateDB.DateEdit = DateTime.Now;
					templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
					templateDB.MaxRowInAPage = 30;
					templateDB.Name = template.Name ?? "NameTemplate";

					repository.Update(templateDB, true);

					foreach (var column in template.Columns)
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
