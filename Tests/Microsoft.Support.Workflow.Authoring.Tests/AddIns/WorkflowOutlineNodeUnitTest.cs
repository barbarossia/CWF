using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Activities.Statements;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.Activities;
using System.Activities.Expressions;

namespace Microsoft.Support.Workflow.Authoring.Tests.AddIns
{
    [TestClass]
    public class WorkflowOutlineNodeUnitTest
    {
        [TestMethod]
        [TestCategory("Unittest")]
        [Owner("v-kason")]
        public void WorkflowOutlineNode_Test()
        {
            Sequence model = new Sequence()
            {
                DisplayName = "s0",
                Activities = 
                            {
                                new WriteLine(){DisplayName="w0"},
                                new Switch<int>()
                                {
                                    DisplayName = "Verify Value from User",
                                    Cases = 
                                    {
                                        {2,null},
                                        { 0,new WriteLine()},
                                        {  1, new WriteLine() { Text = "    Try a lower number number..." } }, 
                                        { -1, new WriteLine() { Text = "    Try a higher number" } }
                                    }
                                },
                                CreateFlowchartWithFaults("MNK",0),
                                new Pick()
                                { 
                                    DisplayName="p0",
                                    Branches=
                                    {
                                        new PickBranch(){DisplayName="p1"},
                                        new PickBranch(){DisplayName="p2"},
                                    }
                                },
                                new ForEach<Int32>()
                                {
                                    Body = new ActivityAction<int>()
                                },
                                new ForEach<Int32>()
                                {
                                    Body = new System.Activities.ActivityAction<int>()
                                    {Handler = new WriteLine()},
                                },
                            },
            };

            EditingContext context = new EditingContext();
            ModelTreeManager mtm = new ModelTreeManager(context);
            mtm.Load(model);
            ModelItem root = mtm.Root;
            WorkflowOutlineNode wfo = new WorkflowOutlineNode(root);
            Assert.AreEqual(wfo.NodeName, "s0");
            Assert.AreEqual(wfo.ToString(), "s0");
            Assert.AreEqual(wfo.ActivityType, typeof(Sequence));
        }

        private static Activity CreateFlowchartWithFaults(string promoCode, int numKids)
        {
            Variable<string> promo = new Variable<string> { Default = promoCode };
            Variable<int> numberOfKids = new Variable<int> { Default = numKids };
            Variable<double> discount = new Variable<double>();
            DelegateInArgument<DivideByZeroException> ex = new DelegateInArgument<DivideByZeroException>();

            FlowStep discountNotApplied = new FlowStep
            {
                Action = new WriteLine
                {
                    DisplayName = "WriteLine: Discount not applied",
                    Text = "Discount not applied"
                },
                Next = null
            };

            FlowStep discountApplied = new FlowStep
            {
                Action = new WriteLine
                {
                    DisplayName = "WriteLine: Discount applied",
                    Text = "Discount applied "
                },
                Next = null
            };

            FlowDecision flowDecision = new FlowDecision
            {
                Condition = ExpressionServices.Convert<bool>((ctx) => discount.Get(ctx) > 0),
                True = discountApplied,
                False = discountNotApplied
            };

            FlowStep singleStep = new FlowStep
            {
                Action = new Assign
                {
                    DisplayName = "discount = 10.0",
                    To = new OutArgument<double>(discount),
                    Value = new InArgument<double>(10.0)
                },
                Next = flowDecision
            };

            FlowStep mnkStep = new FlowStep
            {
                Action = new Assign
                {
                    DisplayName = "discount = 15.0",
                    To = new OutArgument<double>(discount),
                    Value = new InArgument<double>(15.0)
                },
                Next = flowDecision
            };

            FlowStep mwkStep = new FlowStep
            {
                Action = new TryCatch
                {
                    DisplayName = "Try/Catch for Divide By Zero Exception",
                    Try = new Assign
                    {
                        DisplayName = "discount = 15 + (1 - 1/numberOfKids)*10",
                        To = new OutArgument<double>(discount),
                        Value = new InArgument<double>((ctx) => (15 + (1 - 1 / numberOfKids.Get(ctx)) * 10))
                    },
                    Catches = 
                    {
                         new Catch<System.DivideByZeroException>
                         {
                             Action = new ActivityAction<System.DivideByZeroException>
                             {
                                 Argument = ex,
                                 DisplayName = "ActivityAction - DivideByZeroException",
                                 Handler =
                                     new Sequence
                                     {
                                         DisplayName = "Divide by Zero Exception Workflow",
                                         Activities =
                                         {
                                            new WriteLine() 
                                            { 
                                                DisplayName = "WriteLine: DivideByZeroException",
                                                Text = "DivideByZeroException: Promo code is MWK - but number of kids = 0" 
                                            },
                                            new Assign<double>
                                            {
                                                DisplayName = "Exception - discount = 0", 
                                                To = discount,
                                                Value = new InArgument<double>(0)
                                            }
                                         }
                                     }
                             }
                         }
                    }
                },
                Next = flowDecision
            };

            FlowStep discountDefault = new FlowStep
            {
                Action = new Assign<double>
                {
                    DisplayName = "Default discount assignment: discount = 0",
                    To = discount,
                    Value = new InArgument<double>(0)
                },
                Next = flowDecision
            };


            FlowSwitch<string> promoCodeSwitch = new FlowSwitch<string>
            {
                Expression = promo,
                Cases =
                {
                   { "Single", singleStep },
                   { "MNK", mnkStep },
                   { "MWK", mwkStep },
                   {"as", new FlowStep()
                         { 
                             Action= new ForEach<Int32>()
                             {
                                Body = new System.Activities.ActivityAction<int>()
                                {Handler = new WriteLine()},
                             }
                         }
                    },
                    {"as1",new FlowStep()
                                      {
                                             Action =new Pick()
                                                    { 
                                                        DisplayName="p0",
                                                        Branches=
                                                        {
                                                            new PickBranch(){DisplayName="p1"},
                                                            new PickBranch(){DisplayName="p2"},
                                                        }
                                                    }
                                      }
                  }
                },
                Default = discountDefault
            };

            Flowchart flowChart = new Flowchart
            {
                DisplayName = "Promotional Discount Calculation",
                Variables = { discount, promo, numberOfKids },
                StartNode = promoCodeSwitch,
                Nodes = 
                { 
                    promoCodeSwitch, 
                    singleStep, 
                    mnkStep, 
                    mwkStep, 
                    discountDefault, 
                    flowDecision, 
                    discountApplied, 
                    discountNotApplied,
                   
                }
            };
            return flowChart;
        }
    }
}
