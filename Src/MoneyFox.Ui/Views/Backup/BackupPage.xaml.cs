namespace MoneyFox.Ui.Views.Backup;

using ViewModels.DataBackup;

public partial class BackupPage
{
    public BackupPage()
    {
        InitializeComponent();
        BindingContext = App.GetViewModel<BackupViewModel>();
    }

    internal BackupViewModel ViewModel => (BackupViewModel)BindingContext;

    protected override void OnAppearing()
    {
        ViewModel.InitializeCommand.Execute(null);
    }
}
