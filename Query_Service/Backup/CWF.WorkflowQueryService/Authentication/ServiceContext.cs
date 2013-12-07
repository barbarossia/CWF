// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowQueryServiceContext.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace CWF.WorkflowQueryService.Authentication
{
    using System.Collections.Generic;
    using System.ServiceModel;

    /// <summary>
    /// Service context class
    /// </summary>
    public class WorkflowQueryServiceContext : IExtension<OperationContext>
    {
            
        /// <summary>
        /// The current custom context
        /// </summary>
        public static WorkflowQueryServiceContext Current
        {
            get
            {
                WorkflowQueryServiceContext context = null;
                if (OperationContext.Current != null)
                {
                    context = OperationContext.Current.Extensions.Find<WorkflowQueryServiceContext>();
                    if (context == null)
                    {
                        context = new WorkflowQueryServiceContext();
                        OperationContext.Current.Extensions.Add(context);
                    }
                }

                return context;
            }
        }

        ///<summary>
        ///     Enables an extension object to find out when it has been aggregated. Called
        ///     when the extension is added to the System.ServiceModel.IExtensibleObject<T>.Extensions
        ///     property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</parameters>
        public void Attach(OperationContext owner)
        {
        }

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when
        /// an extension is removed from the System.ServiceModel.IExtensibleObject<T>.Extensions
        /// property.
        /// </summary>
        /// <param name="owner">The extensible object that aggregates this extension.</param>
        public void Detach(OperationContext owner)
        {
        }
    }
}