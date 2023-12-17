namespace MoneyFox.Ui.Tests.Views.Categories;

using Common.Navigation;
using Core.Common.Interfaces;
using Core.Features.CategoryDeletion;
using Core.Queries;
using MediatR;
using Ui.Views.Categories.ModifyCategory;

public class EditCategoryViewModelTests
{
    private readonly IDialogService dialogService;
    private readonly IMediator mediator;
    private readonly INavigationService navigationService;

    private readonly EditCategoryViewModel vm;

    public EditCategoryViewModelTests()
    {
        mediator = Substitute.For<IMediator>();
        dialogService = Substitute.For<IDialogService>();
        navigationService = Substitute.For<INavigationService>();
        vm = new(mediator: mediator, dialogService: dialogService, navigationService: navigationService);
    }

    [Fact]
    public async Task CallsDelete_WhenConfirmationWasConfirmed()
    {
        // Arrange
        dialogService.ShowConfirmMessageAsync(title: Arg.Any<string>(), message: Arg.Any<string>()).Returns(true);
        mediator.Send(Arg.Any<GetCategoryById.Query>())
        .Returns(
            new CategoryData(
                Id: 4,
                Name: "Beer",
                Note: null,
                NoteRequired: false,
                Created: DateTime.Now,
                LastModified: DateTime.Now));

        // Act
        await vm.OnNavigatedAsync(4);
        await vm.DeleteCommand.ExecuteAsync(null);

        // Assert
        await mediator.Received(1).Send(request: Arg.Any<DeleteCategoryById.Command>(), cancellationToken: Arg.Any<CancellationToken>());
        await navigationService.Received(1).GoBack();
    }

    [Fact]
    public async Task DoesNotDelete_WhenConfirmationWasDeclined()
    {
        // Arrange
        _ = dialogService.ShowConfirmMessageAsync(title: Arg.Any<string>(), message: Arg.Any<string>()).Returns(false);

        // Act
        await vm.DeleteCommand.ExecuteAsync(null);

        // Assert
        await mediator.Received(0).Send(request: Arg.Any<DeleteCategoryById.Command>(), cancellationToken: Arg.Any<CancellationToken>());
        await navigationService.Received(0).GoBack();
    }
}
