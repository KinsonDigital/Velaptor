using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace RaptorTests.Helpers
{
    /// <summary>
    /// Provides helper methods for the <see cref="XUnit"/>'s <see cref="Assert"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AssertHelpers
    {
        #region Public Methods
        /// <summary>
        /// Verifies that the exact exception is thrown (and not a derived exception type) and that
        /// the exception message matches the given <paramref name="expectedMessage"/>.
        /// </summary>
        /// <typeparam name="T">The type of exception that the test is verifying.</typeparam>
        /// <param name="testCode">The code that will be be throwing the expected exception.</param>
        /// <param name="expectedMessage">The expected message of the exception.</param>
        public static void ThrowsWithMessage<T>(Action testCode, string expectedMessage) where T : Exception
        {
            Assert.Equal(expectedMessage, Assert.Throws<T>(testCode).Message);
        }

        public static void DoesNotThrow<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T)
            {
                Assert.True(false, $"Expected the exception {typeof(T).Name} to not be thrown.");
            }
        }

        public static void DoesNotThrowNullReference(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    Assert.True(false, $"Expected not to raise a {nameof(NullReferenceException)} exception.");
                }
                else
                {
                    Assert.True(true);
                }
            }
        }

        public static void IsNullOrZeroField(object fieldContainer, string name)
        {
            try
            {
                var result = fieldContainer.IsNullOrZeroField(name);

                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.Message);
            }
        }
        #endregion
    }
}
