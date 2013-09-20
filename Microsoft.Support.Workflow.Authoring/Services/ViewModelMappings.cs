namespace Microsoft.Support.Workflow.Authoring.Services
{
    using System;
    using System.Collections.Generic;
    using ViewModels;
    using Views;
    using Microsoft.Support.Workflow.Authoring.ViewModels.Marketplace;

    /// <summary>
    /// Maps view models with their corresponding UI element (Typically views).
    /// This is used by the DialogService so the viewmodels and views are loosely coupled
    /// </summary>
    public static class ViewViewModelMappings
    {
        /// <summary>
        /// Dictionary that contains the mappings of viewmodels and views.
        /// </summary>
        private readonly static IDictionary<Type, Type> Mappings;

        /// <summary>
        /// Default constructor
        /// </summary>
        static ViewViewModelMappings()
        {
            Mappings = new Dictionary<Type, Type>
            {
                {typeof (AboutViewModel), typeof (AboutView)},
                {typeof (ImportAssemblyViewModel), typeof (ImportAssemblyView)},
                {typeof(MarketplaceViewModel),typeof(MarketplaceHomeView)},
                {typeof(MarketplaceAssetDetailsViewModel),typeof(MarketplaceDetailsView)},
                {typeof (NewWorkflowViewModel), typeof (NewWorkflowView)},
                {typeof (OpenWorkflowFromServerViewModel), typeof (OpenWorkflowFromServerView)},
                {typeof (ReviewActivityViewModel), typeof (ReviewActivityView)},
                {typeof (UploadAssemblyViewModel), typeof (UploadAssemblyView)},
                {typeof(ClickableMessageViewModel), typeof(ClickableMessage)},
                {typeof(SelectAssemblyAndActivityViewModel),typeof(SelectAssemblyAndActivityView)},
                {typeof(ManageWorkflowTypeViewModel),typeof(ManageWorkflowTypeView)},
                {typeof(SelectWorkflowViewModel),typeof(SelectWorkflowsView)},
                {typeof(SelectImportAssemblyViewModel),typeof( SelectImportAssemblyView)},
                {typeof(SearchTaskActivityViewModel),typeof(SearchTaskActivitiesView)},
                {typeof(ChangeAuthorViewModel),typeof(ChangeAuthor)},
                {typeof(CopyCurrentProjectViewModel),typeof(CopyCurrentProject)},
                {typeof(EnvironmentSecurityOptionsViewModel),typeof(EnvironmentSecurityOptions)},
                {typeof(MoveProjectViewModel),typeof(MoveProject)},
                {typeof(TenantSecurityOptionsViewModel),typeof(TenantSecurityOptions)},
                {typeof(ChangeAuthorSummaryViewModel),typeof(ChangeAuthorSummary)},
                {typeof(DefaultValueSettingsViewModel),typeof(DefaultValueSettingsView)},
            };
        }

        /// <summary>
        /// Searches in the mappings dictionary and returns the corresponding view of a viewmodel.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <returns></returns>
        public static Type GetViewTypeFromViewModelType(Type viewModelType)
        {
            return Mappings[viewModelType];
        }

    }
}
