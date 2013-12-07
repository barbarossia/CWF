using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Statements;

namespace Microsoft.Support.Workflow.Activity
{
    public class BranchValue
    {
        public string TestValue { get; set; }
        public int BranchNumber {get; set;}
    }


    public class BranchOnContext : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<xxContextItem> ContextItem { get; set; }
        

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            xxContextItem contextItem = context.GetValue(this.ContextItem);

            FlowSwitch<xxContextItem> tada = new FlowSwitch<xxContextItem>();


        }
    }
}
