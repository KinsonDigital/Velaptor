// <copyright file="SoundDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Audio
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Raptor.Audio;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="SoundData{T}"/> struct.
    /// </summary>
    public class SoundDataTests
    {
        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsPropertiesToCorrectValues()
        {
            // Act
            var data = new SoundData<float>(
                new[] { 1f },
                44100,
                2,
                AudioFormat.Stereo16,
                123);

            // Assert
            Assert.Equal(new[] { 1f }, data.BufferData.ToArray());
            Assert.Equal(44100, data.SampleRate);
            Assert.Equal(2, data.Channels);
            Assert.Equal(AudioFormat.Stereo16, data.Format);
            Assert.Equal(123, data.TotalSeconds);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Equals_WhenInvokingOverloadedEqualsOperator_ReturnsCorrectResult()
        {
            // Arrange
            var dataA = new SoundData<float>
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            var dataB = new SoundData<float>()
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            // Act
            var actual = dataA == dataB;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenInvokingOverloadedNoEqualsOperator_ReturnsCorrectResult()
        {
            // Arrange
            var dataA = new SoundData<float>
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            var dataB = new SoundData<float>()
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            // Act
            var actual = dataA != dataB;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenInvokingWithObjectThatIsNotSoundDataType_ReturnsCorrectResult()
        {
            // Arrange
            var dataA = new SoundData<float>
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            var dataB = new object();

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenInvokingWithObjectSoundDataType_ReturnsCorrectResult()
        {
            // Arrange
            var dataA = new SoundData<float>
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            object dataB = new SoundData<float>()
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(new[] { 1f }, 44100, 2, AudioFormat.Stereo16, 123, true)]
        [InlineData(new[] { 1f, 2f }, 6000, 2, AudioFormat.Mono16, 123, false)]
        public void Equals_WhenInvokingTypedParam_ReturnsCorrectResult(
            float[] bufferData,
            int sampleRate,
            int channels,
            AudioFormat format,
            float totalSeconds,
            bool expected)
        {
            // Arrange
            var dataA = new SoundData<float>
            {
                BufferData = new ReadOnlyCollection<float>(bufferData),
                SampleRate = sampleRate,
                Channels = channels,
                Format = format,
                TotalSeconds = totalSeconds,
            };

            var dataB = new SoundData<float>
            {
                BufferData = new ReadOnlyCollection<float>(new[] { 1f }),
                SampleRate = 44100,
                Channels = 2,
                Format = AudioFormat.Stereo16,
                TotalSeconds = 123,
            };

            // Act
            var actual = dataA.Equals(dataB);

            // Assert
            Assert.Equal(expected, actual);
        }
        #endregion
    }
}
