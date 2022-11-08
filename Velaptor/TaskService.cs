// <copyright file="TaskService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Creates a new task for asynchronous operations to be performed.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class TaskService : ITaskService
{
    private readonly CancellationTokenSource tokenSrc = new ();
    private Task? internalTask;
    private bool isDisposed;

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

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            if (this.internalTask is not null)
            {
                // If the task is still running, stop it first then dispose
                if (this.internalTask.Status == TaskStatus.Running)
                {
                    Cancel();
                }

                if (this.internalTask.Status == TaskStatus.RanToCompletion ||
                    this.internalTask.Status == TaskStatus.Faulted ||
                    this.internalTask.Status == TaskStatus.Canceled)
                {
                    this.internalTask.Dispose();
                }
            }
        }

        this.isDisposed = true;
    }
}
