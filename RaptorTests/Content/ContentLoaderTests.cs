using Moq;
using Raptor.Content;
using Raptor.Graphics;
using Raptor.Plugins;
using Raptor;
using Xunit;

namespace RaptorTests.Content
{
    /// <summary>
    /// Unit tests to test the <see cref="ContentLoader"/> class.
    /// </summary>
    public class ContentLoaderTests
    {
        //#region Private Fields
        //private IContentLoader _contentLoader;
        //private readonly Mock<IContentLoader> _mockCoreContentLoader = new Mock<IContentLoader>();
        //#endregion


        //#region Constructors
        //public ContentLoaderTests()
        //{
        //    var mockTexture = new Mock<ITexture>();
        //    var mockText = new Mock<IText>();

        //    _mockCoreContentLoader = new Mock<IContentLoader>();
        //    _mockCoreContentLoader.SetupProperty(m => m.ContentRootDirectory);
        //    _mockCoreContentLoader.SetupGet(m => m.GamePath).Returns("GamePath");
        //    _mockCoreContentLoader.Setup(m => m.Load<ITexture>(It.IsAny<string>())).Returns(() =>
        //    {
        //        return mockTexture.Object;
        //    });
        //    _mockCoreContentLoader.Setup(m => m.LoadText<IText>(It.IsAny<string>())).Returns(() =>
        //    {
        //        return mockText.Object;
        //    });

        //    _contentLoader = _mockCoreContentLoader.Object;
        //}
        //#endregion


        //#region Constructor Tests
        //[Fact]
        //public void Ctor_WhenInvoking_SetsInternalLoader()
        //{
        //    //Arrange
        //    var loader = new ContentLoader(_contentLoader);
        //    var expected = "RootDir";

        //    //Act
        //    loader.ContentRootDirectory = "RootDir";
        //    var actual = loader.ContentRootDirectory;

        //    //Assert
        //    Assert.Equal(expected, actual);
        //}
        //#endregion


        //#region Prop Tests
        //[Fact]
        //public void GamePath_WhenGettingValue_ReturnsCorrectValue()
        //{
        //    //Arrange
        //    var loader = new ContentLoader(_contentLoader);
        //    var expected = "GamePath";

        //    //Act
        //    var actual = loader.GamePath;

        //    //Assert
        //    Assert.Equal(expected, actual);
        //}

        //[Fact]
        //public void ContentRootDirectory_WhenGettingAndSettingValue_ReturnsCorrectValue()
        //{
        //    //Arrange
        //    var loader = new ContentLoader(_contentLoader);
        //    var expected = "MyRootDir";

        //    //Act
        //    loader.ContentRootDirectory = "MyRootDir";
        //    var actual = loader.ContentRootDirectory;

        //    //Assert
        //    Assert.Equal(expected, actual);
        //}
        //#endregion


        //#region Public Methods
        //[Fact]
        //public void LoadTexture_WhenInvoked_ReturnsTexture()
        //{
        //    //Arrange
        //    var loader = new ContentLoader(_contentLoader);

        //    //Act
        //    var actual = loader.LoadTexture("TextureName");

        //    //Assert
        //    Assert.NotNull(actual);
        //    _mockCoreContentLoader.Verify(m => m.Load<ITexture>(It.IsAny<string>()), Times.Once());
        //}


        //[Fact]
        //public void LoadText_WhenInvoked_ReturnsText()
        //{
        //    //Arrange
        //    var loader = new ContentLoader(_contentLoader);

        //    //Act
        //    var actual = loader.LoadText("TextName");

        //    //Assert
        //    Assert.NotNull(actual);
        //    _mockCoreContentLoader.Verify(m => m.LoadText<IText>(It.IsAny<string>()), Times.Once());
        //}
        //#endregion
    }
}
