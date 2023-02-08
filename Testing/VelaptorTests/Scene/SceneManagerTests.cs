// <copyright file="SceneManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Scene;
using Xunit;

/// <summary>
/// Tests the <see cref="SceneManager"/> class.
/// </summary>
public class SceneManagerTests
{
    #region Prop Tests
    [Fact]
    public void CurrentScene_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var mockScene = new Mock<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object);

        // Act
        var actual = sut.CurrentScene;

        // Assert
        actual.Should().BeSameAs(mockScene.Object);
    }

    [Fact]
    public void InActiveScenes_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expected = new[] { sceneBId }.AsReadOnly();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(sceneBId);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object);

        // Act
        var actual = sut.InActiveScenes;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(
        "test-scene",
        "2ab9a4ff-03cb-46e6-8e5f-99989567d968",
        "The scene 'test-scene' with the ID '2ab9a4ff-03cb-46e6-8e5f-99989567d968' already exists.")]
    [InlineData(
        null,
        "b2c7123b-6044-4c6d-b79b-4086f3f89939",
        "The scene with the ID 'b2c7123b-6044-4c6d-b79b-4086f3f89939' already exists.")]
    [InlineData(
        "",
        "c0b7410a-356d-488c-8b62-cb48a30b76e5",
        "The scene with the ID 'c0b7410a-356d-488c-8b62-cb48a30b76e5' already exists.")]
    public void AddScene_WhenSceneWithIdAlreadyExists_ThrowsException(
        string name,
        Guid id,
        string expected)
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.SetupGet(p => p.Id).Returns(id);

        var mockSceneB = new Mock<IScene>();
        mockSceneB.SetupGet(p => p.Name).Returns(name);
        mockSceneB.SetupGet(p => p.Id).Returns(id);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);

        // Act
        var act = () => sut.AddScene(mockSceneB.Object);

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage(expected);
    }

    [Fact]
    public void AddScene_SceneIsSetToBeActive_SetsAllOtherScenesToInactive()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);

        // Act
        sut.AddScene(mockSceneB.Object, setToActive: true);

        // Assert
        sut.CurrentScene.Should().BeSameAs(mockSceneB.Object);
    }

    [Fact]
    public void AddScene_WhenNoScenesExist_SetsSceneAsActive()
    {
        // Arrange
        var mockScene = new Mock<IScene>();

        var sut = new SceneManager();

        // Act
        sut.AddScene(mockScene.Object, setToActive: false);

        // Assert
        sut.CurrentScene.Should().BeSameAs(mockScene.Object);
    }

    [Fact]
    public void RemoveScene_WithNoScenes_DoesNotThrowExceptionOrRemoveItems()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var act = () => sut.RemoveScene(Guid.NewGuid());

        // Assert
        act.Should().NotThrow();
        sut.CurrentScene.Should().BeNull();
    }

    [Fact]
    public void RemoveScene_WithSceneIDThatDoesNotExist_DoesNotRemoveIncorrectItem()
    {
        // Arrange
        var sceneId = Guid.NewGuid();
        var mockSceneA = new Mock<IScene>();
        mockSceneA.SetupGet(p => p.Id).Returns(sceneId);

        var doesNotExistSceneId = Guid.NewGuid();

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);

        // Act
        sut.RemoveScene(doesNotExistSceneId);

        // Assert
        sut.CurrentScene.Should().NotBeNull();
        sut.SceneExists(sceneId).Should().BeTrue();
    }

    [Fact]
    public void RemoveScene_WhenRemovingTheOnlyScene_RemovesScene()
    {
        // Arrange
        var sceneId = Guid.NewGuid();

        var mockScene = new Mock<IScene>();
        mockScene.Name = nameof(mockScene);
        mockScene.SetupGet(p => p.Id).Returns(sceneId);
        mockScene.SetupGet(p => p.Name).Returns(nameof(mockScene));

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object, setToActive: true);

        // Act
        sut.RemoveScene(sceneId);

        // Assert
        sut.CurrentScene.Should().BeNull();
        mockScene.VerifyOnce(m => m.UnloadContent());
    }

    [Fact]
    public void RemoveScene_WhenRemovingTheCurrentlyActiveScene_RemovesScene()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object, setToActive: true);
        sut.AddScene(mockSceneB.Object);

        // Act
        sut.RemoveScene(sceneAId);

        // Assert
        sut.CurrentScene.Should().NotBeNull();
        mockSceneA.VerifyOnce(m => m.UnloadContent());
    }

    [Fact]
    public void RemoveScene_WhenNotRemovingTheCurrentlyActiveScene_RemovesScene()
    {
        // Arrange
        var sceneBId = Guid.NewGuid();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(sceneBId);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object, setToActive: true);
        sut.AddScene(mockSceneB.Object);

        // Act
        sut.RemoveScene(sceneBId);

        // Assert
        sut.CurrentScene.Should().NotBeNull();
    }

    [Fact]
    public void NextScene_WithOnlySingleScene_DoesNotLoadOrUnloadAnything()
    {
        // Arrange
        var mockScene = new Mock<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object);

        // Act
        sut.NextScene();

        // Assert
        mockScene.VerifyNever(m => m.UnloadContent());
        mockScene.VerifyNever(m => m.LoadContent());
    }

    [Fact]
    public void NextScene_WithMoreThanASingleSceneAndCurrentSceneIsNotTheLastScene_MovesToTheNextScene()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object, setToActive: true);
        sut.AddScene(mockSceneB.Object);

        // Act
        sut.NextScene();

        // Assert
        mockSceneA.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().NotBeSameAs(mockSceneA.Object);

        mockSceneB.VerifyOnce(m => m.LoadContent());
        sut.CurrentScene.Should().BeSameAs(mockSceneB.Object);
    }

    [Fact]
    public void NextScene_WithMoreThanASingleSceneAndIsTheLastScene_MovesToTheFirstScene()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object, setToActive: true);

        // Act
        sut.NextScene();

        // Assert
        mockSceneA.VerifyOnce(m => m.LoadContent());
        sut.CurrentScene.Should().BeSameAs(mockSceneA.Object);

        mockSceneB.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().NotBeSameAs(mockSceneB.Object);
    }

    [Fact]
    public void PreviousScene_WithOnlySingleScene_DoesNotLoadOrUnloadAnything()
    {
        // Arrange
        var mockScene = new Mock<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object);

        // Act
        sut.PreviousScene();

        // Assert
        mockScene.VerifyNever(m => m.UnloadContent());
        mockScene.VerifyNever(m => m.LoadContent());
    }

    [Fact]
    public void PreviousScene_WithMoreThanASingleSceneAndCurrentSceneIsNotTheFirstScene_MovesToThePreviousScene()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object, setToActive: true);

        // Act
        sut.PreviousScene();

        // Assert
        mockSceneB.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().NotBeSameAs(mockSceneB.Object);

        mockSceneA.VerifyOnce(m => m.LoadContent());
        sut.CurrentScene.Should().BeSameAs(mockSceneA.Object);
    }

    [Fact]
    public void PreviousScene_WithMoreThanASingleSceneAndCurrentSceneIsTheFirstScene_MovesToTheLastScene()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object, setToActive: true);
        sut.AddScene(mockSceneB.Object);

        // Act
        sut.PreviousScene();

        // Assert
        mockSceneB.VerifyOnce(m => m.LoadContent());
        sut.CurrentScene.Should().BeSameAs(mockSceneB.Object);

        mockSceneA.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().NotBeSameAs(mockSceneA.Object);
    }

    [Fact]
    public void SetSceneAsActive_WhenSceneDoesNotExist_DoesNotChangeAnyActiveStatusForAnyScenes()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expected = new[] { sceneAId }.AsReadOnly();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(sceneBId);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object, setToActive: true);

        // Act
        sut.SetSceneAsActive(Guid.NewGuid());

        // Assert
        sut.InActiveScenes.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetSceneAsActive_WhenInvoked_SetsSceneToActiveAndOthersToInActive()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expected = new[] { sceneBId }.AsReadOnly();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Id).Returns(sceneBId);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object, setToActive: true);

        // Act
        sut.SetSceneAsActive(sceneAId);

        // Assert
        sut.InActiveScenes.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void LoadContent_WhenDisposed_ThrowsException()
    {
        // Arrange
        var expected = "Cannot load a scene manager that has been disposed.";
        expected += $"{Environment.NewLine}Object name: 'SceneManager'.";

        var sut = new SceneManager();
        sut.Dispose();

        // Act
        var act = () => sut.LoadContent();

        // Assert
        act.Should().Throw<ObjectDisposedException>()
            .WithMessage(expected);
    }

    [Fact]
    public void LoadContent_WhenAlreadyLoaded_DoesNotLoadSceneContentAgain()
    {
        // Arrange
        var mockScene = new Mock<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object);
        sut.LoadContent();

        // Act
        sut.LoadContent();

        // Assert
        mockScene.VerifyOnce(m => m.LoadContent());
    }

    [Fact]
    public void LoadContent_WithNoScenes_DoesNotThrowException()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var act = () => sut.LoadContent();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void UnloadContent_WhenContentIsNotLoaded_DoesNotUnloadContent()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object);

        // Act
        sut.UnloadContent();

        // Assert
        mockSceneA.VerifyNever(m => m.UnloadContent());
        mockSceneB.VerifyNever(m => m.UnloadContent());
        sut.CurrentScene.Should().NotBeNull();
    }

    [Fact]
    public void UnloadContent_WhenContentIsLoadedAndManagerIsDisposed_DoesNotUnloadContentMoreThanOnce()
    {
        // Arrange
        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));
        mockSceneA.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));
        mockSceneB.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object);
        sut.LoadContent();
        sut.Dispose();

        // Act
        sut.UnloadContent();

        // Assert
        mockSceneA.VerifyOnce(m => m.UnloadContent());
        mockSceneB.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().BeNull();
    }

    [Fact]
    public void UnloadContent_WhenContentIsLoadedAndManagerIsNotDisposed_DoesNotUnloadContentMoreThanOnce()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expectedInActiveIds = new[] { sceneBId }.AsReadOnly();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.Name = nameof(mockSceneA);
        mockSceneA.SetupGet(p => p.Name).Returns(nameof(mockSceneA));
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);

        var mockSceneB = new Mock<IScene>();
        mockSceneB.Name = nameof(mockSceneB);
        mockSceneB.SetupGet(p => p.Name).Returns(nameof(mockSceneB));
        mockSceneB.SetupGet(p => p.Id).Returns(sceneBId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object, setToActive: true);
        sut.AddScene(mockSceneB.Object);
        sut.LoadContent();

        // Act
        sut.UnloadContent();

        // Assert
        mockSceneA.VerifyOnce(m => m.UnloadContent());
        mockSceneB.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().NotBeNull();
        sut.CurrentScene.Id.Should().Be(sceneAId);
        sut.InActiveScenes.Should().BeEquivalentTo(expectedInActiveIds);
    }

    [Fact]
    public void Update_WithNoScenes_DoesNotThrowException()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var act = () => sut.Update(default);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Update_WithScenes_UpdatesScene()
    {
        // Arrange
        var mockScene = new Mock<IScene>();
        mockScene.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object);

        var frameTime = new FrameTime
        {
            TotalTime = new TimeSpan(1, 2, 3, 4, 5),
            ElapsedTime = new TimeSpan(6, 7, 8, 9, 10),
        };

        // Act
        sut.Update(frameTime);

        // Assert
        mockScene.VerifyOnce(m => m.Update(frameTime));
    }

    [Fact]
    public void Render_WithNoScenes_DoesNotThrowException()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var act = () => sut.Render();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Render_WithScenes_RendersScene()
    {
        // Arrange
        var mockScene = new Mock<IScene>();
        mockScene.SetupGet(p => p.Id).Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockScene.Object);

        // Act
        sut.Render();

        // Assert
        mockScene.VerifyOnce(m => m.Render());
    }

    [Fact]
    public void SceneExists_WhenSceneDoesExist_ReturnsTrue()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);

        // Act
        var actual = sut.SceneExists(sceneAId);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void SceneExists_WhenSceneDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);

        // Act
        var actual = sut.SceneExists(Guid.NewGuid());

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Dispose_WhenInvokedS_DisposesOfScenes()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var mockSceneA = new Mock<IScene>();
        mockSceneA.SetupGet(p => p.Id).Returns(sceneAId);

        var mockSceneB = new Mock<IScene>();
        mockSceneB.SetupGet(p => p.Id).Returns(sceneBId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA.Object);
        sut.AddScene(mockSceneB.Object);

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        mockSceneA.VerifyOnce(m => m.UnloadContent());
        mockSceneB.VerifyOnce(m => m.UnloadContent());
        sut.CurrentScene.Should().BeNull();
    }
    #endregion
}
