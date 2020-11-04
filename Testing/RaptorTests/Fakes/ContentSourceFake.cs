// <copyright file="ContentSourceFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Fakes
{
    using System.IO.Abstractions;
    using Raptor.Content;

    /// <summary>
    /// Used to test the abstract <see cref="ContentSource"/> class.
    /// </summary>
    public class ContentSourceFake : ContentSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSourceFake"/> class.
        /// </summary>
        /// <param name="testContentDirName">The content directory test name.</param>
        /// <param name="directory">The mocked directory.</param>
        public ContentSourceFake(string testContentDirName, IDirectory directory)
            : base(directory) => ContentDirectoryName = testContentDirName;
    }
}
