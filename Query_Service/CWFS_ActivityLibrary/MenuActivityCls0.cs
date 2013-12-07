using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;




namespace Microsoft.Support.Workflow.Activity
{
    [Designer(typeof(Microsoft.Support.Workflow.Activity.DisplayMenuDesigner))]
    [ToolboxItemFilter("CWFS")]
    //[DesignerCategory("GenericActivity")]
    [DesignerCategory("GenericActivity")]
    public class MenuActivity : MenuActivityX
    {
        public MenuActivity()
        {
            //todo replace inheritence of menuactivity with getting xaml from database????
            
        }
    }
}