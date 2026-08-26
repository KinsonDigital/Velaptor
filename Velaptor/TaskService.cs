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
[ExcludeFromCodeCoverage(
    Justification =
        $"Cannot test due to direct interaction with the dotnet {nameof(System)}.{nameof(System.Threading)}.{nameof(System.Threading.Tasks)} API.")]
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
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left as 'public' for future use.")]
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
}
