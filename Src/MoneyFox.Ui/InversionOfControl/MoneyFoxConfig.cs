namespace MoneyFox.Ui.InversionOfControl;

using Common.Services;
using Infrastructure.Adapters;
using Mapping;
using Microsoft.Identity.Client;
using MoneyFox.Core.Common.Interfaces;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.InversionOfControl;
using MoneyFox.Infrastructure.InversionOfControl;
using ViewModels.About;
using ViewModels.Accounts;
using ViewModels.Budget;
using ViewModels.Categories;
using ViewModels.Dashboard;
using ViewModels.DataBackup;
using ViewModels.Dialogs;
using ViewModels.OverflowMenu;
using ViewModels.Payments;
using ViewModels.Settings;
using ViewModels.SetupAssistant;
using ViewModels.Statistics;

public sealed class MoneyFoxConfig
{
    private const string MsalApplicationId = "00a3e4cd-b4b0-4730-be62-5fcf90a94a1d";

    public void Register(ServiceCollection serviceCollection)
    {
        RegisterServices(serviceCollection);
        RegisterViewModels(serviceCollection);
        RegisterAdapters(serviceCollection);

        // TODO: use this here again
        //RegisterIdentityClient(serviceCollection);
        serviceCollection.AddSingleton(_ => AutoMapperFactory.Create());
        new CoreConfig().Register(serviceCollection);
        InfrastructureConfig.Register(serviceCollection);
    }

    private static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDialogService, DialogService>();
        serviceCollection.AddTransient<INavigationService, NavigationService>();
        serviceCollection.AddTransient<IToastService, ToastService>();
    }

    private static void RegisterViewModels(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<AboutViewModel>();
        serviceCollection.AddTransient<AccountListViewModel>();
        serviceCollection.AddTransient<AddAccountViewModel>();
        serviceCollection.AddTransient<EditAccountViewModel>();
        serviceCollection.AddTransient<AddCategoryViewModel>();
        serviceCollection.AddTransient<CategoryListViewModel>();
        serviceCollection.AddTransient<EditCategoryViewModel>();
        serviceCollection.AddTransient<SelectCategoryViewModel>();
        serviceCollection.AddTransient<DashboardViewModel>();
        serviceCollection.AddTransient<BackupViewModel>();
        serviceCollection.AddTransient<OverflowMenuViewModel>();
        serviceCollection.AddTransient<AddPaymentViewModel>();
        serviceCollection.AddTransient<EditPaymentViewModel>();
        serviceCollection.AddTransient<PaymentListViewModel>();
        serviceCollection.AddTransient<SettingsViewModel>();
        serviceCollection.AddTransient<CategoryIntroductionViewModel>();
        serviceCollection.AddTransient<SetupCompletionViewModel>();
        serviceCollection.AddTransient<WelcomeViewModel>();
        serviceCollection.AddTransient<PaymentForCategoryListViewModel>();
        serviceCollection.AddTransient<StatisticAccountMonthlyCashFlowViewModel>();
        serviceCollection.AddTransient<StatisticCashFlowViewModel>();
        serviceCollection.AddTransient<StatisticCategoryProgressionViewModel>();
        serviceCollection.AddTransient<StatisticCategorySpreadingViewModel>();
        serviceCollection.AddTransient<StatisticCategorySummaryViewModel>();
        serviceCollection.AddTransient<StatisticSelectorViewModel>();
        serviceCollection.AddTransient<SelectDateRangeDialogViewModel>();
        serviceCollection.AddTransient<SelectFilterDialogViewModel>();
        serviceCollection.AddTransient<AddBudgetViewModel>();
        serviceCollection.AddTransient<EditBudgetViewModel>();
        serviceCollection.AddTransient<BudgetListViewModel>();
    }

    private static void RegisterAdapters(ServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IBrowserAdapter, BrowserAdapter>();
        serviceCollection.AddTransient<IConnectivityAdapter, ConnectivityAdapter>();
        serviceCollection.AddTransient<IEmailAdapter, EmailAdapter>();
        serviceCollection.AddTransient<ISettingsAdapter, SettingsAdapter>();
    }

    private static void RegisterIdentityClient(ServiceCollection serviceCollection)
    {
        var publicClientApplication = PublicClientApplicationBuilder.Create(MsalApplicationId)
            .WithRedirectUri($"msal{MsalApplicationId}://auth")
            .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
            .Build();

        serviceCollection.AddSingleton(publicClientApplication);
    }
}
