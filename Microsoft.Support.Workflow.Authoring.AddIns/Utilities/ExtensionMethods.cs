﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Collections.Generic;
    
    /// <summary>
    /// Extension methods of the application
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Allows to remove all the elements in an observablecollection that comply with certain search criteria
        /// </summary>
        /// <typeparam name="T">Type of the observable collection elements</typeparam>
        /// <param name="collection">Source collection</param>
        /// <param name="condition">Condition that will be checked to remove the elements</param>
        /// <returns></returns>
        public static ObservableCollection<T> Remove<T>(this ObservableCollection<T> collection, Func<T, bool> condition)
        {
            var itemsToRemove = collection.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                collection.Remove(itemToRemove);
            }

            return collection;
        }
    }
}
