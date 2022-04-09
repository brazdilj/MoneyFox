﻿namespace MoneyFox.Views.Dialogs
{

    using System;
    using System.Threading.Tasks;
    using Core._Pending_.Common.Messages;
    using Rg.Plugins.Popup.Extensions;
    using ViewModels.Dialogs;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPopup
    {
        public FilterPopup()
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.SelectFilterDialogViewModel;
        }

        public FilterPopup(PaymentListFilterChangedMessage message)
        {
            InitializeComponent();
            BindingContext = ViewModelLocator.SelectFilterDialogViewModel;
            ViewModel.Initialize(message);
        }

        private SelectFilterDialogViewModel ViewModel => (SelectFilterDialogViewModel)BindingContext;

        public async Task ShowAsync()
        {
            await Application.Current.MainPage.Navigation.PushPopupAsync(this);
        }

        private static async Task DismissAsync()
        {
            await Application.Current.MainPage.Navigation.PopPopupAsync();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            ViewModel.FilterSelectedCommand.Execute(null);
            await DismissAsync();
        }
    }

}
