namespace MoneyFox.Ui.Views.Payments.PaymentList;

using System.Collections.ObjectModel;
using Accounts.AccountModification;
using AutoMapper;
using Common.Navigation;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Queries;
using Core.Queries.PaymentsForAccount;
using MediatR;
using PaymentModification;

internal sealed class PaymentListViewModel : NavigableViewModel
{
    private readonly IMapper mapper1;
    private readonly IMediator mediator1;
    private readonly INavigationService navigationService1;
    private ReadOnlyObservableCollection<PaymentDayGroup> paymentDayGroups = null!;
    private AccountViewModel selectedAccount = new();

    public PaymentListViewModel(IMediator mediator, IMapper mapper, INavigationService navigationService)
    {
        mediator1 = mediator;
        mapper1 = mapper;
        navigationService1 = navigationService;
        WeakReferenceMessenger.Default.Register<PaymentListFilterChangedMessage>(
            recipient: this,
            handler: (_, m) => { LoadPayments(m).GetAwaiter().GetResult(); });
    }

    public AccountViewModel SelectedAccount
    {
        get => selectedAccount;

        private set
        {
            selectedAccount = value;
            OnPropertyChanged();
        }
    }

    public ReadOnlyObservableCollection<PaymentDayGroup> PaymentDayGroups
    {
        get => paymentDayGroups;
        private set => SetProperty(field: ref paymentDayGroups, newValue: value);
    }

    public AsyncRelayCommand GoToAddPaymentCommand => new(() => navigationService1.GoTo<AddPaymentViewModel>(SelectedAccount.Id));

    public AsyncRelayCommand<PaymentListItemViewModel> GoToEditPaymentCommand => new(pvm => navigationService1.GoTo<EditPaymentViewModel>(pvm!.Id));

    public override async Task OnNavigatedAsync(object? parameter)
    {
        var accountId = Convert.ToInt32(parameter);
        SelectedAccount = mapper1.Map<AccountViewModel>(await mediator1.Send(new GetAccountByIdQuery(accountId)));
        await LoadPayments(new());
    }

    public override Task OnNavigatedBackAsync(object? parameter)
    {
        return LoadPayments(new());
    }

    private async Task LoadPayments(PaymentListFilterChangedMessage message)
    {
        var paymentData = await mediator1.Send(
            new GetPaymentsForAccount.Query(
                AccountId: SelectedAccount.Id,
                TimeRangeStart: message.TimeRangeStart,
                TimeRangeEnd: message.TimeRangeEnd,
                FilteredPaymentType: message.FilteredPaymentType,
                IsRecurringFilterActive: message.IsClearedFilterActive,
                IsClearedFilterActive: message.IsRecurringFilterActive));

        var paymentVms = paymentData.Select(
                p => new PaymentListItemViewModel
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    ChargedAccountId = p.ChargedAccountId,
                    CurrentAccountId = SelectedAccount.Id,
                    Date = p.Date.ToDateTime(TimeOnly.MinValue),
                    CategoryName = p.CategoryName,
                    IsCleared = p.IsCleared,
                    IsRecurring = p.IsRecurring,
                    Note = p.Note,
                    Type = p.Type
                })
            .OrderByDescending(p => p.Date);

        var dailyGroupedPayments = paymentVms.GroupBy(p => p.Date.Date)
            .Select(g => new PaymentDayGroup(date: DateOnly.FromDateTime(g.Key), payments: g.ToList()))
            .ToList();

        PaymentDayGroups = new(new(dailyGroupedPayments));
    }
}
