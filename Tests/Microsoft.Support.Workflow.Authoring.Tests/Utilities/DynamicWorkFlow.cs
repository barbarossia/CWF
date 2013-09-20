using System;
using System.Linq;
using System.Text;
using System.Xaml;
using System.Activities;
using CWF.DataContracts;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using AuthoringToolTests.Services;
using System.Activities.Statements;
using System.Activities.Expressions;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.Activities;
using Microsoft.DynamicImplementations;
using Microsoft.Support.Workflow.Authoring.Models;
using Microsoft.Support.Workflow.Authoring.Services;
using Microsoft.Support.Workflow.Authoring.Tests.TestInputs;
using TestInput_Lib2;

namespace Microsoft.Support.Workflow.Authoring.Tests
{   
    /// <summary>
    /// Dynamic workFlow generation
    /// </summary>
    public class DynamicWorkFlow
    {
        private Sequence newWorkFlow;

        /// <summary>
        /// Constructor
        /// </summary>
        public Sequence GetNewWorkFlow
        {
            get
            {
                return newWorkFlow;
            }
        }

        private Sequence IntActivity
        {
            get
            {
                Variable<int> n = new Variable<int> { Name = "n" };

                var wf = new Sequence
                {
                    Variables = { n },
                    Activities =
                    {
                        new Assign<int>
                        {
                            To = n,
                            Value = new Random().Next(100, 100000)
                        },
                        new WriteLine
                        {
                            Text = string.Format("Actvity build with Int DataType {0}", TestUtilities.GenerateRandomString(20))
                        }
                    }
                };
                return wf;
            }
        }

        private Sequence DoubleActivity
        {
            get
            {
                Variable<double> d = new Variable<double> { Name = "d" };
                var wf = new Sequence
                {
                    Variables = { d },
                    Activities =
                    {
                        new Assign<double>
                        {
                            To = d,
                            Value = new Random().NextDouble()
                        },
                        new WriteLine
                        {
                            Text = string.Format("Actvity build with Doulbe DataType {0}", TestUtilities.GenerateRandomString(20))
                        }
                    }
                };
                return wf;
            }
        }

        private Sequence StringActivity
        {
            get
            {
                Variable<string> s = new Variable<string> { Name = "s" };

                var wf = new Sequence
                {
                    Variables = { s },
                    Activities =
                    {
                        new Assign<string>
                        {
                            To = s,
                            Value = TestUtilities.GenerateRandomString(500)
                        },
                        new WriteLine
                        {
                            Text = string.Format("Actvity build with StringActivity DataType {0}", TestUtilities.GenerateRandomString(20))
                        }
                    }
                };
                return wf;
            }
        }

        private Sequence GenerateDynamicWorkFlow
        { 
            get
            {
                Variable<int> n = new Variable<int> { Name = "n" };
                Variable<double> d = new Variable<double> { Name = "d" };
                Variable<string> s = new Variable<string> { Name = "s" };
                
                var wf = new Sequence
                {
                    Variables = { s, n, d },
                    Activities =
                    {
                        new Assign<string>
                        {
                            To = s,
                            Value = TestUtilities.GenerateRandomString(500)
                        },
                        new Assign<int>
                        {
                            To = n,
                            Value = new Random().Next(222222, 999999)
                        },
                        new Assign<double>
                        {
                            To = d,
                            Value = new Random().NextDouble()
                        },
                        new WriteLine
                        {
                            Text = string.Format("Actvity build with Multiple DataType {0}", TestUtilities.GenerateRandomString(50))
                        }
                    }
                };

                return wf;
            }
        }
        
        public DynamicWorkFlow(WorkFlowDataTypes dataType)
        {
            //Intialize
            newWorkFlow = new Sequence();

            switch (dataType)
            { 
                case WorkFlowDataTypes.Int:
                    newWorkFlow = IntActivity;
                    break;
                case WorkFlowDataTypes.Double:
                    newWorkFlow = DoubleActivity;
                    break;
                case WorkFlowDataTypes.String:
                    newWorkFlow = StringActivity;
                    break;
                case WorkFlowDataTypes.Dynamic:
                    newWorkFlow = GenerateDynamicWorkFlow;
                    break;
                default:
                    newWorkFlow = GenerateDynamicWorkFlow;
                    break;
            }           
        }

        /// <summary>
        /// Generate randow workflow with different data type
        /// </summary>
        public static WorkFlowDataTypes AutoDynamicWorkFlow
        {
            //Intialize
            get
            {
                WorkFlowDataTypes wfDataTypes = WorkFlowDataTypes.Int;
                int n = new Random().Next(0, 4);

                switch (n)
                {
                    case 0:
                        wfDataTypes = WorkFlowDataTypes.Int;
                        break;
                    case 1:
                        wfDataTypes = WorkFlowDataTypes.Double;
                        break;
                    case 2:
                        wfDataTypes = WorkFlowDataTypes.String;
                        break;
                    case 3:
                        wfDataTypes = WorkFlowDataTypes.Dynamic;
                        break;
                }
                return wfDataTypes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        public static WorkflowItem CreateDependentWorkFlow(string name, string displayName)
        {
            WorkflowItem workFlowTemplet = null;
            int random = new Random().Next(0, 3);
            switch (random)
            {
                case 0:
                    var act1 = new Testinput_Lib1.Activity1();
                    workFlowTemplet = new WorkflowItem(name, displayName, act1.ToXaml(), "workflow");
                    break;
                case 1:
                    var act2 = new TestInput_Lib2.Activity1();
                    workFlowTemplet = new WorkflowItem(name, displayName, act2.ToXaml(), "workflow");
                    break;
                case 2:
                    var act3 = new Testinput_Lib3.Activity1();
                    workFlowTemplet = new WorkflowItem(name, displayName, act3.ToXaml(), "workflow");
                    break;
            }
            return workFlowTemplet;
        }
    }

    /// <summary>
    /// WorkFlow Data Types
    /// </summary>
    public enum WorkFlowDataTypes
    {
        Int,
        Double,
        String,
        Dynamic
    }

}
