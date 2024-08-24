// <copyright file="SceneManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using NSubstitute;
using Velaptor;
using Velaptor.Scene;
using Velaptor.Scene.Exceptions;
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
        var mockScene = Substitute.For<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene);

        // Act
        var actual = sut.CurrentScene;

        // Assert
        actual.Should().BeSameAs(mockScene);
    }

    [Fact]
    public void InActiveScenes_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expected = new[] { sceneBId }.AsReadOnly();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);
        mockSceneA.Name.Returns(nameof(mockSceneA));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(sceneBId);
        mockSceneB.Name.Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB);

        // Act
        var actual = sut.InActiveScenes;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void IsLoaded_BeforeContentIsLoaded_ReturnsFalse()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var actual = sut.IsLoaded;

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsLoaded_AfterContentIsLoaded_ReturnsTrue()
    {
        // Arrange
        var sut = new SceneManager();
        sut.LoadContent();

        // Act
        var actual = sut.IsLoaded;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void TotalScenes_WithExistingScenes_ReturnsCorrectNumberOfScenes()
    {
        // Arrange
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(new Guid("C6BE5B20-B672-40B1-96F0-C231147E008D"));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(new Guid("7CE3F8E2-42A0-4EC4-BECB-7B7CFA88D707"));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB);

        // Act
        var actual = sut.TotalScenes;

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void CurrentSceneIndex_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var actual = sut.CurrentSceneIndex;

        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void UsesNavigationWrapping_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var actual = sut.UsesNavigationWrapping;

        // Assert
        actual.Should().BeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void AddScene_WhenSceneWithIdAlreadyExists_ThrowsException()
    {
        // Arrange
        var sceneId = Guid.NewGuid();
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneId);

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Name.Returns("test-name");
        mockSceneB.Id.Returns(sceneId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);

        // Act
        var act = () => sut.AddScene(mockSceneB);

        // Assert
        act.Should().Throw<Exception>()
            .WithMessage($"The scene 'test-name' with the ID '{sceneId}' already exists.");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddScene_With2ParamsWhenNoScenesExistWithActiveAsFalse_CorrectlyAddsScene(bool setToActive)
    {
        // Arrange
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(new Guid("3d5415ff-2ede-4b75-b42a-6c511e1a2bb8"));

        var sut = new SceneManager();

        // Act
        sut.AddScene(mockSceneA, setToActive);

        // Assert
        sut.CurrentScene.Should().BeSameAs(mockSceneA);
        sut.CurrentSceneIndex.Should().Be(0);
    }

    [Fact]
    public void AddScene_With2ParamsWhenAddingSecondSceneWithActiveAsTrue_CorrectlyAddsScene()
    {
        // Arrange
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(new Guid("3d5415ff-2ede-4b75-b42a-6c511e1a2bb8"));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(new Guid("68110c4a-c588-4475-b3ae-90444811a39c"));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);

        // Act
        sut.AddScene(mockSceneB, setToActive: true);

        // Assert
        sut.CurrentScene.Should().BeSameAs(mockSceneB);
        sut.CurrentSceneIndex.Should().Be(1);
    }

    [Fact]
    public void AddScene_With2ParamsWhenAddingSecondSceneWithActiveAsFalse_CorrectlyAddsScene()
    {
        // Arrange
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(new Guid("649e0a11-3842-4eaf-aa70-558bd119a452"));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(new Guid("5bd043a9-3ef3-4b90-b680-4a01f9bea02e"));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);

        // Act
        sut.AddScene(mockSceneB, setToActive: false);

        // Assert
        sut.CurrentScene.Should().BeSameAs(mockSceneA);
        sut.CurrentSceneIndex.Should().Be(0);
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
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneId);

        var doesNotExistSceneId = Guid.NewGuid();

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);

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

        var mockScene = Substitute.For<IScene>();
        mockScene.Id.Returns(sceneId);
        mockScene.Name.Returns(nameof(mockScene));

        var sut = new SceneManager();
        sut.AddScene(mockScene, setToActive: true);

        // Act
        sut.RemoveScene(sceneId);

        // Assert
        sut.CurrentScene.Should().BeNull();
        mockScene.Received(1).UnloadContent();
    }

    [Fact]
    public void RemoveScene_WhenRemovingTheCurrentlyActiveScene_RemovesScene()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);
        mockSceneA.Name.Returns(nameof(mockSceneA));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(Guid.NewGuid());
        mockSceneB.Name.Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA, setToActive: true);
        sut.AddScene(mockSceneB);

        // Act
        sut.RemoveScene(sceneAId);

        // Assert
        sut.CurrentScene.Should().NotBeNull();
        mockSceneA.Received(1).UnloadContent();
    }

    [Fact]
    public void RemoveScene_WhenNotRemovingTheCurrentlyActiveScene_RemovesScene()
    {
        // Arrange
        var sceneBId = Guid.NewGuid();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(Guid.NewGuid());
        mockSceneA.Name.Returns(nameof(mockSceneA));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(sceneBId);
        mockSceneB.Name.Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA, setToActive: true);
        sut.AddScene(mockSceneB);

        // Act
        sut.RemoveScene(sceneBId);

        // Assert
        sut.CurrentScene.Should().NotBeNull();
    }

    [Fact]
    public void NextScene_WithOnlySingleScene_DoesNotLoadOrUnloadAnything()
    {
        // Arrange
        var mockScene = Substitute.For<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene);

        // Act
        sut.NextScene();

        // Assert
        mockScene.DidNotReceive().UnloadContent();
        mockScene.DidNotReceive().LoadContent();
    }

    [Fact]
    public void NextScene_WithNavigationWrappingAndFirstSceneIsActiveAndLastSceneIsInactive_MovesToLastScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = true;

        sut.AddScene(mockFirstScene, true);
        sut.AddScene(mockLastScene, false);

        // Act
        sut.NextScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(1);

        mockFirstScene.Received(1).UnloadContent();
        mockLastScene.Received(1).LoadContent();

        sut.CurrentScene.Should().NotBeSameAs(mockFirstScene);
        sut.CurrentScene.Should().BeSameAs(mockLastScene);
    }

    [Fact]
    public void NextScene_WithNavigationWrappingAndFirstSceneIsInactiveAndLastSceneIsActive_MovesToFirstScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = true;

        sut.AddScene(mockFirstScene, false);
        sut.AddScene(mockLastScene, true);

        // Act
        sut.NextScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(0);

        mockLastScene.Received(1).UnloadContent();
        mockFirstScene.Received(1).LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockFirstScene);
        sut.CurrentScene.Should().NotBeSameAs(mockLastScene);
    }

    [Fact]
    public void NextScene_WithoutNavigationWrappingAndFirstSceneIsActiveAndLastSceneIsInactive_MovesToLastScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = false;

        sut.AddScene(mockFirstScene, true);
        sut.AddScene(mockLastScene, false);

        // Act
        sut.NextScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(1);

        mockFirstScene.Received(1).UnloadContent();
        mockLastScene.Received(1).LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockLastScene);
        sut.CurrentScene.Should().NotBeSameAs(mockFirstScene);
    }

    [Fact]
    public void NextScene_WithoutNavigationWrappingAndFirstSceneIsInactiveAndLastSceneIsActive_DoesNotMoveToFirstScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = false;

        sut.AddScene(mockFirstScene, false);
        sut.AddScene(mockLastScene, true);

        // Act
        sut.NextScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(1);

        mockFirstScene.DidNotReceive().UnloadContent();
        mockLastScene.DidNotReceive().LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockLastScene);
        sut.CurrentScene.Should().NotBeSameAs(mockFirstScene);
    }

    [Fact]
    public void PreviousScene_WithOnlySingleScene_DoesNotLoadOrUnloadAnything()
    {
        // Arrange
        var mockScene = Substitute.For<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene);

        // Act
        sut.PreviousScene();

        // Assert
        mockScene.DidNotReceive().UnloadContent();
        mockScene.DidNotReceive().LoadContent();
    }

    [Fact]
    public void PreviousScene_WithNavigationWrappingAndFirstSceneIsActiveAndLastSceneIsInactive_MovesToLastScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = true;

        sut.AddScene(mockFirstScene, true);
        sut.AddScene(mockLastScene, false);

        // Act
        sut.PreviousScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(1);

        mockFirstScene.Received(1).UnloadContent();
        mockLastScene.Received(1).LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockLastScene);
        sut.CurrentScene.Should().NotBeSameAs(mockFirstScene);
    }

    [Fact]
    public void PreviousScene_WithNavigationWrappingAndFirstSceneIsInactiveAndLastSceneIsActive_MovesToFirstScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = true;

        sut.AddScene(mockFirstScene, false);
        sut.AddScene(mockLastScene, true);

        // Act
        sut.PreviousScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(0);

        mockLastScene.Received(1).UnloadContent();
        mockFirstScene.Received(1).LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockFirstScene);
        sut.CurrentScene.Should().NotBeSameAs(mockLastScene);
    }

    [Fact]
    public void PreviousScene_WithoutNavigationWrappingAndFirstSceneIsActiveAndLastSceneIsInactive_DoesNotMoveToLastScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = false;

        sut.AddScene(mockFirstScene, true);
        sut.AddScene(mockLastScene, false);

        // Act
        sut.PreviousScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(0);

        mockFirstScene.DidNotReceive().UnloadContent();
        mockLastScene.DidNotReceive().LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockFirstScene);
        sut.CurrentScene.Should().NotBeSameAs(mockLastScene);
    }

    [Fact]
    public void PreviousScene_WithoutNavigationWrappingAndFirstSceneIsInactiveAndLastSceneIsActive_MovesToFirstScene()
    {
        // Arrange
        var mockFirstScene = Substitute.For<IScene>();
        mockFirstScene.Id.Returns(new Guid("eb3092e0-9132-4b17-9f98-a67828af595d"));
        mockFirstScene.Name.Returns(nameof(mockFirstScene));

        var mockLastScene = Substitute.For<IScene>();
        mockLastScene.Name.Returns(nameof(mockLastScene));
        mockLastScene.Id.Returns(new Guid("24809195-ac65-4b45-9b16-50c30bc0355d"));

        var sut = new SceneManager();
        sut.UsesNavigationWrapping = false;

        sut.AddScene(mockFirstScene, false);
        sut.AddScene(mockLastScene, true);

        // Act
        sut.PreviousScene();

        // Assert
        sut.CurrentSceneIndex.Should().Be(0);

        mockLastScene.Received(1).UnloadContent();
        mockFirstScene.Received(1).LoadContent();

        sut.CurrentScene.Should().BeSameAs(mockFirstScene);
        sut.CurrentScene.Should().NotBeSameAs(mockLastScene);
    }

    [Fact]
    public void SetSceneAsActive_WhenSceneDoesNotExist_ThrowsException()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();
        var id = Guid.NewGuid();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);
        mockSceneA.Name.Returns(nameof(mockSceneA));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(sceneBId);
        mockSceneB.Name.Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB, setToActive: true);

        // Act
        var act = () => sut.SetSceneAsActive(id);

        // Assert
        act.Should().Throw<SceneDoesNotExistException>()
            .WithMessage($"The scene with the ID '{id.ToString()}' does not exist.");
    }

    [Fact]
    public void SetSceneAsActive_WhenInvoked_SetsSceneToActiveAndOthersToInActive()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expected = new[] { sceneBId }.AsReadOnly();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);
        mockSceneA.Name.Returns(nameof(mockSceneA));

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(sceneBId);
        mockSceneB.Name.Returns(nameof(mockSceneB));

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB, setToActive: true);

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
        var mockScene = Substitute.For<IScene>();

        var sut = new SceneManager();
        sut.AddScene(mockScene);
        sut.LoadContent();

        // Act
        sut.LoadContent();

        // Assert
        mockScene.Received(1).LoadContent();
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
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Name.Returns(nameof(mockSceneA));
        mockSceneA.Id.Returns(Guid.NewGuid());

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Name.Returns(nameof(mockSceneB));
        mockSceneB.Id.Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB);

        // Act
        sut.UnloadContent();

        // Assert
        mockSceneA.DidNotReceive().UnloadContent();
        mockSceneB.DidNotReceive().UnloadContent();
        sut.CurrentScene.Should().NotBeNull();
    }

    [Fact]
    public void UnloadContent_WhenContentIsLoadedAndManagerIsDisposed_DoesNotUnloadContentMoreThanOnce()
    {
        // Arrange
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Name.Returns(nameof(mockSceneA));
        mockSceneA.Id.Returns(Guid.NewGuid());

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Name.Returns(nameof(mockSceneB));
        mockSceneB.Id.Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB);
        sut.LoadContent();
        sut.Dispose();

        // Act
        sut.UnloadContent();

        // Assert
        mockSceneA.Received(1).UnloadContent();
        mockSceneB.Received(1).UnloadContent();
        sut.CurrentScene.Should().BeNull();
    }

    [Fact]
    public void UnloadContent_WhenContentIsLoadedAndManagerIsNotDisposed_DoesNotUnloadContentMoreThanOnce()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var expectedInActiveIds = new[] { sceneBId }.AsReadOnly();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Name.Returns(nameof(mockSceneA));
        mockSceneA.Id.Returns(sceneAId);

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Name.Returns(nameof(mockSceneB));
        mockSceneB.Id.Returns(sceneBId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA, setToActive: true);
        sut.AddScene(mockSceneB);
        sut.LoadContent();

        // Act
        sut.UnloadContent();

        // Assert
        mockSceneA.Received(1).UnloadContent();
        mockSceneB.Received(1).UnloadContent();
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
        var mockScene = Substitute.For<IScene>();
        mockScene.Id.Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockScene);

        var frameTime = new FrameTime
        {
            TotalTime = new TimeSpan(1, 2, 3, 4, 5),
            ElapsedTime = new TimeSpan(6, 7, 8, 9, 10),
        };

        // Act
        sut.Update(frameTime);

        // Assert
        mockScene.Received(1).Update(frameTime);
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
        var mockScene = Substitute.For<IScene>();
        mockScene.Id.Returns(Guid.NewGuid());

        var sut = new SceneManager();
        sut.AddScene(mockScene);

        // Act
        sut.Render();

        // Assert
        mockScene.Received(1).Render();
    }

    [Fact]
    public void SceneExists_WhenSceneDoesExist_ReturnsTrue()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);

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

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);

        // Act
        var actual = sut.SceneExists(Guid.NewGuid());

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Resize_WhenInvoked_UpdatesWindowSizeForEachScene()
    {
        // Arrange
        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(Guid.NewGuid());
        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(Guid.NewGuid());
        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB);

        // Act
        sut.Resize(new SizeU(15u, 15u));

        // Assert
        mockSceneA.Received(1).Resize(new SizeU(15u, 15u));
        mockSceneB.Received(1).Resize(new SizeU(15u, 15u));
    }

    [Fact]
    [SuppressMessage("csharpsquid", "S3966", Justification = "Disposing twice is required for testing.")]
    public void Dispose_WhenInvokedS_DisposesOfScenes()
    {
        // Arrange
        var sceneAId = Guid.NewGuid();
        var sceneBId = Guid.NewGuid();

        var mockSceneA = Substitute.For<IScene>();
        mockSceneA.Id.Returns(sceneAId);

        var mockSceneB = Substitute.For<IScene>();
        mockSceneB.Id.Returns(sceneBId);

        var sut = new SceneManager();
        sut.AddScene(mockSceneA);
        sut.AddScene(mockSceneB);

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        mockSceneA.Received(1).UnloadContent();
        mockSceneB.Received(1).UnloadContent();
        sut.CurrentScene.Should().BeNull();
    }
    #endregion
}
