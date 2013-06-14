// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.Common.Converters
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using Security;
    using BTV = Microsoft.Support.Workflow.Authoring.AddIns.Converters.BooleanToVisibilityConverter;

    /// <summary>
    /// False = Visible, True = Hidden or Collapsed (depends on CollapseWhenInvisible)
    /// </summary>
    public class BooleanToVisibilityConverter : BTV
    {
    }
}