// <copyright file="EventLoggerServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services
{
    using System;
    using System.IO.Abstractions;
    using FluentAssertions;
    using Moq;
    using Velaptor;
    using Velaptor.Services;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="EventLoggerService"/> class.
    /// </summary>
    public class EventLoggerServiceTests
    {
        private readonly Mock<IDirectory> mockDir;
        private readonly Mock<IFile> mockFile;
        private readonly Mock<IConsoleService> mockConsoleService;
        private readonly Mock<IDateTimeService> mockDateTimeService;
        private readonly Mock<IAppSettingsService> mockAppSettingService;

        public EventLoggerServiceTests()
        {
            this.mockDir = new Mock<IDirectory>();
            this.mockFile = new Mock<IFile>();
            this.mockConsoleService = new Mock<IConsoleService>();
            this.mockDateTimeService = new Mock<IDateTimeService>();
            this.mockAppSettingService = new Mock<IAppSettingsService>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullDirectoryParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new EventLoggerService(
                    null,
                    this.mockFile.Object,
                    this.mockConsoleService.Object,
                    this.mockDateTimeService.Object,
                    this.mockAppSettingService.Object);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'directory')");
        }

        [Fact]
        public void Ctor_WithNullFileParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new EventLoggerService(
                    this.mockDir.Object,
                    null,
                    this.mockConsoleService.Object,
                    this.mockDateTimeService.Object,
                    this.mockAppSettingService.Object);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'file')");
        }

        [Fact]
        public void Ctor_WithNullConsoleServiceParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new EventLoggerService(
                    this.mockDir.Object,
                    this.mockFile.Object,
                    null,
                    this.mockDateTimeService.Object,
                    this.mockAppSettingService.Object);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'consoleService')");
        }

        [Fact]
        public void Ctor_WithNullDateTimeServiceParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new EventLoggerService(
                    this.mockDir.Object,
                    this.mockFile.Object,
                    this.mockConsoleService.Object,
                    null,
                    this.mockAppSettingService.Object);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'dateTimeService')");
        }

        [Fact]
        public void Ctor_WithNullAppSettingsServiceParam_ThrowsException()
        {
            // Arrange & Act
            var act = () =>
            {
                _ = new EventLoggerService(
                    this.mockDir.Object,
                    this.mockFile.Object,
                    this.mockConsoleService.Object,
                    this.mockDateTimeService.Object,
                    null);
            };

            // Assert
            act.Should()
                .Throw<ArgumentNullException>()
                .WithMessage("The parameter must not be null. (Parameter 'appSettingsService')");
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Event_WhenAllLoggingIsDisabled_DoesNotLogAnything()
        {
            // Arrange
            var appSettings = new AppSettings()
            {
                LoggingEnabled = false,
            };

            this.mockAppSettingService.SetupGet(p => p.Settings).Returns(appSettings);

            var sut = CreateService();

            // Act
            sut.Event("test-event", "event msg");

            // Assert
            this.mockDateTimeService.Verify(m => m.Now(), Times.Never);
            this.mockConsoleService.Verify(m => m.Write(It.IsAny<string>()), Times.Never);
            this.mockConsoleService.Verify(m => m.WriteLine(It.IsAny<string>()), Times.Never);
            this.mockDir.Verify(m => m.GetCurrentDirectory(), Times.Never);
            this.mockFile.Verify(m => m.Exists(It.IsAny<string>()), Times.Never);
            this.mockFile.Verify(m => m.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            this.mockFile.Verify(m => m.AppendAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Event_WithConsoleLoggingEnabled_LogsToConsole()
        {
            // Arrange
            MockTime(18, 36, 12);

            var sut = CreateService();

            var appSettings = new AppSettings()
            {
                LoggingEnabled = true,
                ConsoleLoggingEnabled = true,
                FileLoggingEnabled = false,
            };

            this.mockAppSettingService.SetupGet(p => p.Settings).Returns(appSettings);

            // Act
            sut.Event("test-event", "event msg");

            // Assert
            this.mockConsoleService.VerifyGet(p => p.ForegroundColor, Times.Once);
            this.mockConsoleService.VerifySet(p => p.ForegroundColor = ConsoleColor.DarkGray, Times.Exactly(3));
            this.mockConsoleService.VerifySet(p => p.ForegroundColor = ConsoleColor.Cyan, Times.Once);
            this.mockConsoleService.VerifySet(p => p.ForegroundColor = ConsoleColor.DarkCyan, Times.Once);
            this.mockConsoleService.VerifySet(p => p.ForegroundColor = ConsoleColor.White, Times.Exactly(2));

            this.mockConsoleService.Verify(m => m.Write("["), Times.Once);
            this.mockConsoleService.Verify(m => m.Write("18:36:12"), Times.Once);
            this.mockConsoleService.Verify(m => m.Write(" EVENT"), Times.Once);
            this.mockConsoleService.Verify(m => m.Write("("), Times.Once);
            this.mockConsoleService.Verify(m => m.Write("test-event"), Times.Once);
            this.mockConsoleService.Verify(m => m.Write(")"), Times.Once);
            this.mockConsoleService.Verify(m => m.Write("]"), Times.Once);

            this.mockConsoleService.Verify(m => m.Write(" event msg"), Times.Once);
        }

        [Fact]
        public void Event_WithFileLoggingEnabledAndFileDoesExist_LogsToFile()
        {
            // Arrange
            const string expectedTextToAppend = "[09:15:57 EVENT(test-event)] event msg";
            const string logsDirName = "logs";
            const string baseDirPath = "C:/app-dir";
            const string logFileName = "event-logs-20221024.txt";
            const string logFilePath = $"{baseDirPath}/{logsDirName}/{logFileName}";

            MockDateAndTime(2022, 10, 24, 9, 15, 57);
            this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            this.mockDir.Setup(m => m.GetCurrentDirectory()).Returns(baseDirPath);

            var sut = CreateService();
            var appSettings = new AppSettings()
            {
                LoggingEnabled = true,
                ConsoleLoggingEnabled = false,
                FileLoggingEnabled = true,
            };

            this.mockAppSettingService.SetupGet(p => p.Settings).Returns(appSettings);

            // Act
            sut.Event("test-event", "event msg");

            // Assert
            this.mockFile.Verify(m => m.Exists(logFilePath), Times.Once);
            this.mockFile.Verify(m => m.AppendAllText(logFilePath, expectedTextToAppend), Times.Once);
        }

        [Fact]
        public void Event_WithFileLoggingEnabledAndFileAlreadyExists_LogsToFile()
        {
            // Arrange
            var expectedTextToAppend = $"{Environment.NewLine}[09:15:57 EVENT(test-event)] event msg";
            const string logsDirName = "logs";
            const string baseDirPath = "C:/app-dir";
            const string logFileName = "event-logs-20221024.txt";
            const string logFilePath = $"{baseDirPath}/{logsDirName}/{logFileName}";

            MockDateAndTime(2022, 10, 24, 9, 15, 57);
            this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(true);
            this.mockDir.Setup(m => m.GetCurrentDirectory()).Returns(baseDirPath);

            var sut = CreateService();
            var appSettings = new AppSettings()
            {
                LoggingEnabled = true,
                ConsoleLoggingEnabled = false,
                FileLoggingEnabled = true,
            };

            this.mockAppSettingService.SetupGet(p => p.Settings).Returns(appSettings);

            // Act
            sut.Event("test-event", "event msg");

            // Assert
            this.mockFile.Verify(m => m.Exists(logFilePath), Times.Once);
            this.mockFile.Verify(m => m.AppendAllText(logFilePath, expectedTextToAppend), Times.Once);
        }

        [Fact]
        public void Event_WithFileLoggingEnabledAndFileDoesNotExist_LogsToFile()
        {
            // Arrange
            const string expectedTextToAppend = "[14:08:23 EVENT(test-event)] event msg";
            const string logsDirName = "logs";
            const string baseDirPath = "C:/app-dir";
            const string logFileName = "event-logs-20220902.txt";
            const string logFilePath = $"{baseDirPath}/{logsDirName}/{logFileName}";

            MockDateAndTime(2022, 09, 02, 14, 08, 23);
            this.mockFile.Setup(m => m.Exists(It.IsAny<string>())).Returns(false);
            this.mockDir.Setup(m => m.GetCurrentDirectory()).Returns(baseDirPath);

            var sut = CreateService();
            var appSettings = new AppSettings()
            {
                LoggingEnabled = true,
                ConsoleLoggingEnabled = false,
                FileLoggingEnabled = true,
            };

            this.mockAppSettingService.SetupGet(p => p.Settings).Returns(appSettings);

            // Act
            sut.Event("test-event", "event msg");

            // Assert
            this.mockFile.Verify(m => m.Exists(logFilePath), Times.Once);
            this.mockFile.Verify(m => m.WriteAllText(logFilePath, string.Empty));
            this.mockFile.Verify(m => m.AppendAllText(logFilePath, expectedTextToAppend), Times.Once);
        }
        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="EventLoggerService"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private EventLoggerService CreateService()
            => new (this.mockDir.Object,
                this.mockFile.Object,
                this.mockConsoleService.Object,
                this.mockDateTimeService.Object,
                this.mockAppSettingService.Object);

        /// <summary>
        /// Mocks the time using the given <paramref name="hour"/>, <paramref name="minute"/>, and <paramref name="second"/>.
        /// </summary>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        private void MockTime(int hour, int minute, int second)
        {
            this.mockDateTimeService.Setup(m => m.Now())
                .Returns(new DateTime(2022, 1, 2, hour, minute, second, 0));
        }

        /// <summary>
        /// Mocks the date and time using the given values.
        /// </summary>
        private void MockDateAndTime(int year, int month, int day, int hour, int minute, int second)
        {
            this.mockDateTimeService.Setup(m => m.Now())
                .Returns(new DateTime(year, month, day, hour, minute, second, 0));
        }
    }
}
