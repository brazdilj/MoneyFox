namespace MoneyFox.Ui;

using Common.Exceptions;
using CommunityToolkit.Mvvm.Messaging;
using Core.Common.Extensions;
using Core.Common.Interfaces;
using Core.Common.Settings;
using Core.Features._Legacy_.Payments.ClearPayments;
using Core.Features._Legacy_.Payments.CreateRecurringPayments;
using Core.Features.DbBackup;
using Domain;
using Domain.Aggregates.AccountAggregate;
using Domain.Aggregates.RecurringTransactionAggregate;
using Domain.Exceptions;
using Infrastructure.Adapters;
using InversionOfControl;
using MediatR;
using Messages;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Views;
using Views.Setup;

public partial class App
{
    private bool isRunning;

    public App()
    {
        var settingsAdapter = new SettingsAdapter();
        var settingsFacade = new SettingsFacade(settingsAdapter);
        InitializeComponent();
        SetupServices();
        FillResourceDictionary();
        MainPage = settingsFacade.IsSetupCompleted ? GetAppShellPage() : new SetupShell();
    }

    public static Dictionary<string, ResourceDictionary> ResourceDictionary { get; } = new();

    public static Action<IServiceCollection>? AddPlatformServicesAction { get; set; }

    private static IServiceProvider? ServiceProvider { get; set; }

    public static Page GetAppShellPage()
    {
        return DeviceInfo.Current.Idiom == DeviceIdiom.Desktop || DeviceInfo.Current.Idiom == DeviceIdiom.Tablet || DeviceInfo.Current.Idiom == DeviceIdiom.TV
            ? new AppShellDesktop()
            : new AppShell();
    }

    private void FillResourceDictionary()
    {
        foreach (var dictionary in Resources.MergedDictionaries)
        {
            var key = dictionary.Source.OriginalString.Split(';').First().Split('/').Last().Split('.').First();
            ResourceDictionary.Add(key: key, value: dictionary);
        }
    }

    internal static TViewModel GetViewModel<TViewModel>() where TViewModel : BasePageViewModel
    {
        return ServiceProvider?.GetService<TViewModel>() ?? throw new ResolveViewModelException<TViewModel>();
    }

    protected override void OnStart()
    {
        StartupTasksAsync().ConfigureAwait(false);
    }

    protected override void OnResume()
    {
        StartupTasksAsync().ConfigureAwait(false);
    }

    private static void SetupServices()
    {
        var services = new ServiceCollection();
        AddPlatformServicesAction?.Invoke(services);
        new MoneyFoxConfig().Register(services);
        ServiceProvider = services.BuildServiceProvider();
        var appDbContext = ServiceProvider.GetService<IAppDbContext>();
        appDbContext!.MigrateDb();

        var settings = ServiceProvider.GetService<ISettingsFacade>();

        // Migrate RecurringTransaction
        foreach (var recurringPayment in appDbContext.RecurringPayments.Include(rp => rp.Category)
                     .Include(rp => rp.ChargedAccount)
                     .Include(rp => rp.TargetAccount)
                     .Include(rp => rp.RelatedPayments))
        {
            var recurringTransactionId = Guid.NewGuid();
            var amount = recurringPayment.Type == PaymentType.Expense ? -recurringPayment.Amount : recurringPayment.Amount;
            var recurringTransaction = RecurringTransaction.Create(
                recurringTransactionId: recurringTransactionId,
                chargedAccount: recurringPayment.ChargedAccount.Id,
                targetAccount: recurringPayment.TargetAccount?.Id,
                amount: new(amount: amount, currencyAlphaIsoCode: settings!.DefaultCurrency),
                categoryId: recurringPayment.Category?.Id,
                startDate: recurringPayment.StartDate.ToDateOnly(),
                endDate: recurringPayment.EndDate.HasValue ? DateOnly.FromDateTime(recurringPayment.EndDate.Value) : null,
                recurrence: recurringPayment.Recurrence.ToRecurrence(),
                note: recurringPayment.Note,
                isLastDayOfMonth: recurringPayment.IsLastDayOfMonth,
                isTransfer: recurringPayment.Type == PaymentType.Transfer);

            foreach (var payment in recurringPayment.RelatedPayments)
            {
                payment.AddRecurringTransactionId(recurringTransactionId);
            }

            appDbContext.Add(recurringTransaction);
        }

        appDbContext.SaveChangesAsync().Wait();
    }

    private async Task StartupTasksAsync()
    {
        // Don't execute this again when already running
        if (isRunning)
        {
            return;
        }

        if (ServiceProvider == null)
        {
            return;
        }

        isRunning = true;
        var settingsFacade = ServiceProvider.GetService<ISettingsFacade>() ?? throw new ResolveDependencyException<ISettingsFacade>();
        var mediator = ServiceProvider.GetService<IMediator>() ?? throw new ResolveDependencyException<IMediator>();
        try
        {
            if (settingsFacade is { IsBackupAutoUploadEnabled: true, IsLoggedInToBackupService: true })
            {
                var backupService = ServiceProvider.GetService<IBackupService>() ?? throw new ResolveDependencyException<IBackupService>();
                await backupService.RestoreBackupAsync();
                WeakReferenceMessenger.Default.Send(new BackupRestoredMessage());
            }
        }
        catch (NetworkConnectionException)
        {
            Log.Information("Backup wasn't able to restore on startup - app is offline");
        }
        catch (Exception ex)
        {
            Log.Error(exception: ex, messageTemplate: "Failed to restore backup on startup");
        }

        try
        {
            await mediator.Send(new ClearPaymentsCommand());
            await mediator.Send(new CreateRecurringPaymentsCommand());
            settingsFacade.LastExecutionTimeStampSyncBackup = DateTime.Now;
        }
        catch (Exception ex)
        {
            Log.Error(exception: ex, messageTemplate: "Startup tasks failed");
        }
        finally
        {
            isRunning = false;
        }
    }
}
