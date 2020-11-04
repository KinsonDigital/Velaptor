// <copyright file="ContentSourceFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Fakes
{
    using System.IO.Abstractions;
    using Raptor.Content;

    public class ContentSourceFake : ContentSource
    {
        public ContentSourceFake(string testContentDirName, IDirectory directory)
            : base(directory)
        {
            ContentDirectoryName = testContentDirName;
        }
    }
}
