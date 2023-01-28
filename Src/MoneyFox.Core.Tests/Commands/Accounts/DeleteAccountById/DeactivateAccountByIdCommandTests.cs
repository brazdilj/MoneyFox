﻿namespace MoneyFox.Core.Tests.Commands.Accounts.DeleteAccountById;

using Core.Commands.Accounts.DeleteAccountById;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoneyFox.Domain.Aggregates.AccountAggregate;

public class DeactivateAccountByIdCommandTests : InMemoryTestBase
{
    private readonly DeactivateAccountByIdCommand.Handler handler;

    public DeactivateAccountByIdCommandTests()
    {
        handler = new(Context);
    }

    [Fact]
    public async Task DeactivatedAccountNotDeleted()
    {
        // Arrange
        var account = new Account("test");
        await Context.AddAsync(account);
        await Context.SaveChangesAsync();

        // Act
        await handler.Handle(request: new(account.Id), cancellationToken: default);

        // Assert
        (await Context.Accounts.FirstOrDefaultAsync(x => x.Id == account.Id)).Should().NotBeNull();
    }

    [Fact]
    public async Task AccountDeactivated()
    {
        // Arrange
        var account = new Account("test");
        await Context.AddAsync(account);
        await Context.SaveChangesAsync();

        // Act
        await handler.Handle(request: new(account.Id), cancellationToken: default);

        // Assert
        (await Context.Accounts.FirstOrDefaultAsync(x => x.Id == account.Id)).IsDeactivated.Should().BeTrue();
    }
}
