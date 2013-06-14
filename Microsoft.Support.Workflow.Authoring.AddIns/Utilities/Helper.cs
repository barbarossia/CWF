using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Activities.Presentation;
using System.Activities;
using Microsoft.Support.Workflow.Authoring.AddIns.Models;
using System.ServiceModel.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    public static class Helper
    {
        /// <summary>
        /// Helper method which encapsulates the knowledge for extracting Implementation from a WorkflowDesigner
        /// </summary>
        /// <param name="workflowDesigner"></param>
        /// <returns>Target Activity</returns>
        public static Activity GetActivityFromDesigner(WorkflowDesigner workflowDesigner)
        {
            object root = workflowDesigner.Context.Services.GetService<ModelService>().Root.GetCurrentValue();
            var activityBuilder = root as ActivityBuilder;
            if (activityBuilder != null)
            {
                return (activityBuilder).Implementation;
            }
            var workflowService = root as WorkflowService;
            if (workflowService != null)
            {
                return (workflowService).Body;
            }
            throw new InvalidOperationException("Can't get activity from designer");
        }

        /// <summary>
        /// Eliminate chained nullchecks for expressions a la LINQ "Select"
        /// </summary>
        public static TResult IfNotNull<T, TResult>(this T val, Func<T, TResult> select)
                where T : class
        {
            return val == null ? default(TResult) : @select(val);
        }

        /// <summary>
        /// Eliminate chained nullchecks for statements a la LINQ
        /// </summary>
        public static void IfNotNull<T>(this T val, Action<T> action)
                where T : class
        {
            if (val != null)
                action(val);
        }
        /// <summary>
        /// Translate generic activity class name to readable name. For example from Add `3 to Add T1,T2,T3
        /// </summary>
        /// <param name="activityTypeName">
        /// Activity type name
        /// </param>
        /// <returns>
        /// The translate activity type name.
        /// </returns>
        public static string TranslateActivityTypeName(string activityTypeName)
        {
            int position = activityTypeName.IndexOf('`');

            if (position < 0)
            {
                return activityTypeName;
            }

            string className = activityTypeName.Substring(0, position);
            string number = activityTypeName.Substring(position + 1);
            int x = int.Parse(number);
            var strArr = new string[x];

            for (int i = 0; i < x; i++)
            {
                strArr[i] = string.Format("T{0}", i + 1);
            }

            return string.Format("{0} <{1}>", className, string.Join(",", strArr));
        }

        /// <summary>
        /// Get all built in activity types in System.Activities.dll
        /// </summary>
        /// <returns>
        /// All activity types in this assembly.
        /// </returns>
        public static List<Type> GetAllVisibleBuiltInActivityTypes()
        {
            // Load activities from System.Activities assembly
            Assembly activitiesAssembly = typeof(Activity).Assembly;

            List<Type> allSystemActivitiesTypes = GetAllActivityTypesInAssembly(activitiesAssembly);

            var usefulNamespace = new List<string> { "System.Activities.Statements", "System.Activities.Expressions", };

            var filter = new List<string>
                {
                    ////"AbortInstanceFlagValidator",
                    "Activity", 
                    "Activity`1", 
                    "ActivityWithResult", 
                    "ActivityWithResultWrapper`1", 
                    "Add`3", 
                    "AddToCollection`1", 
                    "AddValidationError", 
                    "And`3", 
                    "AndAlso", 
                    "ArgumentReference`1", 
                    "ArgumentValue`1", 
                    "ArrayItemReference`1", 
                    "ArrayItemValue`1", 
                    "As`2", 
                    "AssertValidation", 
                    "Assign", 
                    "Assign`1", 
                    "AsyncCodeActivity", 
                    "AsyncCodeActivity`1", 
                    "CancellationScope", 
                    "Cast`2", 
                    "ClearCollection`1", 
                    "CodeActivity", 
                    "CodeActivity`1", 
                    "CompensableActivity", 
                    "Compensate", 
                    ////"CompensationParticipant",
                    "Confirm", 
                    "Constraint", 
                    "Constraint`1", 
                    "CreateBookmarkScope", 
                    ////"DefaultCompensation",
                    ////"DefaultConfirmation",
                    "Delay", 
                    "DelegateArgumentReference`1", 
                    "DelegateArgumentValue`1", 
                    "DeleteBookmarkScope", 
                    "Divide`3", 
                    "DoWhile", 
                    "DynamicActivity", 
                    "DynamicActivity`1", 
                    "EmptyDelegateActivity", 
                    "Equal`3", 
                    "ExistsInCollection`1", 
                    "FieldReference`2", 
                    "FieldValue`2", 
                    "Flowchart", 
                    "ForEach`1", 
                    "GetChildSubtree", 
                    "GetParentChain", 
                    "GetWorkflowTree", 
                    "GreaterThan`3", 
                    "GreaterThanOrEqual`3", 
                    "HandleScope`1", 
                    "If", 
                    "IndexerReference`2", 
                    ////"InternalCompensate",
                    ////"InternalConfirm",
                    "InvokeAction", 
                    "InvokeAction`1", 
                    "InvokeAction`10", 
                    "InvokeAction`11", 
                    "InvokeAction`12", 
                    "InvokeAction`13", 
                    "InvokeAction`14", 
                    "InvokeAction`15", 
                    "InvokeAction`16", 
                    "InvokeAction`2", 
                    "InvokeAction`3", 
                    "InvokeAction`4", 
                    "InvokeAction`5", 
                    "InvokeAction`6", 
                    "InvokeAction`7", 
                    "InvokeAction`8", 
                    "InvokeAction`9", 
                    "InvokeDelegate", 
                    "InvokeFunc`1", 
                    "InvokeFunc`10", 
                    "InvokeFunc`11", 
                    "InvokeFunc`12", 
                    "InvokeFunc`13", 
                    "InvokeFunc`14", 
                    "InvokeFunc`15", 
                    "InvokeFunc`16", 
                    "InvokeFunc`17", 
                    "InvokeFunc`2", 
                    "InvokeFunc`3", 
                    "InvokeFunc`4", 
                    "InvokeFunc`5", 
                    "InvokeFunc`6", 
                    "InvokeFunc`7", 
                    "InvokeFunc`8", 
                    "InvokeFunc`9", 
                    "InvokeMethod", 
                    "InvokeMethod`1", 
                    ////"IsolationLevelValidator",
                    ////"LambdaReference`1",
                    ////"LambdaValue`1",
                    "LessThan`3", 
                    "LessThanOrEqual`3", 
                    "Literal`1", 
                    ////"LocationReferenceValue`1",
                    "MultidimensionalArrayItemReference`1", 
                    "Multiply`3", 
                    "NativeActivity", 
                    "NativeActivity`1", 
                    "New`1", 
                    "NewArray`1", 
                    "Not`2", 
                    "NotEqual`3", 
                    ////"ObtainType",
                    "Or`3", 
                    "OrElse", 
                    "Parallel", 
                    "ParallelForEach`1", 
                    "Persist", 
                    "Pick", 
                    ////"PickBranchBody",
                    "Pop", 
                    "PropertyReference`2", 
                    "PropertyValue`2", 
                    "RemoveFromCollection`1", 
                    "Rethrow", 
                    ////"RethrowBuildConstraint",
                    "Sequence", 
                    "Subtract`3", 
                    "Switch`1", 
                    "TerminateWorkflow", 
                    "Throw", 
                    "TransactionScope", 
                    "TryCatch", 
                    "ValueTypeFieldReference`2", 
                    "ValueTypeIndexerReference`2", 
                    "ValueTypePropertyReference`2", 
                    "VariableReference`1", 
                    "VariableValue`1", 
                    "VisualBasicReference`1", 
                    "VisualBasicValue`1", 
                    "While", 
                    ////"WorkflowCompensationBehavior",
                    "WriteLine", 
                };

            List<Type> availableActivityTypes = allSystemActivitiesTypes
                                                   .Where(type => usefulNamespace.Contains(type.Namespace) && filter.Contains(type.Name))
                                                   .ToList();
            availableActivityTypes.Add(typeof(System.Activities.Statements.PickBranch)); // not really an activity but it belongs in the toolbox anyway
            availableActivityTypes.Add(typeof(System.Activities.Statements.FlowDecision)); // not really an activity but it belongs in the toolbox anyway
            availableActivityTypes.Add(typeof(System.Activities.Statements.FlowSwitch<>)); // not really an activity but it belongs in the toolbox anyway
            availableActivityTypes.AddRange(GetAllActivityTypesInAssembly(typeof(Send).Assembly)); // don't need to filter the ones in System.ServiceModel.Activities

            return availableActivityTypes;
        }

        /// <summary>
        /// Query all types derives from System.Activities.Activity class
        /// </summary>
        /// <param name="assembly">
        /// Source assembly
        /// </param>
        /// <param name="activityAssemblyItem">
        /// The ActivityAssemblyItem, if any, which will own the given assembly. If there is a type load error 
        /// we will set activityAssemblyItem.NotSafeForTypeLoad = true to signal the WorkflowDesigner not to 
        /// load this assembly.
        /// </param>
        /// <returns>
        /// All activity types in the assembly.
        /// </returns>
        public static List<Type> GetAllActivityTypesInAssembly(Assembly assembly, ActivityAssemblyItem activityAssemblyItem = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            Type[] rawTypeList;

            if (assembly.ReflectionOnly)
            {
                Assembly targetAssembly = Assembly.LoadFrom(assembly.Location);
                rawTypeList = targetAssembly.GetAsManyTypesAsPossible(activityAssemblyItem);
            }
            else
            {
                rawTypeList = assembly.GetAsManyTypesAsPossible(activityAssemblyItem);
            }

            var activityTypeList = rawTypeList
                                      .Where(type => type.IsVisible && (type.IsSubclassOf(typeof(Activity))
                                                  || typeof(IActivityTemplateFactory).IsAssignableFrom(type)))
                                      .ToList();

            return activityTypeList;
        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="owningAssemblyItem">
        /// The ActivityAssemblyItem, if any, which will own the given assembly. If there is a type load error 
        /// we will set activityAssemblyItem.NotSafeForTypeLoad = true to signal the WorkflowDesigner not to 
        /// load this assembly. Will be null for built-in assemblies like System.Activities.
        /// </param>
        /// <returns></returns>
        public static Type[] GetAsManyTypesAsPossible(this Assembly assembly, ActivityAssemblyItem owningAssemblyItem = null)
        {
            try
            {
                var types = assembly.GetTypes();
                if (owningAssemblyItem != null)
                {
                    owningAssemblyItem.NotSafeForTypeLoad = false;
                }
                return types;
            }
            catch (ReflectionTypeLoadException e)
            {
                owningAssemblyItem.IfNotNull(owner => owner.NotSafeForTypeLoad = true);
                return e.Types.Where(t => t != null).ToArray();
            }
        }


    }
}
