using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class MyProfile_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChartTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 24, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    IsUsing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveBudgets",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgets", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 24, nullable: false),
                    CodeName = table.Column<string>(maxLength: 3, nullable: false),
                    SpecificCulture = table.Column<string>(maxLength: 16, nullable: false),
                    Icon = table.Column<string>(maxLength: 1, nullable: false),
                    CanBeUser = table.Column<bool>(nullable: false),
                    CodeName_CBR = table.Column<string>(maxLength: 8, nullable: true),
                    CodeNumber_CBR = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MailTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PeriodTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: false),
                    IsUsing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VisibleElements",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsShow_BudgetMonth = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShow_BudgetYear = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowInCollective = table.Column<bool>(nullable: false, defaultValue: false),
                    IsShowOnDashboards = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisibleElements", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    IsConfirmEmail = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    ImageLink = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateDelete = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsAllowCollectiveBudget = table.Column<bool>(nullable: false),
                    UserTypeID = table.Column<int>(nullable: false, defaultValue: 1),
                    CurrencyID = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_Currencies_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_UserTypeID",
                        column: x => x.UserTypeID,
                        principalTable: "UserTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetAreas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    IsShowOnSite = table.Column<bool>(nullable: false, defaultValue: true),
                    Description = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 64, nullable: true),
                    IsShowInCollective = table.Column<bool>(nullable: false, defaultValue: true),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAreas", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetAreas_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Charts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    LastDateEdit = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    ChartTypeID = table.Column<int>(nullable: false),
                    VisibleElementID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Charts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Charts_ChartTypes_ChartTypeID",
                        column: x => x.ChartTypeID,
                        principalTable: "ChartTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Charts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Charts_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveBudgetRequestOwners",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgetRequestOwners", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequestOwners_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveBudgetUsers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(maxLength: 16, nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateUpdate = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    CollectiveBudgetID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgetUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetUsers_CollectiveBudgets_CollectiveBudgetID",
                        column: x => x.CollectiveBudgetID,
                        principalTable: "CollectiveBudgets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetUsers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ExpectationMoney = table.Column<decimal>(type: "Money", nullable: true),
                    DateStart = table.Column<DateTime>(nullable: true),
                    DateEnd = table.Column<DateTime>(nullable: true),
                    IsFinished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    VisibleElementID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Goals_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Goals_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Limits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LimitMoney = table.Column<decimal>(type: "Money", nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    VisibleElementID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Limits_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Limits_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Limits_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MailLogs",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 64, nullable: false),
                    SentDateTime = table.Column<DateTime>(nullable: false),
                    CameDateTime = table.Column<DateTime>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    IsSuccessful = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    MailTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MailLogs_MailTypes_MailTypeID",
                        column: x => x.MailTypeID,
                        principalTable: "MailTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MailLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypeViews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsShow = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionTypeViews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionTypeViews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CurrentPeriod = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    LastSeenDate = table.Column<DateTime>(nullable: true),
                    DateDelete = table.Column<DateTime>(nullable: true),
                    IsCountCollectiveBudget = table.Column<bool>(nullable: false),
                    MaxRowInAPage = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Templates_PeriodTypes_PeriodTypeID",
                        column: x => x.PeriodTypeID,
                        principalTable: "PeriodTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Templates_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IP = table.Column<string>(maxLength: 64, nullable: true),
                    City = table.Column<string>(maxLength: 32, nullable: true),
                    Country = table.Column<string>(maxLength: 32, nullable: true),
                    Location = table.Column<string>(maxLength: 64, nullable: true),
                    PostCode = table.Column<string>(maxLength: 16, nullable: true),
                    BrowerName = table.Column<string>(maxLength: 32, nullable: true),
                    BrowserVersion = table.Column<string>(maxLength: 16, nullable: true),
                    OS_Name = table.Column<string>(maxLength: 32, nullable: true),
                    Os_Version = table.Column<string>(maxLength: 16, nullable: true),
                    ScreenSize = table.Column<string>(maxLength: 16, nullable: true),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    ActionCodeName = table.Column<string>(maxLength: 16, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    SessionID = table.Column<string>(maxLength: 32, nullable: true),
                    ObjectID = table.Column<string>(maxLength: 40, nullable: true),
                    IsUserVisible = table.Column<bool>(nullable: false),
                    IsPhone = table.Column<bool>(nullable: false),
                    IsTablet = table.Column<bool>(nullable: false),
                    ParentUserLogID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLogs_UserLogs_ParentUserLogID",
                        column: x => x.ParentUserLogID,
                        principalTable: "UserLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserLogs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    BudgetPages_WithCollective = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_EarningWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_SpendingWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_InvestingWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_LimitWidgets = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_GoalWidgets = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_BigCharts = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_EarningWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_SpendingWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_InvestingWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_LimitWidgets = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_GoalWidgets = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_BigCharts = table.Column<bool>(nullable: false, defaultValue: true),
                    LimitPage_Show_IsFinished = table.Column<bool>(nullable: false, defaultValue: true),
                    LimitPage_IsShow_Collective = table.Column<bool>(nullable: false),
                    GoalPage_IsShow_Finished = table.Column<bool>(nullable: false),
                    GoalPage_IsShow_Collective = table.Column<bool>(nullable: false),
                    WebSiteTheme_CodeName = table.Column<string>(nullable: true, defaultValue: "light")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_ID",
                        column: x => x.ID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 64, nullable: true),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true),
                    IsShowOnSite = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowInCollective = table.Column<bool>(nullable: false, defaultValue: true),
                    UserID = table.Column<Guid>(nullable: true),
                    BudgetAreaID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetSections_BudgetAreas_BudgetAreaID",
                        column: x => x.BudgetAreaID,
                        principalTable: "BudgetAreas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetSections_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetSections_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChartFields",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true),
                    ChartID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartFields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChartFields_Charts_ChartID",
                        column: x => x.ChartID,
                        principalTable: "Charts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveBudgetRequests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(maxLength: 16, nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateUpdate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    CollectiveBudgetRequestOwnerID = table.Column<int>(nullable: false),
                    CollectiveBudgetID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveBudgetRequests", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequests_CollectiveBudgets_CollectiveBudgetID",
                        column: x => x.CollectiveBudgetID,
                        principalTable: "CollectiveBudgets",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequests_CollectiveBudgetRequestOwners_CollectiveBudgetRequestOwnerID",
                        column: x => x.CollectiveBudgetRequestOwnerID,
                        principalTable: "CollectiveBudgetRequestOwners",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectiveBudgetRequests_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GoalRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: true),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    GoalID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GoalRecords_Goals_GoalID",
                        column: x => x.GoalID,
                        principalTable: "Goals",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalRecords_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemplateColumns",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Formula = table.Column<string>(nullable: false),
                    ColumnTypeID = table.Column<int>(nullable: false),
                    Format = table.Column<string>(nullable: true),
                    FooterActionTypeID = table.Column<int>(nullable: true),
                    PlaceAfterCommon = table.Column<int>(nullable: true),
                    TemplateID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateColumns", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TemplateColumns_Templates_TemplateID",
                        column: x => x.TemplateID,
                        principalTable: "Templates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    RawData = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: false),
                    DateTimeCreate = table.Column<DateTime>(nullable: false),
                    DateTimeEdit = table.Column<DateTime>(nullable: false),
                    DateTimeDelete = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsHide = table.Column<bool>(nullable: false),
                    IsShowForCollection = table.Column<bool>(nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    CurrencyNominal = table.Column<int>(nullable: false, defaultValue: 1),
                    UserID = table.Column<Guid>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_Currencies_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CollectiveSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SectionID = table.Column<int>(nullable: true),
                    ChildSectionID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectiveSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CollectiveSections_BudgetSections_ChildSectionID",
                        column: x => x.ChildSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CollectiveSections_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SectionGroupLimits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LimitID = table.Column<int>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionGroupLimits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionGroupLimits_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionGroupLimits_Limits_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionGroupCharts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChartFieldID = table.Column<int>(nullable: false),
                    BudgetSectionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SectionGroupCharts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SectionGroupCharts_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SectionGroupCharts_ChartFields_ChartFieldID",
                        column: x => x.ChartFieldID,
                        principalTable: "ChartFields",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateBudgetSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BudgetSectionID = table.Column<int>(nullable: false),
                    TemplateColumnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateBudgetSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TemplateBudgetSections_BudgetSections_BudgetSectionID",
                        column: x => x.BudgetSectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateBudgetSections_TemplateColumns_TemplateColumnID",
                        column: x => x.TemplateColumnID,
                        principalTable: "TemplateColumns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAreas_UserID",
                table: "BudgetAreas",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_BudgetSectionID",
                table: "BudgetRecords",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_CurrencyID",
                table: "BudgetRecords",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_DateTimeOfPayment",
                table: "BudgetRecords",
                column: "DateTimeOfPayment");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_UserID",
                table: "BudgetRecords",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_BudgetAreaID",
                table: "BudgetSections",
                column: "BudgetAreaID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_SectionTypeID",
                table: "BudgetSections",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetSections_UserID",
                table: "BudgetSections",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChartFields_ChartID",
                table: "ChartFields",
                column: "ChartID");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_ChartTypeID",
                table: "Charts",
                column: "ChartTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_UserID",
                table: "Charts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Charts_VisibleElementID",
                table: "Charts",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequestOwners_UserID",
                table: "CollectiveBudgetRequestOwners",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequests_CollectiveBudgetID",
                table: "CollectiveBudgetRequests",
                column: "CollectiveBudgetID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequests_CollectiveBudgetRequestOwnerID",
                table: "CollectiveBudgetRequests",
                column: "CollectiveBudgetRequestOwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetRequests_UserID",
                table: "CollectiveBudgetRequests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetUsers_CollectiveBudgetID",
                table: "CollectiveBudgetUsers",
                column: "CollectiveBudgetID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveBudgetUsers_UserID",
                table: "CollectiveBudgetUsers",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveSections_ChildSectionID",
                table: "CollectiveSections",
                column: "ChildSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_CollectiveSections_SectionID",
                table: "CollectiveSections",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_GoalRecords_GoalID",
                table: "GoalRecords",
                column: "GoalID");

            migrationBuilder.CreateIndex(
                name: "IX_GoalRecords_UserID",
                table: "GoalRecords",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserID",
                table: "Goals",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_VisibleElementID",
                table: "Goals",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_PeriodTypeID",
                table: "Limits",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_UserID",
                table: "Limits",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_VisibleElementID",
                table: "Limits",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_MailLogs_MailTypeID",
                table: "MailLogs",
                column: "MailTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_MailLogs_UserID",
                table: "MailLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupCharts_BudgetSectionID",
                table: "SectionGroupCharts",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupCharts_ChartFieldID",
                table: "SectionGroupCharts",
                column: "ChartFieldID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_BudgetSectionID",
                table: "SectionGroupLimits",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionGroupLimits_LimitID",
                table: "SectionGroupLimits",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_PeriodTypeID",
                table: "SectionTypeViews",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_SectionTypeID",
                table: "SectionTypeViews",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SectionTypeViews_UserID",
                table: "SectionTypeViews",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateBudgetSections_BudgetSectionID",
                table: "TemplateBudgetSections",
                column: "BudgetSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateBudgetSections_TemplateColumnID",
                table: "TemplateBudgetSections",
                column: "TemplateColumnID");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateColumns_TemplateID",
                table: "TemplateColumns",
                column: "TemplateID");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_PeriodTypeID",
                table: "Templates",
                column: "PeriodTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Templates_UserID",
                table: "Templates",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_ParentUserLogID",
                table: "UserLogs",
                column: "ParentUserLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserID",
                table: "UserLogs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrencyID",
                table: "Users",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeID",
                table: "Users",
                column: "UserTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetRecords");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetRequests");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetUsers");

            migrationBuilder.DropTable(
                name: "CollectiveSections");

            migrationBuilder.DropTable(
                name: "GoalRecords");

            migrationBuilder.DropTable(
                name: "MailLogs");

            migrationBuilder.DropTable(
                name: "SectionGroupCharts");

            migrationBuilder.DropTable(
                name: "SectionGroupLimits");

            migrationBuilder.DropTable(
                name: "SectionTypeViews");

            migrationBuilder.DropTable(
                name: "TemplateBudgetSections");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetRequestOwners");

            migrationBuilder.DropTable(
                name: "CollectiveBudgets");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "MailTypes");

            migrationBuilder.DropTable(
                name: "ChartFields");

            migrationBuilder.DropTable(
                name: "Limits");

            migrationBuilder.DropTable(
                name: "BudgetSections");

            migrationBuilder.DropTable(
                name: "TemplateColumns");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "BudgetAreas");

            migrationBuilder.DropTable(
                name: "SectionTypes");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "ChartTypes");

            migrationBuilder.DropTable(
                name: "VisibleElements");

            migrationBuilder.DropTable(
                name: "PeriodTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "UserTypes");
        }
    }
}
