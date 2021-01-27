// <copyright file="CachedValueTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests
{
    using Moq;
    using Raptor;
    using Xunit;

    public class CachedValueTests
    {
        #region Prop Tests
        [Fact]
        public void IsCaching_WhenSettingToTrue_ReturnsCorrectResult()
        {
            // Arrange
            var cachedValue = new CachedValue<int>(
                defaultValue: 5678,
                getterWhenNotCaching: () => 1234,
                setterWhenNotCaching: (value) => { })
            {
                IsCaching = true,
            };

            // Act
            var actual = cachedValue.GetValue();

            // Assert
            Assert.Equal(1234, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void GetValue_WhileCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var cachedValue = new CachedValue<int>(
                defaultValue: 1234,
                getterWhenNotCaching: () => It.IsAny<int>(),
                setterWhenNotCaching: (value) => { });

            // Act
            var actual = cachedValue.GetValue();

            // Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void GetValue_WhileNotCachingValue_ReturnsCorrectResult()
        {
            // Arrange
            var cachedValue = new CachedValue<int>(
                defaultValue: 5678,
                getterWhenNotCaching: () => 1234,
                setterWhenNotCaching: (value) => { })
            {
                IsCaching = false,
            };

            // Act
            var actual = cachedValue.GetValue();

            // Assert
            Assert.Equal(1234, actual);
        }

        [Fact]
        public void SetValue_WhileCaching_ReturnsCorrectResult()
        {
            // Arrange
            var cachedValue = new CachedValue<int>(
                defaultValue: 5678,
                getterWhenNotCaching: () => 0,
                setterWhenNotCaching: (value) => { });

            // Act
            cachedValue.SetValue(1234);
            cachedValue.SetValue(5678);
            var actual = cachedValue.GetValue();

            // Assert
            Assert.Equal(5678, actual);
        }

        [Fact]
        public void SetValue_WhileNotCachingValue_InvokesOnResolve()
        {
            // Arrange
            var actual = 0;
            var cachedValue = new CachedValue<int>(
                defaultValue: 5678,
                getterWhenNotCaching: () => 0,
                setterWhenNotCaching: (value) => actual = value)
            {
                IsCaching = false,
            };

            // Act
            cachedValue.SetValue(1234);

            // Assert
            Assert.Equal(1234, actual);
        }
        #endregion
    }
}
