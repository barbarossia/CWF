using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Authoring.AddIns.MultipleAuthor;
using Microsoft.Support.Workflow.Authoring.AddIns.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using System.Activities.Presentation.Model;

namespace Microsoft.Support.Workflow.Authoring.Tests.Services
{
    [TestClass]
    public class ModelItemServiceUnitTest
    {
        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        public void TestFindFlowchat()
        {
            Activity at = new Flowchart()
            {
                Nodes = 
                { 
                    new FlowStep 
                    {
                        Action = new TaskActivity() 
                        { 
                            TaskBody = new Sequence()
                            {
                            }
                        }, 
                    }                    
                }
            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var results = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType));

            Assert.IsTrue(results.Count() == 1);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        public void TestFindFlowchatSequence()
        {
            Activity at = new Flowchart()
            {
                Nodes = 
                { 
                    new FlowStep 
                    {
                        Action = new Sequence() 
                        { 
                            Activities = {new TaskActivity()},
                        }, 
                    }                    
                }
            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var results = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType));

            Assert.IsTrue(results.Count() == 1);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        public void TestFindSequenceFlowchat()
        {
            Activity at = new Sequence()
            {
                Activities =
                { 
                    new Flowchart 
                    {
                        Nodes =
                        {
                            new FlowStep 
                            {
                                Action = new TaskActivity() , 
                            }
                        }      
                    }
                }
            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var results = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType));

            Assert.IsTrue(results.Count() == 1);
        }

        [TestMethod]
        [TestCategory("Unit-NoDif")]
        [Owner("v-bobzh")]
        public void TestFindForeach()
        {
            Activity at = new Sequence()
            {
                Activities =
                { 
                    new ForEach<int> 
                    {
                        Body = new ActivityAction<int>
                        {
                            Argument = new DelegateInArgument<int>("i"),
                            Handler =  new TaskActivity(),
                        }
                    }
                }
            };

            var wfDesigner = TestUtilities.CreateWorkflowDesigner(at);
            var results = ModelItemService.Find(wfDesigner.GetRoot(), (modelItemType) => typeof(TaskActivity).IsAssignableFrom(modelItemType));

            Assert.IsTrue(results.Count() == 1);
        }
    }
}
