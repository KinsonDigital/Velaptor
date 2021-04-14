// <copyright file="TaskService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Creates a new task for asynchronous operations to be performed.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly CancellationTokenSource tokenSrc = new CancellationTokenSource();
        private Task? internalTask;
        private bool isDiposed;

        /// <inheritdoc/>
        public CancellationTokenSource SetAction(Action action)
        {
            this.internalTask = new Task(action, this.tokenSrc.Token);

            return this.tokenSrc;
        }

        /// <inheritdoc/>
        public void Start()
        {
            if (this.internalTask is null)
            {
                throw new InvalidOperationException($"The task cannot be started until the '{nameof(SetAction)}' has been invoked.");
            }

            this.internalTask.Start();
        }

        /// <inheritdoc/>
        public Task ContinueWith(
            Action<Task> continuationAction,
            TaskContinuationOptions taskContinuationOptions,
            TaskScheduler scheduler)
        {
            if (this.internalTask is null)
            {
                throw new InvalidOperationException($"The task cannot be continued until the '{nameof(SetAction)}' has been invoked.");
            }

            return this.internalTask.ContinueWith(continuationAction, this.tokenSrc.Token, taskContinuationOptions, scheduler);
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            if (this.internalTask is null)
            {
                return;
            }

            if (this.internalTask.Status == TaskStatus.Running)
            {
                this.tokenSrc.Cancel();
                this.tokenSrc.Token.WaitHandle.WaitOne();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDiposed)
            {
                if (disposing)
                {
                    if (!(this.internalTask is null))
                    {
                        // If the task is still running, stop it first then dispose
                        if (this.internalTask.Status == TaskStatus.Running)
                        {
                            Cancel();
                            this.internalTask.Dispose();
                        }
                    }
                }

                this.isDiposed = true;
            }
        }
    }
}
