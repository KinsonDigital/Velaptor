// <copyright file="PointerCounterTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests
{
    using System;
    using Raptor;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="PointerContainer"/> class.
    /// </summary>
    public class PointerCounterTests
    {
        #region Method Tests
        [Fact]
        public void PackPointerAndUnpackPointer_WhenInvoked_CorrectSetsPointer()
        {
            // Arrange
            var pointerContainer = new PointerContainer();
            var pointer = new IntPtr(1234);

            // Act
            pointerContainer.PackPointer(pointer);

            // Assert
            Assert.Equal(pointer, pointerContainer.UnpackPointer());
        }
        #endregion
    }
}
