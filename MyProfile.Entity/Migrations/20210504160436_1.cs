using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProfile.Entity.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    IsVisible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChartTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 24, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    IsUsing = table.Column<bool>(nullable: false),
                    IsBig = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 512, nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChatType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ID);
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
                    CodeName = table.Column<string>(maxLength: 8, nullable: false),
                    SpecificCulture = table.Column<string>(maxLength: 16, nullable: false),
                    Icon = table.Column<string>(maxLength: 8, nullable: false),
                    CanBeUser = table.Column<bool>(nullable: false),
                    CodeName_CBR = table.Column<string>(maxLength: 8, nullable: true),
                    CodeNumber_CBR = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyRateHistories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    Nominal = table.Column<int>(nullable: false),
                    NumCode = table.Column<string>(maxLength: 8, nullable: false),
                    CharCode = table.Column<string>(maxLength: 8, nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    CodeName_CBR = table.Column<string>(maxLength: 16, nullable: false),
                    CurrencyID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRateHistories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HelpMenus",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 32, nullable: false),
                    Icon = table.Column<string>(maxLength: 32, nullable: false),
                    IsVisible = table.Column<bool>(nullable: false, defaultValue: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpMenus", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IPSettings",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IP = table.Column<string>(maxLength: 64, nullable: true),
                    IsBlock = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    LastVisit = table.Column<DateTime>(nullable: false),
                    Counter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IPSettings", x => x.ID);
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
                name: "PaymentSystems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 24, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    Logo = table.Column<string>(maxLength: 512, nullable: true),
                    IsVisible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSystems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTariffs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: true),
                    CodeName = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTariffs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PeriodTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false),
                    IsUsing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    TryCounter = table.Column<int>(nullable: false),
                    LimitCounter = table.Column<int>(nullable: false),
                    Percent = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    Extension = table.Column<string>(maxLength: 24, nullable: false),
                    BodyBase64 = table.Column<string>(maxLength: 8, nullable: true),
                    FolderName = table.Column<string>(maxLength: 32, nullable: false),
                    SrcPath = table.Column<string>(maxLength: 256, nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerTasks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    FirstStart = table.Column<DateTime>(nullable: true),
                    LastStart = table.Column<DateTime>(nullable: true),
                    TaskStatus = table.Column<string>(maxLength: 16, nullable: false),
                    TaskType = table.Column<string>(maxLength: 64, nullable: false),
                    CronExpression = table.Column<string>(maxLength: 16, nullable: true),
                    CronComment = table.Column<string>(maxLength: 64, nullable: true),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerTasks", x => x.ID);
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
                name: "SiteSettings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmailsForFeedback = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptionCategories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 128, nullable: false),
                    CodeName = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptionCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 132, nullable: false),
                    Image = table.Column<string>(maxLength: 256, nullable: true),
                    IconCss = table.Column<string>(maxLength: 32, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TelegramAccountStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 16, nullable: true),
                    CodeName = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccountStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TimeZones",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WindowsTimezoneID = table.Column<string>(maxLength: 64, nullable: true),
                    WindowsDisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    UTCOffsetHours = table.Column<decimal>(nullable: false),
                    UTCOffsetMinutes = table.Column<int>(nullable: false),
                    IsDST = table.Column<bool>(nullable: false),
                    Abreviatura = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeZones", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CodeName = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VisibleElements",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsShow_BudgetMonth = table.Column<bool>(nullable: false),
                    IsShow_BudgetYear = table.Column<bool>(nullable: false),
                    IsShowInCollective = table.Column<bool>(nullable: false, defaultValue: false),
                    IsShowOnDashboards = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisibleElements", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 64, nullable: false),
                    CodeName = table.Column<string>(maxLength: 16, nullable: false),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Icon = table.Column<string>(maxLength: 64, nullable: true),
                    IsVisible = table.Column<bool>(nullable: false),
                    IsPaymentSystem = table.Column<bool>(nullable: false),
                    BankTypeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountTypes_BankTypes_BankTypeID",
                        column: x => x.BankTypeID,
                        principalTable: "BankTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(maxLength: 512, nullable: true),
                    LogoCircle = table.Column<string>(maxLength: 512, nullable: true),
                    LogoRectangle = table.Column<string>(maxLength: 512, nullable: true),
                    URL = table.Column<string>(maxLength: 512, nullable: true),
                    Tels = table.Column<string>(maxLength: 512, nullable: true),
                    Raiting = table.Column<int>(nullable: false),
                    Licence = table.Column<string>(maxLength: 256, nullable: true),
                    Region = table.Column<string>(maxLength: 128, nullable: true),
                    bankiruID = table.Column<string>(maxLength: 16, nullable: true),
                    BrandColor = table.Column<string>(maxLength: 16, nullable: true),
                    BankTypeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Banks_BankTypes_BankTypeID",
                        column: x => x.BankTypeID,
                        principalTable: "BankTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(maxLength: 32, nullable: false),
                    Topic = table.Column<string>(maxLength: 128, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    MoodID = table.Column<int>(nullable: false),
                    ChatID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Feedbacks_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCounters",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CanBeCount = table.Column<int>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    PaymentTariffID = table.Column<int>(nullable: false),
                    EntityTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCounters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentCounters_EntityTypes_EntityTypeID",
                        column: x => x.EntityTypeID,
                        principalTable: "EntityTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentCounters_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateFrom = table.Column<DateTime>(nullable: false),
                    DateTo = table.Column<DateTime>(nullable: false),
                    LastDatePayment = table.Column<DateTime>(nullable: true),
                    IsPaid = table.Column<bool>(nullable: false),
                    PaymentTariffID = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulerTaskLogs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    IsError = table.Column<bool>(nullable: false),
                    ChangedItems = table.Column<int>(nullable: false),
                    TaskID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulerTaskLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SchedulerTaskLogs_SchedulerTasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "SchedulerTasks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Site = table.Column<string>(maxLength: 512, nullable: true),
                    LogoBig = table.Column<string>(maxLength: 512, nullable: true),
                    LogoSmall = table.Column<string>(maxLength: 512, nullable: true),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    SubScriptionCategoryID = table.Column<int>(nullable: false),
                    ParentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubScriptions_SubScriptions_ParentID",
                        column: x => x.ParentID,
                        principalTable: "SubScriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubScriptions_SubScriptionCategories_SubScriptionCategoryID",
                        column: x => x.SubScriptionCategoryID,
                        principalTable: "SubScriptionCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OlsonTZIDs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    TimeZoneID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OlsonTZIDs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OlsonTZIDs_TimeZones_TimeZoneID",
                        column: x => x.TimeZoneID,
                        principalTable: "TimeZones",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    CodeName = table.Column<string>(maxLength: 64, nullable: false),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsChart = table.Column<bool>(nullable: false),
                    VisibleElementID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Summaries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Summaries_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    SmallLogo = table.Column<string>(maxLength: 256, nullable: true),
                    BigLogo = table.Column<string>(maxLength: 256, nullable: true),
                    IsInterest = table.Column<bool>(nullable: false),
                    Interest = table.Column<decimal>(nullable: false),
                    ServiceCostTo = table.Column<decimal>(nullable: false),
                    ServiceCostFrom = table.Column<decimal>(nullable: false),
                    Cashback = table.Column<decimal>(nullable: false),
                    IsCashback = table.Column<bool>(nullable: false),
                    IsCustomDesign = table.Column<bool>(nullable: false),
                    Rate = table.Column<decimal>(nullable: false),
                    IsRateTo = table.Column<bool>(nullable: true),
                    CreditLimit = table.Column<decimal>(nullable: false),
                    GracePeriod = table.Column<int>(nullable: false),
                    Raiting = table.Column<int>(nullable: false),
                    bonuses = table.Column<string>(nullable: true),
                    paymentSystems = table.Column<string>(nullable: true),
                    bankiruCardID = table.Column<int>(nullable: false),
                    SearchString = table.Column<string>(nullable: true),
                    bankName = table.Column<string>(nullable: true),
                    AccountTypeID = table.Column<int>(nullable: false),
                    BankID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cards_AccountTypes_AccountTypeID",
                        column: x => x.AccountTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cards_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    DateClickToPay = table.Column<DateTime>(nullable: true),
                    DateFinisthToPay = table.Column<DateTime>(nullable: true),
                    DateFrom = table.Column<DateTime>(nullable: true),
                    DateTo = table.Column<DateTime>(nullable: true),
                    PaymentID = table.Column<long>(nullable: false),
                    PaymentTariffID = table.Column<int>(nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentHistories_PaymentTariffs_PaymentTariffID",
                        column: x => x.PaymentTariffID,
                        principalTable: "PaymentTariffs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptionOptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EditDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    IsPersonally = table.Column<bool>(nullable: false),
                    IsFamaly = table.Column<bool>(nullable: false),
                    IsStudent = table.Column<bool>(nullable: false),
                    IsBoth = table.Column<bool>(nullable: false),
                    _raiting = table.Column<decimal>(nullable: true),
                    SubScriptionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptionOptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubScriptionOptions_SubScriptions_SubScriptionID",
                        column: x => x.SubScriptionID,
                        principalTable: "SubScriptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    NumberUser = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    IsConfirmEmail = table.Column<bool>(nullable: false),
                    HashPassword = table.Column<string>(maxLength: 44, nullable: false),
                    SaltPassword = table.Column<string>(maxLength: 44, nullable: false),
                    ImageLink = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateDelete = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsAllowCollectiveBudget = table.Column<bool>(nullable: false),
                    UserTypeID = table.Column<int>(nullable: false, defaultValue: 1),
                    PaymentID = table.Column<long>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false, defaultValue: 1),
                    ResourceID = table.Column<long>(nullable: true),
                    OlsonTZID = table.Column<int>(nullable: true)
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
                        name: "FK_Users_OlsonTZIDs_OlsonTZID",
                        column: x => x.OlsonTZID,
                        principalTable: "OlsonTZIDs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Payments_PaymentID",
                        column: x => x.PaymentID,
                        principalTable: "Payments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Resources_ResourceID",
                        column: x => x.ResourceID,
                        principalTable: "Resources",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_UserTypes_UserTypeID",
                        column: x => x.UserTypeID,
                        principalTable: "UserTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardPaymentSystems",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardID = table.Column<long>(nullable: false),
                    PaymentSystemID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardPaymentSystems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CardPaymentSystems_Cards_CardID",
                        column: x => x.CardID,
                        principalTable: "Cards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardPaymentSystems_PaymentSystems_PaymentSystemID",
                        column: x => x.PaymentSystemID,
                        principalTable: "PaymentSystems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodeHistories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InputPromoCode = table.Column<string>(maxLength: 512, nullable: false),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    IsApplied = table.Column<bool>(nullable: false),
                    PaymentHistoryID = table.Column<Guid>(nullable: false),
                    PromoCodeID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodeHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PromoCodeHistories_PaymentHistories_PaymentHistoryID",
                        column: x => x.PaymentHistoryID,
                        principalTable: "PaymentHistories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodeHistories_PromoCodes_PromoCodeID",
                        column: x => x.PromoCodeID,
                        principalTable: "PromoCodes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubScriptionPricings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PricingPeriodTypeID = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "Money", nullable: false),
                    SubScriptionOptionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScriptionPricings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SubScriptionPricings_SubScriptionOptions_SubScriptionOptionID",
                        column: x => x.SubScriptionOptionID,
                        principalTable: "SubScriptionOptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Balance = table.Column<decimal>(type: "Money", nullable: false),
                    CachbackBalance = table.Column<decimal>(type: "Money", nullable: false),
                    Description = table.Column<string>(maxLength: 264, nullable: true),
                    CachbackForAllPercent = table.Column<decimal>(nullable: true),
                    IsCachback = table.Column<bool>(nullable: false),
                    IsCachbackMoney = table.Column<bool>(nullable: false),
                    IsOverdraft = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsCountTheBalance = table.Column<bool>(nullable: false),
                    IsCountBalanceInMainAccount = table.Column<bool>(nullable: false),
                    IsHide = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    ResetCachbackDate = table.Column<DateTime>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    AccountTypeID = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: true),
                    BankID = table.Column<int>(nullable: true),
                    PaymentSystemID = table.Column<int>(nullable: true),
                    ParentAccountID = table.Column<long>(nullable: true),
                    CardID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeID",
                        column: x => x.AccountTypeID,
                        principalTable: "AccountTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Accounts_Banks_BankID",
                        column: x => x.BankID,
                        principalTable: "Banks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Cards_CardID",
                        column: x => x.CardID,
                        principalTable: "Cards",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Currencies_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_ParentAccountID",
                        column: x => x.ParentAccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_PaymentSystems_PaymentSystemID",
                        column: x => x.PaymentSystemID,
                        principalTable: "PaymentSystems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
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
                    IsShowInCollective = table.Column<bool>(nullable: false, defaultValue: true),
                    IsCreatedByConstructor = table.Column<bool>(nullable: false),
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
                    VisibleElementID = table.Column<long>(nullable: false)
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
                    IsCreatedByConstructor = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    VisibleElementID = table.Column<long>(nullable: false)
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
                name: "HelpArticles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 512, nullable: false),
                    IsVisible = table.Column<bool>(nullable: false, defaultValue: true),
                    KeyWords = table.Column<string>(nullable: false),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    Link = table.Column<string>(maxLength: 512, nullable: false),
                    RelatedArticleIDs = table.Column<string>(maxLength: 256, nullable: true),
                    OwnerID = table.Column<Guid>(nullable: true),
                    HelpMenuID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpArticles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HelpArticles_HelpMenus_HelpMenuID",
                        column: x => x.HelpMenuID,
                        principalTable: "HelpMenus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpArticles_Users_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Limits",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    LimitMoney = table.Column<decimal>(type: "Money", nullable: false),
                    IsFinished = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsCreatedByConstructor = table.Column<bool>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    PeriodTypeID = table.Column<int>(nullable: false),
                    VisibleElementID = table.Column<long>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: true, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Limits", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Limits_Currencies_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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
                    Code = table.Column<long>(nullable: false),
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
                name: "Reminders",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DateReminder = table.Column<DateTime>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    IsRepeat = table.Column<bool>(nullable: false),
                    RepeatEvery = table.Column<string>(maxLength: 16, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CssIcon = table.Column<string>(maxLength: 32, nullable: true),
                    OffSetClient = table.Column<int>(nullable: false),
                    TimeZoneClient = table.Column<string>(maxLength: 64, nullable: true),
                    UserID = table.Column<Guid>(nullable: false),
                    OlsonTZID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reminders_OlsonTZIDs_OlsonTZID",
                        column: x => x.OlsonTZID,
                        principalTable: "OlsonTZIDs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reminders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionTypeViews",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
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
                    ID = table.Column<long>(nullable: false)
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
                    IsCreatedByConstructor = table.Column<bool>(nullable: false),
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
                name: "ToDoListFolders",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 32, nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoListFolders", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoListFolders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserConnects",
                columns: table => new
                {
                    UserID = table.Column<Guid>(nullable: false),
                    TelegramLogin = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnects", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_UserConnects_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEntityCounters",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedCount = table.Column<int>(nullable: false),
                    LastChanges = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    EntityTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntityCounters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserEntityCounters_EntityTypes_EntityTypeID",
                        column: x => x.EntityTypeID,
                        principalTable: "EntityTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEntityCounters_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    IP = table.Column<string>(maxLength: 64, nullable: true),
                    City = table.Column<string>(maxLength: 32, nullable: true),
                    Country = table.Column<string>(maxLength: 32, nullable: true),
                    Location = table.Column<string>(maxLength: 64, nullable: true),
                    BrowerName = table.Column<string>(maxLength: 32, nullable: true),
                    BrowserVersion = table.Column<string>(maxLength: 16, nullable: true),
                    OS_Name = table.Column<string>(maxLength: 32, nullable: true),
                    Os_Version = table.Column<string>(maxLength: 16, nullable: true),
                    ScreenSize = table.Column<string>(maxLength: 16, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    IsPhone = table.Column<bool>(nullable: false),
                    IsTablet = table.Column<bool>(nullable: false),
                    IsLandingPage = table.Column<bool>(nullable: false),
                    EnterDate = table.Column<DateTime>(nullable: false),
                    LogOutDate = table.Column<DateTime>(nullable: true),
                    Referrer = table.Column<string>(nullable: true),
                    ContinentCode = table.Column<string>(maxLength: 32, nullable: true),
                    ContinentName = table.Column<string>(maxLength: 32, nullable: true),
                    Index = table.Column<string>(maxLength: 32, nullable: true),
                    Info = table.Column<string>(nullable: true),
                    ProviderInfo = table.Column<string>(nullable: true),
                    Threat = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserID",
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
                    Month_Summary = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_Accounts = table.Column<bool>(nullable: false, defaultValue: true),
                    Month_ToDoLists = table.Column<bool>(nullable: false),
                    Year_EarningWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_SpendingWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_InvestingWidget = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_LimitWidgets = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_GoalWidgets = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_BigCharts = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_Summary = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_Accounts = table.Column<bool>(nullable: false, defaultValue: true),
                    Year_ToDoLists = table.Column<bool>(nullable: false),
                    LimitPage_Show_IsFinished = table.Column<bool>(nullable: false, defaultValue: true),
                    LimitPage_IsShow_Collective = table.Column<bool>(nullable: false),
                    GoalPage_IsShow_Finished = table.Column<bool>(nullable: false),
                    GoalPage_IsShow_Collective = table.Column<bool>(nullable: false),
                    WebSiteTheme = table.Column<string>(nullable: true, defaultValue: "light"),
                    Mail_News = table.Column<bool>(nullable: false, defaultValue: true),
                    Mail_Reminders = table.Column<bool>(nullable: false, defaultValue: true),
                    CanUseAlgorithm = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowHints = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowFirstEnterHint = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowConstructor = table.Column<bool>(nullable: false),
                    IsShowCookie = table.Column<bool>(nullable: false, defaultValue: true)
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
                name: "UserSummaries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsChart = table.Column<bool>(nullable: false),
                    VisibleElementID = table.Column<long>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false),
                    SummaryID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummaries", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSummaries_Summaries_SummaryID",
                        column: x => x.SummaryID,
                        principalTable: "Summaries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummaries_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummaries_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 132, nullable: false),
                    Image = table.Column<string>(maxLength: 256, nullable: true),
                    IconCss = table.Column<string>(maxLength: 32, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TagID = table.Column<long>(nullable: true),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserTags_Tags_TagID",
                        column: x => x.TagID,
                        principalTable: "Tags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTags_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSubScriptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Price = table.Column<decimal>(type: "Money", nullable: false),
                    PricePerMonth = table.Column<decimal>(type: "Money", nullable: false),
                    IsPause = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SubScriptionPricingID = table.Column<int>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubScriptions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSubScriptions_SubScriptionPricings_SubScriptionPricingID",
                        column: x => x.SubScriptionPricingID,
                        principalTable: "SubScriptionPricings",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSubScriptions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountHistories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionType = table.Column<string>(maxLength: 16, nullable: false),
                    Actions = table.Column<string>(maxLength: 1024, nullable: true),
                    OldAccountStateJson = table.Column<string>(nullable: true),
                    NewAccountStateJson = table.Column<string>(nullable: true),
                    ValueTo = table.Column<decimal>(type: "Money", nullable: true),
                    ValueFrom = table.Column<decimal>(type: "Money", nullable: true),
                    OldBalance = table.Column<decimal>(type: "Money", nullable: true),
                    NewBalance = table.Column<decimal>(type: "Money", nullable: true),
                    CurrencyValue = table.Column<decimal>(type: "Money", nullable: true),
                    CachbackBalance = table.Column<decimal>(type: "Money", nullable: true),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    Comment = table.Column<string>(maxLength: 2048, nullable: true),
                    StateField = table.Column<string>(nullable: true),
                    AccountID = table.Column<long>(nullable: false),
                    AccountIDFrom = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AccountHistories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountHistories_Accounts_AccountIDFrom",
                        column: x => x.AccountIDFrom,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountInfos",
                columns: table => new
                {
                    AccountID = table.Column<long>(nullable: false),
                    InterestRate = table.Column<decimal>(nullable: true),
                    InterestBalance = table.Column<decimal>(nullable: true),
                    LastInterestAccrualDate = table.Column<DateTime>(nullable: true),
                    InterestNextDate = table.Column<DateTime>(nullable: true),
                    CapitalizationTimeListID = table.Column<int>(nullable: false),
                    IsFinishedDeposit = table.Column<bool>(nullable: false),
                    InterestBalanceForEndOfDeposit = table.Column<decimal>(nullable: true),
                    IsCapitalization = table.Column<bool>(nullable: false),
                    CreditLimit = table.Column<decimal>(nullable: true),
                    CreditExpirationDate = table.Column<DateTime>(nullable: true),
                    GracePeriod = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInfos", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_AccountInfos_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetSections",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    CodeName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CssIcon = table.Column<string>(maxLength: 64, nullable: true),
                    CssColor = table.Column<string>(maxLength: 24, nullable: true, defaultValue: "#rgba(24,28,33,0.8)"),
                    CssBackground = table.Column<string>(maxLength: 24, nullable: true, defaultValue: "#eeeeee"),
                    CssBorder = table.Column<string>(maxLength: 24, nullable: true),
                    IsShowOnSite = table.Column<bool>(nullable: false, defaultValue: true),
                    IsShowInCollective = table.Column<bool>(nullable: false, defaultValue: true),
                    IsCreatedByConstructor = table.Column<bool>(nullable: false),
                    IsCashback = table.Column<bool>(nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "ChartFields",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
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
                    ID = table.Column<long>(nullable: false)
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
                name: "HelpArticleUserViews",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateView = table.Column<DateTime>(nullable: false),
                    HelpArticleID = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelpArticleUserViews", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HelpArticleUserViews_HelpArticles_HelpArticleID",
                        column: x => x.HelpArticleID,
                        principalTable: "HelpArticles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HelpArticleUserViews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReminderDates",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateReminder = table.Column<DateTime>(nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    ReminderID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderDates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReminderDates_Reminders_ReminderID",
                        column: x => x.ReminderID,
                        principalTable: "Reminders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateColumns",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    IsShow = table.Column<bool>(nullable: false),
                    Formula = table.Column<string>(nullable: false),
                    ColumnTypeID = table.Column<int>(nullable: false),
                    Format = table.Column<string>(nullable: true),
                    FooterActionTypeID = table.Column<int>(nullable: true),
                    PlaceAfterCommon = table.Column<int>(nullable: true),
                    TemplateID = table.Column<long>(nullable: false)
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
                name: "ToDoLists",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    IsFavorite = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ToDoListFolderID = table.Column<int>(nullable: false),
                    VisibleElementID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoLists", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoLists_ToDoListFolders_ToDoListFolderID",
                        column: x => x.ToDoListFolderID,
                        principalTable: "ToDoListFolders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToDoLists_VisibleElements_VisibleElementID",
                        column: x => x.VisibleElementID,
                        principalTable: "VisibleElements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HubConnects",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateConnect = table.Column<DateTime>(nullable: false),
                    ConnectionID = table.Column<string>(maxLength: 128, nullable: true),
                    UserConnectID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HubConnects", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HubConnects_UserConnects_UserConnectID",
                        column: x => x.UserConnectID,
                        principalTable: "UserConnects",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelegramAccounts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TelegramID = table.Column<long>(nullable: false),
                    Username = table.Column<string>(maxLength: 512, nullable: true),
                    FirstName = table.Column<string>(maxLength: 512, nullable: true),
                    LastName = table.Column<string>(maxLength: 512, nullable: true),
                    Title = table.Column<string>(maxLength: 512, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    LanguageCode = table.Column<string>(maxLength: 16, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    LastDateConnect = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    StatusID = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccounts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TelegramAccounts_TelegramAccountStatuses_StatusID",
                        column: x => x.StatusID,
                        principalTable: "TelegramAccountStatuses",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelegramAccounts_UserConnects_UserID",
                        column: x => x.UserID,
                        principalTable: "UserConnects",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    Where = table.Column<string>(maxLength: 64, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    ErrorText = table.Column<string>(nullable: true),
                    UserSessionID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_UserSessions_UserSessionID",
                        column: x => x.UserSessionID,
                        principalTable: "UserSessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentDateTime = table.Column<DateTime>(nullable: false),
                    ActionCodeName = table.Column<string>(maxLength: 64, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    UserSessionID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserLogs_UserSessions_UserSessionID",
                        column: x => x.UserSessionID,
                        principalTable: "UserSessions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSummarySectionTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserSummaryID = table.Column<int>(nullable: false),
                    SectionTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummarySectionTypes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSummarySectionTypes_SectionTypes_SectionTypeID",
                        column: x => x.SectionTypeID,
                        principalTable: "SectionTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummarySectionTypes_UserSummaries_UserSummaryID",
                        column: x => x.UserSummaryID,
                        principalTable: "UserSummaries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BudgetRecords",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Total = table.Column<decimal>(type: "Money", nullable: false),
                    Cashback = table.Column<decimal>(type: "Money", nullable: false),
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
                    BudgetSectionID = table.Column<long>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: true, defaultValue: 1),
                    AccountID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetRecords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_BudgetRecords_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
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
                    SectionID = table.Column<long>(nullable: true),
                    ChildSectionID = table.Column<long>(nullable: true)
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
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LimitID = table.Column<long>(nullable: false),
                    BudgetSectionID = table.Column<long>(nullable: false)
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
                name: "UserSummarySections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserSummaryID = table.Column<int>(nullable: false),
                    SectionID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSummarySections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserSummarySections_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSummarySections_UserSummaries_UserSummaryID",
                        column: x => x.UserSummaryID,
                        principalTable: "UserSummaries",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SectionGroupCharts",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChartFieldID = table.Column<long>(nullable: false),
                    BudgetSectionID = table.Column<long>(nullable: false)
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
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BudgetSectionID = table.Column<long>(nullable: false),
                    TemplateColumnID = table.Column<long>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ToDoListItems",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    ToDoListID = table.Column<long>(nullable: false),
                    OwnerUserID = table.Column<Guid>(nullable: true),
                    DoneUserID = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoListItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ToDoListItems_Users_DoneUserID",
                        column: x => x.DoneUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToDoListItems_Users_OwnerUserID",
                        column: x => x.OwnerUserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ToDoListItems_ToDoLists_ToDoListID",
                        column: x => x.ToDoListID,
                        principalTable: "ToDoLists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatUsers",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsChatOwner = table.Column<bool>(nullable: false),
                    Left = table.Column<bool>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    DateLeft = table.Column<DateTime>(nullable: true),
                    IsMute = table.Column<bool>(nullable: false),
                    ChatID = table.Column<long>(nullable: false),
                    UserID = table.Column<Guid>(nullable: true),
                    TelegramAccountID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUsers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ChatUsers_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatUsers_TelegramAccounts_TelegramAccountID",
                        column: x => x.TelegramAccountID,
                        principalTable: "TelegramAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatUsers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NotificationTypeID = table.Column<int>(nullable: false),
                    IsReady = table.Column<bool>(nullable: false),
                    IsReadyDateTime = table.Column<DateTime>(nullable: true),
                    IsSentOnSite = table.Column<bool>(nullable: false),
                    IsSentOnTelegram = table.Column<bool>(nullable: false),
                    IsSentOnMail = table.Column<bool>(nullable: false),
                    IsDone = table.Column<bool>(nullable: false),
                    LastChangeDateTime = table.Column<DateTime>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false),
                    ReadDateTime = table.Column<DateTime>(nullable: true),
                    IsSite = table.Column<bool>(nullable: false),
                    IsMail = table.Column<bool>(nullable: false),
                    IsTelegram = table.Column<bool>(nullable: false),
                    Total = table.Column<decimal>(type: "Money", nullable: true),
                    ExpirationDateTime = table.Column<DateTime>(nullable: true),
                    IsRepeat = table.Column<bool>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false),
                    LimitID = table.Column<long>(nullable: true),
                    ReminderDateID = table.Column<long>(nullable: true),
                    TelegramAccountID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notifications_Limits_LimitID",
                        column: x => x.LimitID,
                        principalTable: "Limits",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_ReminderDates_ReminderDateID",
                        column: x => x.ReminderDateID,
                        principalTable: "ReminderDates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_TelegramAccounts_TelegramAccountID",
                        column: x => x.TelegramAccountID,
                        principalTable: "TelegramAccounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserErrorLogs",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ErrorLogID = table.Column<long>(nullable: true),
                    UserLogID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserErrorLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_UserErrorLogs_ErrorLogs_ErrorLogID",
                        column: x => x.ErrorLogID,
                        principalTable: "ErrorLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserErrorLogs_UserLogs_UserLogID",
                        column: x => x.UserLogID,
                        principalTable: "UserLogs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordHistories",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionTypeCode = table.Column<string>(nullable: false),
                    RecordTotal = table.Column<decimal>(type: "Money", nullable: false),
                    RecordCachback = table.Column<decimal>(type: "Money", nullable: false),
                    AccountNewBalance = table.Column<decimal>(type: "Money", nullable: false),
                    AccountNewBalanceCashback = table.Column<decimal>(type: "Money", nullable: false),
                    AccountTotal = table.Column<decimal>(type: "Money", nullable: false),
                    AccountCashback = table.Column<decimal>(type: "Money", nullable: false),
                    RacordCurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    RecordCurrencyNominal = table.Column<int>(nullable: false),
                    AccountCurrencyRate = table.Column<decimal>(type: "Money", nullable: true),
                    AccountCurrencyNominal = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 264, nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateTimeOfPayment = table.Column<DateTime>(nullable: false),
                    RecordID = table.Column<long>(nullable: false),
                    AccountID = table.Column<long>(nullable: true),
                    RecordCurrencyID = table.Column<int>(nullable: true),
                    AccountCurrencyID = table.Column<int>(nullable: true),
                    SectionID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordHistories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecordHistories_Currencies_AccountCurrencyID",
                        column: x => x.AccountCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordHistories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordHistories_Currencies_RecordCurrencyID",
                        column: x => x.RecordCurrencyID,
                        principalTable: "Currencies",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordHistories_BudgetRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "BudgetRecords",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordHistories_BudgetSections_SectionID",
                        column: x => x.SectionID,
                        principalTable: "BudgetSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordTags",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateSet = table.Column<DateTime>(nullable: false),
                    RecordID = table.Column<long>(nullable: false),
                    UserTagID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordTags", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RecordTags_BudgetRecords_RecordID",
                        column: x => x.RecordID,
                        principalTable: "BudgetRecords",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordTags_UserTags_UserTagID",
                        column: x => x.UserTagID,
                        principalTable: "UserTags",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    DateCreate = table.Column<DateTime>(nullable: false),
                    DateEdit = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ChatID = table.Column<long>(nullable: false),
                    ChatUserID = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_ChatUsers_ChatUserID",
                        column: x => x.ChatUserID,
                        principalTable: "ChatUsers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResourceMessages",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ResourceID = table.Column<long>(nullable: false),
                    MessageID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceMessages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ResourceMessages_Messages_MessageID",
                        column: x => x.MessageID,
                        principalTable: "Messages",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceMessages_Resources_ResourceID",
                        column: x => x.ResourceID,
                        principalTable: "Resources",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistories_AccountID",
                table: "AccountHistories",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHistories_AccountIDFrom",
                table: "AccountHistories",
                column: "AccountIDFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeID",
                table: "Accounts",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_BankID",
                table: "Accounts",
                column: "BankID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CardID",
                table: "Accounts",
                column: "CardID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyID",
                table: "Accounts",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountID",
                table: "Accounts",
                column: "ParentAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_PaymentSystemID",
                table: "Accounts",
                column: "PaymentSystemID");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserID",
                table: "Accounts",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountTypes_BankTypeID",
                table: "AccountTypes",
                column: "BankTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_BankTypeID",
                table: "Banks",
                column: "BankTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAreas_UserID",
                table: "BudgetAreas",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRecords_AccountID",
                table: "BudgetRecords",
                column: "AccountID");

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
                name: "IX_CardPaymentSystems_CardID",
                table: "CardPaymentSystems",
                column: "CardID");

            migrationBuilder.CreateIndex(
                name: "IX_CardPaymentSystems_PaymentSystemID",
                table: "CardPaymentSystems",
                column: "PaymentSystemID");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AccountTypeID",
                table: "Cards",
                column: "AccountTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_BankID",
                table: "Cards",
                column: "BankID");

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
                name: "IX_ChatUsers_ChatID",
                table: "ChatUsers",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_TelegramAccountID",
                table: "ChatUsers",
                column: "TelegramAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_UserID",
                table: "ChatUsers",
                column: "UserID");

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
                name: "IX_ErrorLogs_UserSessionID",
                table: "ErrorLogs",
                column: "UserSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ChatID",
                table: "Feedbacks",
                column: "ChatID",
                unique: true);

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
                name: "IX_HelpArticles_HelpMenuID",
                table: "HelpArticles",
                column: "HelpMenuID");

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticles_OwnerID",
                table: "HelpArticles",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticleUserViews_HelpArticleID",
                table: "HelpArticleUserViews",
                column: "HelpArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_HelpArticleUserViews_UserID",
                table: "HelpArticleUserViews",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_HubConnects_UserConnectID",
                table: "HubConnects",
                column: "UserConnectID");

            migrationBuilder.CreateIndex(
                name: "IX_Limits_CurrencyID",
                table: "Limits",
                column: "CurrencyID");

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
                name: "IX_Messages_ChatID",
                table: "Messages",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatUserID",
                table: "Messages",
                column: "ChatUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LimitID",
                table: "Notifications",
                column: "LimitID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReminderDateID",
                table: "Notifications",
                column: "ReminderDateID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TelegramAccountID",
                table: "Notifications",
                column: "TelegramAccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserID",
                table: "Notifications",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OlsonTZIDs_TimeZoneID",
                table: "OlsonTZIDs",
                column: "TimeZoneID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCounters_EntityTypeID",
                table: "PaymentCounters",
                column: "EntityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentCounters_PaymentTariffID",
                table: "PaymentCounters",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentID",
                table: "PaymentHistories",
                column: "PaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentHistories_PaymentTariffID",
                table: "PaymentHistories",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentTariffID",
                table: "Payments",
                column: "PaymentTariffID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeHistories_PaymentHistoryID",
                table: "PromoCodeHistories",
                column: "PaymentHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeHistories_PromoCodeID",
                table: "PromoCodeHistories",
                column: "PromoCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_AccountCurrencyID",
                table: "RecordHistories",
                column: "AccountCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_AccountID",
                table: "RecordHistories",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_RecordCurrencyID",
                table: "RecordHistories",
                column: "RecordCurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_RecordID",
                table: "RecordHistories",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordHistories_SectionID",
                table: "RecordHistories",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordTags_RecordID",
                table: "RecordTags",
                column: "RecordID");

            migrationBuilder.CreateIndex(
                name: "IX_RecordTags_UserTagID",
                table: "RecordTags",
                column: "UserTagID");

            migrationBuilder.CreateIndex(
                name: "IX_ReminderDates_ReminderID",
                table: "ReminderDates",
                column: "ReminderID");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_OlsonTZID",
                table: "Reminders",
                column: "OlsonTZID");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_UserID",
                table: "Reminders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceMessages_MessageID",
                table: "ResourceMessages",
                column: "MessageID");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceMessages_ResourceID",
                table: "ResourceMessages",
                column: "ResourceID");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerTaskLogs_TaskID",
                table: "SchedulerTaskLogs",
                column: "TaskID");

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
                name: "IX_SubScriptionOptions_SubScriptionID",
                table: "SubScriptionOptions",
                column: "SubScriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptionPricings_SubScriptionOptionID",
                table: "SubScriptionPricings",
                column: "SubScriptionOptionID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptions_ParentID",
                table: "SubScriptions",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_SubScriptions_SubScriptionCategoryID",
                table: "SubScriptions",
                column: "SubScriptionCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Summaries_VisibleElementID",
                table: "Summaries",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccounts_StatusID",
                table: "TelegramAccounts",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccounts_UserID",
                table: "TelegramAccounts",
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
                name: "IX_ToDoListFolders_UserID",
                table: "ToDoListFolders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItems_DoneUserID",
                table: "ToDoListItems",
                column: "DoneUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItems_OwnerUserID",
                table: "ToDoListItems",
                column: "OwnerUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoListItems_ToDoListID",
                table: "ToDoListItems",
                column: "ToDoListID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_ToDoListFolderID",
                table: "ToDoLists",
                column: "ToDoListFolderID");

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_VisibleElementID",
                table: "ToDoLists",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntityCounters_EntityTypeID",
                table: "UserEntityCounters",
                column: "EntityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserEntityCounters_UserID",
                table: "UserEntityCounters",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserErrorLogs_ErrorLogID",
                table: "UserErrorLogs",
                column: "ErrorLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserErrorLogs_UserLogID",
                table: "UserErrorLogs",
                column: "UserLogID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogs_UserSessionID",
                table: "UserLogs",
                column: "UserSessionID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrencyID",
                table: "Users",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_OlsonTZID",
                table: "Users",
                column: "OlsonTZID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PaymentID",
                table: "Users",
                column: "PaymentID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ResourceID",
                table: "Users",
                column: "ResourceID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeID",
                table: "Users",
                column: "UserTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserID",
                table: "UserSessions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubScriptions_SubScriptionPricingID",
                table: "UserSubScriptions",
                column: "SubScriptionPricingID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubScriptions_UserID",
                table: "UserSubScriptions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummaries_SummaryID",
                table: "UserSummaries",
                column: "SummaryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummaries_UserID",
                table: "UserSummaries",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummaries_VisibleElementID",
                table: "UserSummaries",
                column: "VisibleElementID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySections_SectionID",
                table: "UserSummarySections",
                column: "SectionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySections_UserSummaryID",
                table: "UserSummarySections",
                column: "UserSummaryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySectionTypes_SectionTypeID",
                table: "UserSummarySectionTypes",
                column: "SectionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserSummarySectionTypes_UserSummaryID",
                table: "UserSummarySectionTypes",
                column: "UserSummaryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_TagID",
                table: "UserTags",
                column: "TagID");

            migrationBuilder.CreateIndex(
                name: "IX_UserTags_UserID",
                table: "UserTags",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountHistories");

            migrationBuilder.DropTable(
                name: "AccountInfos");

            migrationBuilder.DropTable(
                name: "CardPaymentSystems");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetRequests");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetUsers");

            migrationBuilder.DropTable(
                name: "CollectiveSections");

            migrationBuilder.DropTable(
                name: "CurrencyRateHistories");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "GoalRecords");

            migrationBuilder.DropTable(
                name: "HelpArticleUserViews");

            migrationBuilder.DropTable(
                name: "HubConnects");

            migrationBuilder.DropTable(
                name: "IPSettings");

            migrationBuilder.DropTable(
                name: "MailLogs");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PaymentCounters");

            migrationBuilder.DropTable(
                name: "PromoCodeHistories");

            migrationBuilder.DropTable(
                name: "RecordHistories");

            migrationBuilder.DropTable(
                name: "RecordTags");

            migrationBuilder.DropTable(
                name: "ResourceMessages");

            migrationBuilder.DropTable(
                name: "SchedulerTaskLogs");

            migrationBuilder.DropTable(
                name: "SectionGroupCharts");

            migrationBuilder.DropTable(
                name: "SectionGroupLimits");

            migrationBuilder.DropTable(
                name: "SectionTypeViews");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "TemplateBudgetSections");

            migrationBuilder.DropTable(
                name: "ToDoListItems");

            migrationBuilder.DropTable(
                name: "UserEntityCounters");

            migrationBuilder.DropTable(
                name: "UserErrorLogs");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserSubScriptions");

            migrationBuilder.DropTable(
                name: "UserSummarySections");

            migrationBuilder.DropTable(
                name: "UserSummarySectionTypes");

            migrationBuilder.DropTable(
                name: "CollectiveBudgetRequestOwners");

            migrationBuilder.DropTable(
                name: "CollectiveBudgets");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "HelpArticles");

            migrationBuilder.DropTable(
                name: "MailTypes");

            migrationBuilder.DropTable(
                name: "ReminderDates");

            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "BudgetRecords");

            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "SchedulerTasks");

            migrationBuilder.DropTable(
                name: "ChartFields");

            migrationBuilder.DropTable(
                name: "Limits");

            migrationBuilder.DropTable(
                name: "TemplateColumns");

            migrationBuilder.DropTable(
                name: "ToDoLists");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "UserLogs");

            migrationBuilder.DropTable(
                name: "SubScriptionPricings");

            migrationBuilder.DropTable(
                name: "UserSummaries");

            migrationBuilder.DropTable(
                name: "HelpMenus");

            migrationBuilder.DropTable(
                name: "Reminders");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BudgetSections");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "ChatUsers");

            migrationBuilder.DropTable(
                name: "Charts");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "ToDoListFolders");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "SubScriptionOptions");

            migrationBuilder.DropTable(
                name: "Summaries");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "PaymentSystems");

            migrationBuilder.DropTable(
                name: "BudgetAreas");

            migrationBuilder.DropTable(
                name: "SectionTypes");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "TelegramAccounts");

            migrationBuilder.DropTable(
                name: "ChartTypes");

            migrationBuilder.DropTable(
                name: "PeriodTypes");

            migrationBuilder.DropTable(
                name: "SubScriptions");

            migrationBuilder.DropTable(
                name: "VisibleElements");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "TelegramAccountStatuses");

            migrationBuilder.DropTable(
                name: "UserConnects");

            migrationBuilder.DropTable(
                name: "SubScriptionCategories");

            migrationBuilder.DropTable(
                name: "BankTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "OlsonTZIDs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "UserTypes");

            migrationBuilder.DropTable(
                name: "TimeZones");

            migrationBuilder.DropTable(
                name: "PaymentTariffs");
        }
    }
}
