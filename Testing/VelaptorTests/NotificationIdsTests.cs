// <copyright file="NotificationIdsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using FluentAssertions;
using Velaptor;
using Xunit;

/// <summary>
/// Tests the <see cref="PushNotifications"/> class.
/// </summary>
public class NotificationIdsTests
{
    #region Prop Tests
    [Fact]
    public void GLContextCreatedIf_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.GLContextCreatedId.Should().Be("c44ff8ef-d7fe-4ede-8f72-f4d0d57a721c");
    }

    [Fact]
    public void GLInitializedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.GLInitializedId.Should().Be("2ef5c76f-c7ec-4f8b-b73e-c114b7cfbe2b");
    }

    [Fact]
    public void BatchSizeSetId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.BatchSizeSetId.Should().Be("7819e8f0-9797-4bfd-b9ed-6505c8a6ca89");
    }

    [Fact]
    public void MouseStateChangedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.MouseStateChangedId.Should().Be("b63c2dcd-3ce4-475e-b574-8951413ff381");
    }

    [Fact]
    public void KeyboardStateChangedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.KeyboardStateChangedId.Should().Be("a18686c8-10c9-4ba9-afaf-ceea77c130e2");
    }

    [Fact]
    public void TextureDisposedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.TextureDisposedId.Should().Be("953d4a76-6c3e-49b2-a609-e73b2add942a");
    }

    [Fact]
    public void SoundDisposedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.SoundDisposedId.Should().Be("863983d2-6657-4c8e-8e9a-f3cbd688abe1");
    }

    [Fact]
    public void SystemShuttingDownId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.SystemShuttingDownId.Should().Be("17b9fd1c-67ef-4f36-8973-45e32b0ee85b");
    }

    [Fact]
    public void WindowSizeChangedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.WindowSizeChangedId.Should().Be("d1095c6e-cf1f-4719-b7e2-aeeb285d1d02");
    }

    [Fact]
    public void ViewPortSizeChangedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.ViewPortSizeChangedId.Should().Be("430e7d43-ffd5-4f81-90b9-039e05ed490e");
    }

    [Fact]
    public void RenderBatchBegunId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.RenderBatchBegunId.Should().Be("845e89b2-5a9d-4091-8689-d56f5c3060f3");
    }

    [Fact]
    public void RenderBatchEndedId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.RenderBatchEndedId.Should().Be("ea261a35-58f9-4ddd-8301-004564311002");
    }

    [Fact]
    public void RenderTexturesId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.RenderTexturesId.Should().Be("0b2e75f4-cc15-4cdd-b14c-f5732644e818");
    }

    [Fact]
    public void RenderFontsId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.RenderFontsId.Should().Be("95cb1356-ae02-46f4-900c-651d50bc0de4");
    }

    [Fact]
    public void RenderRectsId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.RenderRectsId.Should().Be("27c20138-52d3-4b5d-936d-3b62e3b7db4d");
    }

    [Fact]
    public void RenderLinesId_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange & Act & Assert
        PushNotifications.RenderLinesId.Should().Be("3fb13cdb-db24-4d28-b117-b9604722277f");
    }
    #endregion
}
