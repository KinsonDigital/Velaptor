// <copyright file="SoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Velaptor.Content;
    using Velaptor.Guards;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates sounds based on the sound file at a location.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class SoundFactory : ISoundFactory
    {
        private static readonly Dictionary<uint, string> Sounds = new ();
        private readonly IDisposable disposeSoundUnsubscriber;
        private readonly IDisposable shutDownUnsubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundFactory"/> class.
        /// </summary>
        /// <param name="disposeSoundReactable">Sends push notifications to dispose of sounds.</param>
        /// <param name="shutDownReactable">Sends a push notifications that the application is shutting down.</param>
        public SoundFactory(
            IReactable<DisposeSoundData> disposeSoundReactable,
            IReactable<ShutDownData> shutDownReactable)
        {
            EnsureThat.ParamIsNotNull(disposeSoundReactable);
            EnsureThat.ParamIsNotNull(shutDownReactable);

            this.disposeSoundUnsubscriber =
                disposeSoundReactable.Subscribe(new Reactor<DisposeSoundData>(RemoveSoundId));

            this.shutDownUnsubscriber =
                shutDownReactable.Subscribe(new Reactor<ShutDownData>(_ => ShutDown()));
        }

        /// <summary>
        /// Gets a new unique sound ID.
        /// </summary>
        /// <param name="filePath">The file path to the sound.</param>
        /// <returns>The new ID for a sound.</returns>
        public static uint GetNewId(string filePath)
        {
            var newId = Sounds.Count <= 0
                ? 1
                : Sounds.Keys.Max() + 1;

            Sounds.Add(newId, filePath);

            return newId;
        }

        /// <inheritdoc/>
        public ISound Create(string filePath)
        {
            var disposeReactor = IoC.Container.GetInstance<IReactable<DisposeSoundData>>();

            var newId = Sounds.Count <= 0
                ? 1
                : Sounds.Keys.Max() + 1;

            Sounds.Add(newId, filePath);

            return new Sound(disposeReactor, filePath, newId);
        }

        /// <summary>
        /// Removes a sound ID that matches the sound ID in the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The sounds data used to remove the ID from the list.</param>
        private static void RemoveSoundId(DisposeSoundData data)
        {
            if (Sounds.ContainsKey(data.SoundId))
            {
                Sounds.Remove(data.SoundId);
            }
        }

        /// <summary>
        /// Unsubscribes from the <see cref="IReactable{T}"/> reactors.
        /// </summary>
        private void ShutDown()
        {
            this.disposeSoundUnsubscriber.Dispose();
            this.shutDownUnsubscriber.Dispose();
        }
    }
}
