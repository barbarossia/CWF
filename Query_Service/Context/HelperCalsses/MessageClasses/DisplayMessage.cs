using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Context.HelperClasses.MessageClasses
{
   [Serializable]
    public class DisplayMessage
    {

       private string _Message; 
       public string Messages
       {
       get{return _Message;}
       set{
           if(value != null)
                {_Message = value;}
           else
           {_Message= "Default _Message";}
          }
   }

      public  DisplayMessage(string str)
        {
            this.Messages = str;
        }
    }
}
