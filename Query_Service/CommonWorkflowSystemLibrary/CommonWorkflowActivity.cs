using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Windows.Markup;
using System.ComponentModel;
using System.Activities.XamlIntegration;

namespace Microsoft.Support.Workflow.Activity
{
    public abstract class CommonWorkflowActivity : System.Activities.Activity
    {

        protected override void CacheMetadata(ActivityMetadata metadata)
        {
            base.CacheMetadata(metadata);
        }


        [DefaultValue("")]
        [XamlDeferLoad(typeof(FuncDeferringLoader), typeof(System.Activities.Activity))]
        [Browsable(false)]
        [Ambient]
        protected override Func<System.Activities.Activity> Implementation
        {
            get
            {
                return base.Implementation;
            }
            set
            {
                base.Implementation = value;
            }
        }
    }
}
