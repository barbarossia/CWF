using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Support.Workflow.Context.HelperClasses.MenuClasses;

namespace Microsoft.Support.Workflow.Context.HelperClasses.MenuClasses
{
    [Serializable]
    public class  MenuString

    {
       private string _menuString;
       public string MenuStrings { get{ return _menuString;} set{_menuString = value;} }

        
       public MenuString( string menuString)
       { 
           _menuString = menuString; 
       }
    }
}
