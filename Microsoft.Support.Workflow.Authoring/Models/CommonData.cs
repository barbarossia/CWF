using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWF.DataContracts;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Reflection;
using Microsoft.Support.Workflow.Authoring.Views;
using System.Windows;


namespace Microsoft.Support.Workflow.Authoring.Models
{
    public class CommonData
    {
        private static CommonData instance;

        public bool IsInitialized { get; set; }

        public static CommonData Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new CommonData();                    
                }               
                return instance;
            }
        }

        private CommonData()
        {
            StatusCodes = new ObservableCollection<string>();
            WorkflowTypes = new List<WorkflowTypesGetBase>();
            ActivityCategories = new ObservableCollection<string>();
        }

        public void Initialize()
        {
            ActivityCategories.Clear();
            StatusCodes.Clear();
          
            Utility.WithContactServerUI(() => WorkflowsQueryServiceUtility.UsingClient(LoadLiveData));
            IsInitialized = true;            
        }

        private void LoadLiveData(IWorkflowsQueryService client)
        {
            WorkflowTypes = client.WorkflowTypeGet(new WorkflowTypesGetRequestDC().SetIncaller()).WorkflowActivityType;
            
            var categories = client.ActivityCategoryGet(new ActivityCategoryByNameGetRequestDC().SetIncaller());
            ActivityCategories.Assign(from category in categories select category.Name);

            var status = client.StatusCodeGet(new StatusCodeGetRequestDC().SetIncaller()).List;
            StatusCodes.Assign(from item in status select item.Name);
                        
        }

        public ObservableCollection<string> StatusCodes
        {
            get; private set;
        }

        public List<WorkflowTypesGetBase> WorkflowTypes
        {
            get; private set;
        }

        public ObservableCollection<string> ActivityCategories
        {
            get;
            private set;
        }
       
    }
}
