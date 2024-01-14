namespace MoneyFox.Ui.Views.Accounts.AccountList;

using CommunityToolkit.Mvvm.ComponentModel;
using Domain;

public sealed class AccountListItemViewModel : ObservableObject
{
    private Money currentBalance = Money.Zero(Currencies.USD);
    private decimal endOfMonthBalance;

    private int id;
    private bool isExcluded;
    private string name = "";

    public required int Id
    {
        get => id;

        set => SetProperty(field: ref id, newValue: value);
    }

    public required string Name
    {
        get => name;
        set => SetProperty(field: ref name, newValue: value);
    }

    public required Money CurrentBalance
    {
        get => currentBalance;
        set => SetProperty(field: ref currentBalance, newValue: value);
    }

    public required decimal EndOfMonthBalance
    {
        get => endOfMonthBalance;
        set => SetProperty(field: ref endOfMonthBalance, newValue: value);
    }

    public required bool IsExcluded
    {
        get => isExcluded;
        set => SetProperty(field: ref isExcluded, newValue: value);
    }
}
