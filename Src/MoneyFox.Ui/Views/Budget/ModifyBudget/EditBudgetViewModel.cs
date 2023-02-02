namespace MoneyFox.Ui.Views.Budget.ModifyBudget;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MediatR;
using Messages;
using MoneyFox.Core.Common.Extensions;
using MoneyFox.Core.Common.Interfaces;
using MoneyFox.Core.Features.BudgetDeletion;
using MoneyFox.Core.Features.BudgetUpdate;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.Queries.BudgetEntryLoading;
using MoneyFox.Domain.Aggregates.BudgetAggregate;
using MoneyFox.Ui.Resources.Strings;

internal sealed class EditBudgetViewModel : ModifyBudgetViewModel
{
    private readonly IDialogService dialogService;
    private readonly INavigationService navigationService;
    private readonly ISender sender;

    private bool isFirstLoad = true;

    public EditBudgetViewModel(ISender sender, INavigationService navigationService, IDialogService dialogService) : base(navigationService: navigationService)
    {
        this.sender = sender;
        this.navigationService = navigationService;
        this.dialogService = dialogService;
    }

    public BudgetId Id { get; private set; }

    public AsyncRelayCommand<int> InitializeCommand => new(InitializeAsync);

    public AsyncRelayCommand DeleteBudgetCommand => new(DeleteBudgetAsync);

    private async Task InitializeAsync(int budgetId)
    {
        if (isFirstLoad is false)
        {
            return;
        }

        var query = new LoadBudgetEntry.Query(budgetId: budgetId);
        var budgetData = await sender.Send(query);
        Id = budgetData.Id;
        Name = budgetData.Name;
        SpendingLimit = budgetData.SpendingLimit;
        TimeRange = budgetData.TimeRange;
        SelectedCategories.Clear();
        SelectedCategories.AddRange(budgetData.Categories.Select(bc => new BudgetCategoryViewModel(categoryId: bc.Id, name: bc.Name)));
        isFirstLoad = false;
    }

    private async Task DeleteBudgetAsync()
    {
        if (await dialogService.ShowConfirmMessageAsync(title: Translations.DeleteTitle, message: Translations.DeleteBudgetConfirmationMessage))
        {
            var command = new DeleteBudget.Command(budgetId: Id);
            _ = await sender.Send(command);
            _ = Messenger.Send(new ReloadMessage());
            await navigationService.GoBackFromModalAsync();
        }
    }

    protected override async Task SaveBudgetAsync()
    {
        var command = new UpdateBudget.Command(
            budgetId: Id,
            name: Name,
            spendingLimit: SpendingLimit,
            budgetTimeRange: TimeRange,
            categories: SelectedCategories.Select(sc => sc.CategoryId).ToList());

        _ = await sender.Send(command);
        _ = Messenger.Send(new BudgetsChangedMessage());
        await navigationService.GoBackFromModalAsync();
    }
}
