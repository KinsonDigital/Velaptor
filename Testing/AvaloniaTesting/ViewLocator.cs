// <copyright file="ViewLocator.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaTesting.ViewModels;

namespace AvaloniaTesting;

/// <summary>
/// Locates views.
/// </summary>
public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
