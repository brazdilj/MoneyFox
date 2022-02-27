﻿namespace MoneyFox.Core.Tests.Queries.Statistics
{
    using Core._Pending_.Common.Interfaces;
    using Core.Aggregates;
    using Core.Aggregates.Payments;
    using Core.Queries.Statistics.Queries.GetCategorySummary;
    using FluentAssertions;
    using Infrastructure;
    using MoneyFox.Infrastructure.Persistence;
    using Moq;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class GetCategorySummaryQueryTests : IDisposable
    {
        private readonly AppDbContext context;
        private readonly Mock<IContextAdapter> contextAdapterMock;

        public GetCategorySummaryQueryTests()
        {
            context = InMemoryAppDbContextFactory.Create();

            contextAdapterMock = new Mock<IContextAdapter>();
            contextAdapterMock.SetupGet(x => x.Context).Returns(context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) => InMemoryAppDbContextFactory.Destroy(context);

        [Fact]
        public async Task GetValues_CorrectSums()
        {
            // Arrange
            var testCat1 = new Category("Ausgehen");
            var testCat2 = new Category("Rent");
            var testCat3 = new Category("Food");
            var testCat4 = new Category("Income");

            var account = new Account("test");

            var paymentList = new List<Payment>
            {
                new Payment(DateTime.Today, 60, PaymentType.Income, account, category: testCat1),
                new Payment(DateTime.Today, 90, PaymentType.Expense, account, category: testCat1),
                new Payment(DateTime.Today, 10, PaymentType.Expense, account, category: testCat3),
                new Payment(DateTime.Today, 90, PaymentType.Expense, account, category: testCat2),
                new Payment(DateTime.Today, 100, PaymentType.Income, account, category: testCat4)
            };

            context.Payments.AddRange(paymentList);
            context.SaveChanges();

            // Act
            CategorySummaryModel result =
                await new GetCategorySummaryQueryHandler(contextAdapterMock.Object)
                    .Handle(
                        new GetCategorySummaryQuery
                        {
                            StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(3)
                        },
                        default);

            // Assert
            result.CategoryOverviewItems.Count.Should().Be(4);
            result.CategoryOverviewItems[0].Value.Should().Be(-90);
            result.CategoryOverviewItems[1].Value.Should().Be(-30);
            result.CategoryOverviewItems[2].Value.Should().Be(-10);
            result.CategoryOverviewItems[3].Value.Should().Be(100);
        }

        [Fact]
        public async Task GetValues_CorrectLabels()
        {
            // Arrange
            var testCat1 = new Category("Ausgehen");
            var testCat2 = new Category("Rent");
            var testCat3 = new Category("Food");
            var testCat4 = new Category("Income");

            var account = new Account("test");
            var paymentList = new List<Payment>
            {
                new Payment(DateTime.Today, 90, PaymentType.Expense, account, category: testCat1),
                new Payment(DateTime.Today, 90, PaymentType.Expense, account, category: testCat2),
                new Payment(DateTime.Today, 10, PaymentType.Expense, account, category: testCat3),
                new Payment(DateTime.Today, 100, PaymentType.Income, account, category: testCat4)
            };

            context.Payments.AddRange(paymentList);
            context.SaveChanges();

            // Act
            CategorySummaryModel result =
                await new GetCategorySummaryQueryHandler(contextAdapterMock.Object)
                    .Handle(
                        new GetCategorySummaryQuery
                        {
                            StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(3)
                        },
                        default);

            // Assert
            result.CategoryOverviewItems[0].Label.Should().Be(testCat1.Name);
            result.CategoryOverviewItems[1].Label.Should().Be(testCat2.Name);
            result.CategoryOverviewItems[2].Label.Should().Be(testCat3.Name);
            result.CategoryOverviewItems[3].Label.Should().Be(testCat4.Name);
        }

        [Fact]
        public async Task GetValues_CorrectPercentage()
        {
            // Arrange
            var testCat1 = new Category("Ausgehen");
            var testCat2 = new Category("Ausgehen");
            var testCat3 = new Category("Income");

            var account = new Account("test");

            var paymentList = new List<Payment>
            {
                new Payment(DateTime.Today, 60, PaymentType.Expense, account, category: testCat1),
                new Payment(DateTime.Today, 90, PaymentType.Expense, account, category: testCat2),
                new Payment(DateTime.Today, 100, PaymentType.Income, account, category: testCat3)
            };

            context.Payments.AddRange(paymentList);
            context.SaveChanges();

            // Act
            CategorySummaryModel result =
                await new GetCategorySummaryQueryHandler(contextAdapterMock.Object)
                    .Handle(
                        new GetCategorySummaryQuery
                        {
                            StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(3)
                        },
                        default);

            // Assert
            result.CategoryOverviewItems[0].Percentage.Should().Be(60);
            result.CategoryOverviewItems[1].Percentage.Should().Be(40);
            result.CategoryOverviewItems[2].Percentage.Should().Be(100);
        }

        [Fact]
        public async Task GetValues_NoCategory_CorrectValue()
        {
            // Arrange
            var account = new Account("test");

            var paymentList = new List<Payment> {new Payment(DateTime.Today, 60, PaymentType.Expense, account)};

            context.Payments.AddRange(paymentList);
            context.SaveChanges();

            // Act
            CategorySummaryModel result =
                await new GetCategorySummaryQueryHandler(contextAdapterMock.Object)
                    .Handle(
                        new GetCategorySummaryQuery
                        {
                            StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(3)
                        },
                        default);

            // Assert
            result.CategoryOverviewItems[0].Value.Should().Be(-60);
        }

        [Fact]
        public async Task GetValues_NoCategory_CorrectLabel()
        {
            // Arrange
            var account = new Account("test");

            var paymentList = new List<Payment> {new Payment(DateTime.Today, 60, PaymentType.Expense, account)};

            context.Payments.AddRange(paymentList);
            context.SaveChanges();

            // Act
            CategorySummaryModel result =
                await new GetCategorySummaryQueryHandler(contextAdapterMock.Object)
                    .Handle(
                        new GetCategorySummaryQuery
                        {
                            StartDate = DateTime.Today.AddDays(-3), EndDate = DateTime.Today.AddDays(3)
                        },
                        default);

            // Assert
            result.CategoryOverviewItems[0].Label.Should().Be(Strings.NoCategoryLabel);
        }
    }
}