// <copyright file="ViewLocator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AvaloniaTesting;

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ViewModels;

/// <summary>
/// Locates views.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type is not null)
        {
            return (Control)Activator.CreateInstance(type) !;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
