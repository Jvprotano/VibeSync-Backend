using VibeSync.Domain.Domains;
using VibeSync.Domain.Enums;
using VibeSync.Domain.Models;
using VibeSync.Tests.Support.Factories;

namespace VibeSync.Tests.Domain;

public class UserPlanTests
{
    private static readonly Guid _userId = Guid.NewGuid();
    private readonly UserPlan _userPlan = new(_userId, Guid.NewGuid(), DateTime.UtcNow, null, null, null, SubscriptionStatusEnum.Active);

    [Fact]
    public void ReachedMaxSpaces_ShouldReturnTrue_WhenUserHasReachedMaxSpaces()
    {
        // Arrange
        var spaces = SpaceFactory.Generate(3, _userId);
        _userPlan.Plan = new Plan(Guid.NewGuid(), "Basic Plan", 3, 0);

        // Act
        var result = _userPlan.ReachedMaxSpaces(spaces);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ReachedMaxSpaces_ShouldReturnFalse_When_SpacesAreFromDifferentMonth()
    {
        // Arrange
        var spaces = SpaceFactory.Generate(2, _userId);
        spaces.Add(new Space("Next month Space", _userId, DateTime.UtcNow.AddMonths(-1)));
        _userPlan.Plan = new Plan(Guid.NewGuid(), "Basic Plan", 3, 0);

        // Act
        var result = _userPlan.ReachedMaxSpaces(spaces);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReachedMaxSpaces_ShouldReturnFalse_WhenUserHasNotReachedMaxSpaces()
    {
        // Arrange
        var spaces = SpaceFactory.Generate(2, _userId);
        _userPlan.Plan = new Plan(Guid.NewGuid(), "Basic Plan", 3, 0);

        // Act
        var result = _userPlan.ReachedMaxSpaces(spaces);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReachedMaxSpaces_ShouldReturnFalse_WhenPlanIsNull()
    {
        // Arrange
        var spaces = SpaceFactory.Generate(2, _userId);
        _userPlan.Plan = null;

        // Act
        var result = _userPlan.ReachedMaxSpaces(spaces);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatus_WhenStatusIsNotCanceled()
    {
        // Arrange
        var newStatus = SubscriptionStatusEnum.Pending;

        // Act
        _userPlan.UpdateStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, _userPlan.Status);
    }

    [Fact]
    public void UpdateStatus_ShouldCancel_WhenStatusIsCanceled()
    {
        // Arrange
        var newStatus = SubscriptionStatusEnum.Canceled;

        // Act
        _userPlan.UpdateStatus(newStatus);

        // Assert
        Assert.Equal(newStatus, _userPlan.Status);
        Assert.NotNull(_userPlan.CancellationDate);
    }

    [Fact]
    public void UpdateStatus_ShouldNotUpdate_WhenAlreadyCanceled()
    {
        // Arrange
        _userPlan.UpdateStatus(SubscriptionStatusEnum.Canceled);
        var newStatus = SubscriptionStatusEnum.Active;

        // Act
        _userPlan.UpdateStatus(newStatus);

        // Assert
        Assert.Equal(SubscriptionStatusEnum.Canceled, _userPlan.Status);
    }
}