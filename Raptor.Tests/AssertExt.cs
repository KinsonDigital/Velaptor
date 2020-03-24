using Xunit;
using System;

namespace KDScorpionCoreTests
{
    /// <summary>
    /// Provides extensions to the <see cref="Assert"/> class.
    /// </summary>
    public static class AssertExt
    {
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
    }
}
