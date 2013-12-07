using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Activities;
using System.ComponentModel;
using System.Activities.Presentation;
using System.ServiceModel;


namespace Microsoft.Support.Workflow.Activity
{
    [IsToolBoxItem(false)]
    [IsSwitch(false)]
    [IsUxActivity(false)]
    [IsCodeBeside(true,HasXamlCode=true)]
    [Designer(typeof(TestCodeBesideDesigner))]
    public partial class TestCodeBeside
    {
        bool published = false;
        public bool IsPublished
        {
            get
            {
                return published;
            }
            set
            {
                published = value;
            }
        }
    }
}
