// <copyright file="SceneManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Scene;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Shouldly;
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
        actual.ShouldBeSameAs(mockScene);
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
        actual.ShouldBeEquivalentTo(expected);
    }

    [Fact]
    public void IsLoaded_BeforeContentIsLoaded_ReturnsFalse()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var actual = sut.IsLoaded;

        // Assert
        actual.ShouldBeFalse();
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
        actual.ShouldBeTrue();
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
        actual.ShouldBe(2);
    }

    [Fact]
    public void CurrentSceneIndex_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var actual = sut.CurrentSceneIndex;

        // Assert
        actual.ShouldBe(0);
    }

    [Fact]
    public void UsesNavigationWrapping_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var actual = sut.UsesNavigationWrapping;

        // Assert
        actual.ShouldBeTrue();
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
        var exception = Should.Throw<Exception>(act);
        exception.Message.ShouldBe($"The scene 'test-name' with the ID '{sceneId}' already exists.");
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
        sut.CurrentScene.ShouldBeSameAs(mockSceneA);
        sut.CurrentSceneIndex.ShouldBe(0);
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
        sut.CurrentScene.ShouldBeSameAs(mockSceneB);
        sut.CurrentSceneIndex.ShouldBe(1);
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
        sut.CurrentScene.ShouldBeSameAs(mockSceneA);
        sut.CurrentSceneIndex.ShouldBe(0);
    }

    [Fact]
    public void RemoveScene_WithNoScenes_DoesNotThrowExceptionOrRemoveItems()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var act = () => sut.RemoveScene(Guid.NewGuid());

        // Assert
        Should.NotThrow(act);
        sut.CurrentScene.ShouldBeNull();
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
        sut.CurrentScene.ShouldNotBeNull();
        sut.SceneExists(sceneId).ShouldBeTrue();
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
        sut.CurrentScene.ShouldBeNull();
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
        sut.CurrentScene.ShouldNotBeNull();
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
        sut.CurrentScene.ShouldNotBeNull();
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
        sut.CurrentSceneIndex.ShouldBe(1);

        mockFirstScene.Received(1).UnloadContent();
        mockLastScene.Received(1).LoadContent();

        sut.CurrentScene.ShouldNotBeSameAs(mockFirstScene);
        sut.CurrentScene.ShouldBeSameAs(mockLastScene);
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
        sut.CurrentSceneIndex.ShouldBe(0);

        mockLastScene.Received(1).UnloadContent();
        mockFirstScene.Received(1).LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockFirstScene);
        sut.CurrentScene.ShouldNotBeSameAs(mockLastScene);
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
        sut.CurrentSceneIndex.ShouldBe(1);

        mockFirstScene.Received(1).UnloadContent();
        mockLastScene.Received(1).LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockLastScene);
        sut.CurrentScene.ShouldNotBeSameAs(mockFirstScene);
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
        sut.CurrentSceneIndex.ShouldBe(1);

        mockFirstScene.DidNotReceive().UnloadContent();
        mockLastScene.DidNotReceive().LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockLastScene);
        sut.CurrentScene.ShouldNotBeSameAs(mockFirstScene);
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
        sut.CurrentSceneIndex.ShouldBe(1);

        mockFirstScene.Received(1).UnloadContent();
        mockLastScene.Received(1).LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockLastScene);
        sut.CurrentScene.ShouldNotBeSameAs(mockFirstScene);
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
        sut.CurrentSceneIndex.ShouldBe(0);

        mockLastScene.Received(1).UnloadContent();
        mockFirstScene.Received(1).LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockFirstScene);
        sut.CurrentScene.ShouldNotBeSameAs(mockLastScene);
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
        sut.CurrentSceneIndex.ShouldBe(0);

        mockFirstScene.DidNotReceive().UnloadContent();
        mockLastScene.DidNotReceive().LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockFirstScene);
        sut.CurrentScene.ShouldNotBeSameAs(mockLastScene);
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
        sut.CurrentSceneIndex.ShouldBe(0);

        mockLastScene.Received(1).UnloadContent();
        mockFirstScene.Received(1).LoadContent();

        sut.CurrentScene.ShouldBeSameAs(mockFirstScene);
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

        // Act && Assert
        var exception = Should.Throw<SceneDoesNotExistException>(() => sut.SetSceneAsActive(id));
        exception.Message.ShouldBe($"The scene with the ID '{id.ToString()}' does not exist.");
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
        sut.InActiveScenes.ShouldBeEquivalentTo(expected);
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
        var exception = Should.Throw<ObjectDisposedException>(act);
        exception.Message.ShouldBe(expected);
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

        act.ShouldNotThrow();
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
        sut.CurrentScene.ShouldNotBeNull();
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
        sut.CurrentScene.ShouldBeNull();
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
        sut.CurrentScene.ShouldNotBeNull();
        sut.CurrentScene.Id.ShouldBe(sceneAId);
        sut.InActiveScenes.ShouldBeEquivalentTo(expectedInActiveIds);
    }

    [Fact]
    public void Update_WithNoScenes_DoesNotThrowException()
    {
        // Arrange
        var sut = new SceneManager();

        // Act
        var act = () => sut.Update(default);

        // Assert
        Should.NotThrow(act);
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
        Should.NotThrow(act);
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
        actual.ShouldBeTrue();
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
        actual.ShouldBeFalse();
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
        sut.CurrentScene.ShouldBeNull();
    }
    #endregion
}
