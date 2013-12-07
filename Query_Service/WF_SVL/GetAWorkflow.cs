using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Support.Workflow.Context;
using System.ServiceModel.Activities;
using System.Activities.XamlIntegration;

namespace WF_SVL
{

    public sealed class GetAWorkflow : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<CorrelationHandle> myHandle { get; set; }
        public InOutArgument<ContextObj> myContext {get; set;}

        

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            CorrelationHandle localHandle = context.GetValue(myHandle);
            ContextObj localObject = context.GetValue(myContext);
            
            Activity myActivity =   ActivityXamlServices.Load(@"C:\Users\v-evsmit\Documents\Projects\Msft CSS CWF\Visual Studio\WF_Svc_Context\WF_SVL\MyWorkflow.xaml");
        }
    }
}
