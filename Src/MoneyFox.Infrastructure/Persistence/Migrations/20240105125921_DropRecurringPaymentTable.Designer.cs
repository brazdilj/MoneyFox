﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoneyFox.Infrastructure.Persistence;

#nullable disable

namespace MoneyFox.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240105125921_DropRecurringPaymentTable")]
    partial class DropRecurringPaymentTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.14");

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.AccountAggregate.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("CurrentBalance")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeactivated")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsExcluded")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.AccountAggregate.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChargedAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCleared")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRecurring")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("RecurringTransactionId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TargetAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ChargedAccountId");

                    b.HasIndex("TargetAccountId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.BudgetAggregate.Budget", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("IncludedCategories")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Budgets");
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.CategoryAggregate.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<bool>("RequireNote")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.RecurringTransactionAggregate.RecurringTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ChargedAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("date");

                    b.Property<bool>("IsLastDayOfMonth")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsTransfer")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastRecurrence")
                        .HasColumnType("date");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<int>("Recurrence")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("RecurringTransactionId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("date");

                    b.Property<int?>("TargetAccountId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RecurringTransactionId")
                        .IsUnique();

                    b.ToTable("RecurringTransactions");
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.AccountAggregate.Payment", b =>
                {
                    b.HasOne("MoneyFox.Domain.Aggregates.CategoryAggregate.Category", "Category")
                        .WithMany("Payments")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MoneyFox.Domain.Aggregates.AccountAggregate.Account", "ChargedAccount")
                        .WithMany()
                        .HasForeignKey("ChargedAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MoneyFox.Domain.Aggregates.AccountAggregate.Account", "TargetAccount")
                        .WithMany()
                        .HasForeignKey("TargetAccountId");

                    b.Navigation("Category");

                    b.Navigation("ChargedAccount");

                    b.Navigation("TargetAccount");
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.BudgetAggregate.Budget", b =>
                {
                    b.OwnsOne("MoneyFox.Domain.Aggregates.BudgetAggregate.BudgetInterval", "Interval", b1 =>
                        {
                            b1.Property<int>("BudgetId")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("NumberOfMonths")
                                .HasColumnType("INTEGER")
                                .HasColumnName("IntervalNumberOfMonths");

                            b1.HasKey("BudgetId");

                            b1.ToTable("Budgets");

                            b1.WithOwner()
                                .HasForeignKey("BudgetId");
                        });

                    b.OwnsOne("MoneyFox.Domain.Aggregates.BudgetAggregate.SpendingLimit", "SpendingLimit", b1 =>
                        {
                            b1.Property<int>("BudgetId")
                                .HasColumnType("INTEGER");

                            b1.Property<decimal>("Value")
                                .HasColumnType("TEXT")
                                .HasColumnName("SpendingLimit");

                            b1.HasKey("BudgetId");

                            b1.ToTable("Budgets");

                            b1.WithOwner()
                                .HasForeignKey("BudgetId");
                        });

                    b.Navigation("Interval")
                        .IsRequired();

                    b.Navigation("SpendingLimit")
                        .IsRequired();
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.RecurringTransactionAggregate.RecurringTransaction", b =>
                {
                    b.OwnsOne("MoneyFox.Domain.Money", "Amount", b1 =>
                        {
                            b1.Property<int>("RecurringTransactionId")
                                .HasColumnType("INTEGER");

                            b1.Property<decimal>("Amount")
                                .HasColumnType("TEXT")
                                .HasColumnName("Amount");

                            b1.Property<string>("Currency")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("Currency");

                            b1.HasKey("RecurringTransactionId");

                            b1.ToTable("RecurringTransactions");

                            b1.WithOwner()
                                .HasForeignKey("RecurringTransactionId");
                        });

                    b.Navigation("Amount")
                        .IsRequired();
                });

            modelBuilder.Entity("MoneyFox.Domain.Aggregates.CategoryAggregate.Category", b =>
                {
                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}
