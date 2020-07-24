﻿using MoneyFox.ViewModels.Budget;
using MoneyFox.Views.Accounts;
using MoneyFox.Views.Dashboard;
using Xamarin.Forms;

namespace MoneyFox
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute(ViewModelLocator.DashboardRoute, typeof(DashboardPage));
            Routing.RegisterRoute(ViewModelLocator.AccountListRoute, typeof(AccountListPage));
            Routing.RegisterRoute(ViewModelLocator.AddAccountRoute, typeof(AddAccountPage));
            Routing.RegisterRoute(ViewModelLocator.BudgetListRoute, typeof(BudgetListPage));
        }
    }
}
