﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GalaSoft.MvvmLight.Views;
using MediatR;
using MoneyFox.Application.Payments.Commands.CreatePayment;
using MoneyFox.Application.Resources;
using MoneyFox.Domain;
using MoneyFox.Domain.Entities;
using MoneyFox.Domain.Exceptions;
using MoneyFox.Presentation.Commands;
using MoneyFox.Presentation.Facades;
using MoneyFox.Presentation.Services;
using MoneyFox.Presentation.Utilities;
using NLog;
using IDialogService = MoneyFox.Presentation.Interfaces.IDialogService;

namespace MoneyFox.Presentation.ViewModels
{
    public class AddPaymentViewModel : ModifyPaymentViewModel 
    {
        private ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator mediator;
        private readonly IMapper mapper;
        private readonly IPaymentService paymentService;
        private readonly INavigationService navigationService;
        private readonly IDialogService dialogService;

        public AddPaymentViewModel(IMediator mediator,
                                   IMapper mapper,
                                   IPaymentService paymentService,
                                   IDialogService dialogService,
                                   ISettingsFacade settingsFacade,
                                   IBackupService backupService,
                                   INavigationService navigationService)
            : base(mediator, mapper, dialogService, settingsFacade, backupService, navigationService)
        {
            this.mediator = mediator;
            this.mapper = mapper;
            this.paymentService = paymentService;
            this.navigationService = navigationService;
            this.dialogService = dialogService;
        }

        public PaymentType PaymentType { get; set; }

        public AsyncCommand InitializeCommand => new AsyncCommand(Initialize);

        protected override async Task Initialize()
        {
            Title = PaymentTypeHelper.GetViewTitleForType(PaymentType, false);
            SelectedPayment.Type = PaymentType;

            await base.Initialize();

            SelectedPayment.ChargedAccount = ChargedAccounts.FirstOrDefault();

            if (SelectedPayment.IsTransfer)
            {
                SelectedItemChangedCommand.Execute(null);
                SelectedPayment.TargetAccount = TargetAccounts.FirstOrDefault();
            }
        }

        protected override async Task SavePayment()
        {
            try {
                var payment = new Payment(SelectedPayment.Date, 
                                          SelectedPayment.Amount, 
                                          SelectedPayment.Type,
                                          mapper.Map<Account>(SelectedPayment.ChargedAccount),
                                          mapper.Map<Account>(SelectedPayment.TargetAccount),
                                          mapper.Map<Category>(SelectedPayment.Category),
                                          SelectedPayment.Note);

                if (SelectedPayment.IsRecurring) {
                    payment.AddRecurringPayment(SelectedPayment.RecurringPayment.Recurrence, SelectedPayment.RecurringPayment.EndDate);
                }

                await mediator.Send(new CreatePaymentCommand(payment));
                navigationService.GoBack();
            }
            catch (InvalidEndDateException)
            {
                await dialogService.ShowMessage(Strings.InvalidEnddateTitle, Strings.InvalidEnddateMessage);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
