using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Microsoft.Support.Workflow.Activity
{
    /// <summary>
    /// Adding some class text to see if we can find this
    /// </summary>
   


    [Microsoft.Support.Workflow.Activity.IsCodeBeside(false)]
    [IsSwitch(false)]
    [IsUxActivity(false)]
    [IsToolBoxItem(true,ToolBoxDisplayName="A toolbox Item",ToolBoxTabName=ToolTabNames.Business,ToolBoxToolTip="testing the toolbox property")]
    [ContextList("ProductId",true,false,true)]
    [ContextList("SupportTopic",false,true,true)]
    [ContextList("Whatever",true,false,false)]
    [ContextList(typeof(Microsoft.Support.Workflow.Activity_Category),true,true,true)]
    public sealed class TestCodeActivity : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> ProductId { get; set; }
        [RequiredArgument]
        public OutArgument<string> SupportTopic { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            string text = context.GetValue(this.ProductId);
        }
    }
}
