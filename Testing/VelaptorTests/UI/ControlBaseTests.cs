// <copyright file="ControlBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Velaptor;
    using Velaptor.Input;
    using Velaptor.UI;
    using VelaptorTests.Fakes;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="ControlBase"/> class.
    /// </summary>
    public class ControlBaseTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlBaseTests"/> class.
        /// </summary>
        public ControlBaseTests() => ClearMouseState();

        /// <summary>
        /// Gets the mouse test data.
        /// </summary>
        public static IEnumerable<object[]> MouseData =>
            new List<object[]>
            {
                //              xPos    yPos    mouseDown   expectedAlpha   expectedRed     expectedGreen       expectedBlue
                new object[] {   75,     75,      true,          255,            190,           190,                190 },
                new object[] {   75,    75,       false,         255,            230,           230,                230 },
                new object[] {   200,    75,      true,          255,            255,           255,                255 },
                new object[] {   200,    75,      false,         255,            255,           255,                255 },
            };

        #region Prop Tests
        [Fact]
        public void Name_WithDefaultValue_ReturnsCorrectResult()
        {
            // Act
            var ctrlBase = new ControlBaseFake();

            // Arrange
            var actual = ctrlBase.Name;

            // Act
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void Name_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var ctrlbase = new ControlBaseFake();
            ctrlbase.Name = "test-name";

            // Arrange
            var actual = ctrlbase.Name;

            // Assert
            Assert.Equal("test-name", actual);
        }

        [Fact]
        public void Position_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new Point(11, 22);
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Position = new Point(11, 22);

            // Act
            var actual = ctrlBase.Position;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Left_WhenSettingValue_PropsReturnCorrectResults()
        {
            // Arrange
            const int expected = 11;
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Left = 11;

            // Act
            var actualLeft = ctrlBase.Left;
            var actualPosX = ctrlBase.Position.X;

            // Assert
            Assert.Equal(expected, actualLeft);
            Assert.Equal(expected, actualPosX);
        }

        [Fact]
        public void Right_WhenSettingValue_PropsReturnCorrectResults()
        {
            // Arrange
            const int expected = 30;
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Width = 20;

            // Act
            ctrlBase.Right = 30;
            var actualRight = ctrlBase.Right;
            var actualPosX = ctrlBase.Position.X;

            // Assert
            Assert.Equal(expected, actualRight);
            Assert.Equal(10, actualPosX);
        }

        [Fact]
        public void Top_WhenSettingValue_PropsReturnCorrectResults()
        {
            // Arrange
            const int expected = 20;
            var ctrlBase = new ControlBaseFake();

            // Act
            ctrlBase.Top = 20;
            var actualTop = ctrlBase.Top;

            // Assert
            Assert.Equal(expected, actualTop);
        }

        [Fact]
        public void Bottom_WhenSettingValue_PropsReturnCorrectResults()
        {
            // Arrange
            const int expected = 30;
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Height = 20;
            ctrlBase.Bottom = 30;

            // Act
            var actualBottom = ctrlBase.Bottom;
            var positionY = ctrlBase.Position.Y;

            // Assert
            Assert.Equal(expected, actualBottom);
            Assert.Equal(10, positionY);
        }

        [Fact]
        public void Width_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            ctrlBase.Width = 11;
            var actual = ctrlBase.Width;

            // Assert
            Assert.Equal(11, actual);
        }

        [Fact]
        public void Height_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            ctrlBase.Height = 11;
            var actual = ctrlBase.Height;

            // Assert
            Assert.Equal(11, actual);
        }

        [Fact]
        public void Visible_WhenGettingDefaultValue_ReturnsTrue()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            var actual = ctrlBase.Visible;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Visible_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            ctrlBase.Visible = false;
            var actual = ctrlBase.Visible;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Enabled_WhenGettingDefaultValue_ReturnsTrue()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            var actual = ctrlBase.Enabled;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Enabled_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            ctrlBase.Enabled = false;
            var actual = ctrlBase.Enabled;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void MouseDownColor_WhenGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var control = new ControlBaseFake();

            // Act
            var actual = control.MouseDownColor;

            // Assert
            Assert.Equal(Color.FromArgb(255, 190, 190, 190), actual);
        }

        [Fact]
        public void MouseHoverColor_WhenGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var control = new ControlBaseFake();

            // Act
            var actual = control.MouseHoverColor;

            // Assert
            Assert.Equal(Color.FromArgb(255, 230, 230, 230), actual);
        }

        [Fact]
        public void MouseDownColor_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var control = new ControlBaseFake();

            // Act
            control.MouseDownColor = Color.FromArgb(11, 22, 33, 44);
            var actual = control.MouseDownColor;

            // Assert
            Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
        }

        [Fact]
        public void MouseHoverColor_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var control = new ControlBaseFake();

            // Act
            control.MouseHoverColor = Color.FromArgb(11, 22, 33, 44);
            var actual = control.MouseHoverColor;

            // Assert
            Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadContent_WhenInvokedBeforeAndAfterLoadingContent_SetContentAsLoaded()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();

            // Act
            var beforeLoad = ctrlBase.IsLoaded;
            ctrlBase.LoadContent();
            var afterLoad = ctrlBase.IsLoaded;

            Assert.False(beforeLoad);
            Assert.True(afterLoad);
        }

        [Theory]
        [MemberData(nameof(MouseData))]
        public void Update_WhenLeftMouseButtonIsDownOverControl_SetsTintColor(
            int xPos,
            int yPos,
            bool mouseDown,
            byte alpha,
            byte red,
            byte green,
            byte blue)
        {
            // Arrange
            var expected = Color.FromArgb(alpha, red, green, blue);

            var ctrlBase = new ControlBaseFake();
            ctrlBase.Position = new Point(50, 50);
            ctrlBase.Width = 100;
            ctrlBase.Height = 100;

            IMouseInput<MouseButton, MouseState>.XPos = xPos;
            IMouseInput<MouseButton, MouseState>.YPos = yPos;
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = mouseDown;

            ctrlBase.LoadContent();

            // Act
            ctrlBase.Update(default(FrameTime));

            // Assert
            Assert.Equal(alpha, ctrlBase.TintColorValue.A);
            Assert.Equal(red, ctrlBase.TintColorValue.R);
            Assert.Equal(green, ctrlBase.TintColorValue.G);
            Assert.Equal(blue, ctrlBase.TintColorValue.B);
        }

        [Fact]
        public void Update_WhenMouseMovesOverCtrl_InvokesMouseMoveEvent()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Position = new Point(50, 50);
            ctrlBase.Width = 100;
            ctrlBase.Height = 100;

            // Set previous mouse position
            IMouseInput<MouseButton, MouseState>.XPos = 75;
            IMouseInput<MouseButton, MouseState>.YPos = 75;

            ctrlBase.LoadContent();

            // Act
            ctrlBase.Update(default(FrameTime));

            // Set current mouse position
            IMouseInput<MouseButton, MouseState>.XPos = 80;
            IMouseInput<MouseButton, MouseState>.YPos = 80;

            // Assert
            Assert.Raises<MousePositionEventArgs>(
                (e) => ctrlBase.MouseMove += e,
                (e) => ctrlBase.MouseMove -= e,
                () =>
                {
                    ctrlBase.Update(default(FrameTime));
                });
        }

        [Fact]
        public void Update_WithMouseButtonDownAndOverCtrl_InvokesMouseDownEvent()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Position = new Point(50, 50);
            ctrlBase.Width = 100;
            ctrlBase.Height = 100;

            IMouseInput<MouseButton, MouseState>.XPos = 75;
            IMouseInput<MouseButton, MouseState>.YPos = 75;
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = true;

            ctrlBase.LoadContent();

            // Act
            ctrlBase.Update(default(FrameTime));

            // Assert
            Assert.Raises<EventArgs>(
                (e) => ctrlBase.MouseDown += e,
                (e) => ctrlBase.MouseDown -= e,
                () =>
                {
                    ctrlBase.Update(default(FrameTime));
                });
        }

        [Fact]
        public void Update_WithMouseButtonDownThenUpOverCtrl_InvokesMouseUpAndClickEvent()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Position = new Point(50, 50);
            ctrlBase.Width = 100;
            ctrlBase.Height = 100;

            var clickInvoked = false;

            void CtrlClicked(object? sender, EventArgs e)
            {
                clickInvoked = true;
            }

            void MouseUp(object? sender, EventArgs e)
            {
                Assert.False(clickInvoked, $"The '{nameof(ControlBase.MouseUp)}' event must be invoked before '{nameof(ControlBase.Click)}'");
            }

            ctrlBase.Click += CtrlClicked;
            ctrlBase.MouseUp += MouseUp;

            IMouseInput<MouseButton, MouseState>.XPos = 75;
            IMouseInput<MouseButton, MouseState>.YPos = 75;

            ctrlBase.LoadContent();

            // Set left mouse button down
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = true;

            // Act
            ctrlBase.Update(default(FrameTime));

            // Set left mouse button as up which is a full click
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = false;

            // Assert
            Assert.Raises<EventArgs>(
                (e) => ctrlBase.MouseUp += e,
                (e) => ctrlBase.MouseUp -= e,
                () =>
                {
                    ctrlBase.Update(default(FrameTime));
                });
        }

        [Fact]
        public void Update_WhenContentIsNotLoaded_DoesNotUpdateCtrl()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Position = new Point(50, 50);
            ctrlBase.Width = 100;
            ctrlBase.Height = 100;

            IMouseInput<MouseButton, MouseState>.XPos = 75;
            IMouseInput<MouseButton, MouseState>.YPos = 75;
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = true;

            // Act & Assert
            AssertExtensions.DoesNotRaise<EventArgs>(
                e => ctrlBase.MouseDown += e,
                e => ctrlBase.MouseDown -= e,
                () => ctrlBase.Update(default(FrameTime)));
        }

        [Fact]
        public void Update_WhenDisabled_DoesNotUpdateCtrl()
        {
            // Arrange
            var ctrlBase = new ControlBaseFake();
            ctrlBase.Enabled = false;
            ctrlBase.Position = new Point(50, 50);
            ctrlBase.Width = 100;
            ctrlBase.Height = 100;

            IMouseInput<MouseButton, MouseState>.XPos = 75;
            IMouseInput<MouseButton, MouseState>.YPos = 75;
            IMouseInput<MouseButton, MouseState>.InputStates[MouseButton.LeftButton] = true;

            ctrlBase.LoadContent();

            // Act & Assert
            AssertExtensions.DoesNotRaise<EventArgs>(
                e => ctrlBase.MouseDown += e,
                e => ctrlBase.MouseDown -= e,
                () => ctrlBase.Update(default(FrameTime)));
        }

        [Fact]
        public void UnloadContent_WhenInvoked_SetsControlAsUnloaded()
        {
            // Arrange
            var control = new ControlBaseFake();

            // Act
            control.UnloadContent();
            var actual = control.IsLoaded;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void ThrowExceptionIfLoadingWhenDisposed_WhenInvokedWhileDisposed_ThrowsException()
        {
            // Arrange
            var control = new ControlBaseFake();
            control.Dispose();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<Exception>(() =>
            {
                control.Invoke_Exception_In_Method_ThrowExceptionIfLoadingWhenDisposed();
            }, "Cannot load a control that has been disposed.");
        }

        [Fact]
        public void ThrowExceptionIfLoadingWhenDisposed_WhenInvokedWhileNotDisposed_ThrowsException()
        {
            // Arrange
            var control = new ControlBaseFake();

            // Act & Assert
            AssertExtensions.DoesNotThrow<Exception>(() =>
            {
                control.Invoke_Exception_In_Method_ThrowExceptionIfLoadingWhenDisposed();
            });
        }
        #endregion

        /// <summary>
        /// Clears the state for the mouse for testing purposes.
        /// </summary>
        private static void ClearMouseState()
        {
            IMouseInput<MouseButton, MouseState>.InputStates.Clear();
            IMouseInput<MouseButton, MouseState>.XPos = 0;
            IMouseInput<MouseButton, MouseState>.YPos = 0;
            IMouseInput<MouseButton, MouseState>.ScrollWheelValue = 0;
        }
    }
}
