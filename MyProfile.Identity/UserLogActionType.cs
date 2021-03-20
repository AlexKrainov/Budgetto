namespace MyProfile.Identity
{
    public class UserLogActionType
    {
        public const string Login = "Login";
        public const string LoginAfterCode = "LoginAfterCode";
        public const string LoginAfterResetPassword = "LoginAfterResetPassword";
        public const string Login_AutoAuthorization = "Login_AutoAuthorization";
        public const string AutoAuthorization = "AutoAuthorization";
        public const string TryLogin = "TryLogin";
        public const string LimitLogin = "LimitLogin";

        public const string Logout = "Logout";
        public const string Registration = "Registration";
        public const string TryRegistration = "TryRegistration";
        public const string RegistrationSendEmail = "RegistrationSendEmail";
        public const string RecoveryPassword_Step1 = "RecoveryPassword_Step1";
        public const string RecoveryPassword_Step2 = "RecoveryPassword_Step2";
        public const string EnterCode = "EnterCode";
        public const string CheckCode = "CheckCode";
        public const string CheckCodeAfterChangeEmail = "CheckCodeAfterChangeEmail";
        public const string ResendEmail = "ResendEmail";

        public const string Email_ConfirmEmail = "Email_ConfirmEmail";
        public const string Email_ConfirmEmailComplete = "Email_ConfirmEmailComplete";
        public const string Email_LoginConfirmation = "Email_LoginConfirmation";
        public const string Email_RecoveryPassword = "Email_RecoveryPassword";
        public const string Email_CheckCodeOk = "Email_CheckCodeOk";
        public const string Email_CheckCodeNotOk = "Email_CheckCodeNotOk";
        public const string Email_CancelLastEmail = "Email_CancelLastEmail";
        public const string Email_AuthorizationByEmail = "Email_AuthorizationByEmail";
        public const string Email_TryAuthorizationByEmail = "Email_TryAuthorizationByEmail";


        public const string BudgetMonth_Page = "BudgetMonth_Page";
        public const string BudgetYear_Page = "BudgetYear_Page";
        public const string Templates_Page = "Templates_Page";
        public const string TemplateEdit_Page = "TemplateEdit_Page";
        public const string Limit_Page = "Limit_Page";
        public const string Goal_Page = "Goal_Page";
        public const string BigCharts_Page = "BigCharts_Page";
        public const string BigChartEdit_Page = "BigChartEdit_Page";
        public const string TimeLine_Page = "TimeLine_Page";
        public const string ToDoLists_Page = "ToDoLists_Page";
        public const string Section_Page = "Section_Page";
        public const string AccountSetting_Page = "AccountSetting_Page";
        public const string Error404_Page = "Error404_Page";
        public const string Error500_Page = "Error500_Page";
        public const string Feedback_Page = "Feedback_Page";
        public const string HelpCenter_Page = "HelpCenter_Page";
        public const string HelpCenter_Article_Page = "HelpCenter_Article_Page";

        public const string BudgetPage_HideGoal = "BudgetPage_HideGoal";
        public const string BudgetPage_HideLimit = "BudgetPage_HideLimit";
        public const string BudgetPage_HideChirt = "BudgetPage_HideChirt";

        public const string Reminder_Part = "Reminder_Part";
        public const string ToDoListEdit_Part = "ToDoListEdit_Part";

        public const string Limit_Create = "Limit_Create";
        public const string Limit_Edit = "Limit_Edit";
        public const string Limit_Delete = "Limit_Delete";
        public const string Limit_Toggle = "Limit_Toggle";
        public const string Limit_Notification = "Limit_Notification";

        public const string Goal_Create = "Goal_Create";
        public const string Goal_Edit = "Goal_Edit";
        public const string Goal_Delete = "Goal_Delete";

        public const string BigChart_Create = "BigChart_Create";
        public const string BigChart_Edit = "BigChart_Edit";
        public const string BigChart_Delete = "BigChart_Delete";
        public const string BigChart_Recovery = "BigChart_Recovery";
        public const string BigChart_Toggle = "BigChart_Toggle";

        public const string Template_Create = "Template_Create";
        public const string Template_Edit = "Template_Edit";
        public const string Template_Delete = "Template_Delete";
        public const string Template_Recovery = "Template_Recovery";
        public const string Template_ColumnOrder = "Template_ColumnOrder";
        public const string Template_ToggleIsDefault = "Template_ToggleIsDefault";

        public const string Area_Create = "Area_Create";
        public const string Area_Edit = "Area_Edit";
        public const string Area_Delete = "Area_Delete";

        public const string Section_Create = "Section_Create";
        public const string Section_Edit = "Section_Edit";
        public const string Section_Delete = "Section_Delete";

        public const string Record_Create = "Record_Create";
        public const string Record_Edit = "Record_Edit";
        public const string Record_Delete = "Record_Delete";
        public const string Record_Recovery = "Record_Recovery";
        public const string Record_IsNotAvailibleUser = "Record_IsNotAvailibleUser";

        public const string Feedback_Create = "Feedback_Create";

        public const string Reminder_Create = "Reminder_Create";
        public const string Reminder_Edit = "Reminder_Edit";
        public const string Reminder_Delete = "Reminder_Delete";
        public const string Reminder_Notification = "Reminder_Notification";

        public const string ToDoListFolder_Create = "ToDoListFolder_Create";
        public const string ToDoListFolder_Edit = "ToDoListFolder_Edit";
        public const string ToDoListFolder_Delete = "ToDoListFolder_Delete";
        public const string ToDoListList_Create = "ToDoListList_Create";
        public const string ToDoListList_Edit = "ToDoListList_Edit";
        public const string ToDoListList_Delete = "ToDoListList_Delete";
        public const string ToDoListList_Recovery = "ToDoListList_Recovery";

        public const string PaymentHistory_Create = "PaymentHistory_Create";
        public const string PaymentHistory_Update = "PaymentHistory_Update";

        public const string Payment_Update = "Payment_Update";

        public const string User_Edit = "User_Edit";
        public const string User_ChangeEmail = "User_ChangeEmail";
        public const string User_ErrorChangeEmail = "User_ErrorChangeEmail";
        public const string User_EnterHintOff = "User_EnterHintOff";
        public const string User_CookieOff = "User_CookieOff";
        public const string User_AutoAuthorization = "User_AutoAuthorization";
        public const string User_LeaveSite = "User_LeaveSite";
        public const string User_Connection_ChangeStatus = "User_Connection_ChangeStatus";

        public const string Document_CookiePolicy = "Document_CookiePolicy";
        public const string Document_PersonalDataProcessingPolicy = "Document_PersonalDataProcessingPolicy";
        public const string Document_TermsOfUse = "Document_TermsOfUse";

        public const string Constructor_Step1_UserInfo = "Constructor_Step1_UserInfo";
        public const string Constructor_Step2_Sections = "Constructor_Step2_Sections";
        public const string Constructor_Step3_Template = "Constructor_Step3_Template";
        public const string Constructor_Step4_Limits = "Constructor_Step4_Limits";
        public const string Constructor_Step5_Goals = "Constructor_Step5_Goals";
        public const string Constructor_Step6_Finish = "Constructor_Step6_Finish";

        public const string ADMIN_GenerateRecords = "ADMIN_GenerateRecords";
        public const string ADMIN_ClearAccount = "ADMIN_ClearAccount";

        public const string LandingPage_Enter = "LandingPage_Enter";
        public const string LandingPage_MovedToAppBudgetto = "LandingPage_MovedToAppBudgetto";
        public const string LandingPage_MovedToAppBudgetto_HeaderButton = "LandingPage_MovedToAppBudgetto_HeaderButton";
        public const string LandingPage_MovedToAppBudgetto_FirstViewButton = "LandingPage_MovedToAppBudgetto_FirstViewButton";
        public const string LandingPage_MovedToAppBudgetto_SecondViewButton = "LandingPage_MovedToAppBudgetto_SecondViewButton";
        public const string LandingPage_MovedToAppBudgetto_SectionViewButton = "LandingPage_MovedToAppBudgetto_SectionViewButton";
        public const string LandingPage_MovedToAppBudgetto_RecordsViewButton = "LandingPage_MovedToAppBudgetto_RecordsViewButton";
        public const string LandingPage_MovedToAppBudgetto_TableViewButton = "LandingPage_MovedToAppBudgetto_TableViewButton";
        public const string LandingPage_MovedToAppBudgetto_LimitViewButton = "LandingPage_MovedToAppBudgetto_LimitViewButton";
        public const string LandingPage_MovedToAppBudgetto_GoalsViewButton = "LandingPage_MovedToAppBudgetto_GoalsViewButton";
        public const string LandingPage_MovedToAppBudgetto_ChartViewButton = "LandingPage_MovedToAppBudgetto_ChartViewButton";
        public const string LandingPage_MovedToAppBudgetto_ReminderViewButton = "LandingPage_MovedToAppBudgetto_ReminderViewButton";
        public const string LandingPage_MovedToAppBudgetto_OneForAllViewButton = "LandingPage_MovedToAppBudgetto_OneForAllViewButton";
        public const string LandingPage_MovedToAppBudgetto_FreePriceButton = "LandingPage_MovedToAppBudgetto_FreePriceButton";
        public const string LandingPage_MovedToAppBudgetto_OneYearPriceButton = "LandingPage_MovedToAppBudgetto_OneYearPriceButton";
        public const string LandingPage_MovedToAppBudgetto_ThreeYearsPriceButton = "LandingPage_MovedToAppBudgetto_ThreeYearsPriceButton";
        public const string LandingPage_MovedToAppBudgetto_EmailButton = "LandingPage_MovedToAppBudgetto_EmailButton";
        public const string LandingPage_ShowMore = "LandingPage_ShowMore";

        public const string Account_Create = "Account_Create";
        public const string Account_Update = "Account_Update";
        public const string Account_Delete = "Account_Delete";
        public const string Account_Recovery = "Account_Recovery";
        public const string Account_TryToDeleteLastAccount = "Account_TryToDeleteLastAccount";
        public const string Account_TransferMoney= "Account_TransferMoney";

        public const string Summary_Set_WorkHours = "Summary_Edit_WorkHours";
    }
}
