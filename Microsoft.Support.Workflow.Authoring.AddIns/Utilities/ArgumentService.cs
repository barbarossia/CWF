using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Diagnostics.Contracts;
using Microsoft.Support.Workflow.Authoring.CompositeActivity;
using Microsoft.Support.Workflow.Authoring.AddIns.CompositeActivity;

namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
   public static class ArgumentService
    {
       private static DynamicActivityPropertyEqualityComparer argumentEqualityComparer = new DynamicActivityPropertyEqualityComparer();
        public const string Name_Prpoerty = "Properties";

       public static string GetExpressionText(this Argument argument)
       {
           if (argument == null)
           {
               return string.Empty;
           }

           var expression = argument.Expression;
           if (expression != null)
           {
               if (expression.GetType().Name == "Literal`1")
               {
                   return expression.GetType().IfNotNull(t => t.GetProperty("Value")).GetValue(expression, null).IfNotNull(o => o.ToString());
               }
               else
               {
                   return expression.GetType().IfNotNull(t => t.GetProperty("ExpressionText")).GetValue(expression, null).IfNotNull(o => o.ToString());
               }
           }
           else
           {
               return string.Empty;
           }
       }

       /// <summary>
       /// Create arguments from composite workflow to exist workflow
       /// </summary>
       public static void CreateArguments(ModelItem oldModel)
       {
           Contract.Requires(oldModel != null && oldModel.Root != null &&
               oldModel.Root.GetCurrentValue() is ActivityBuilder);

           ModelItem root = oldModel.Root;
           var newArgs = GetArgument(oldModel);
           var alreadyArgs = GetArgument(root.GetCurrentValue() as ActivityBuilder);
           var ex = newArgs.Except(alreadyArgs, argumentEqualityComparer);
           AddArguments(root, ex);
       }

       /// <summary>
       /// Return activity has argment
       /// </summary>
       public static bool IsArgument(Activity arg)
       {
           Contract.Requires(arg != null);

           return (typeof(ActivityWithResult).IsAssignableFrom(arg.GetType()));
       }

       /// <summary>
       /// Return activity has argment
       /// </summary>
       public static bool IsArgument(Type arg)
       {
           Contract.Requires(arg != null);

           return typeof(Argument).IsAssignableFrom(arg);
       }

       public static void AddArguments(ModelItem bodyItem, ModelItem rootItem)
       {
           var args = rootItem.Properties[Name_Prpoerty].Collection.ToList()
               .Select(m => m.GetCurrentValue() as DynamicActivityProperty);
           foreach (var arg in args)
           {
               bodyItem.Properties[Name_Prpoerty].Collection.Add(arg);
           }
       }

       public static void AddArguments(ModelItem root, IEnumerable<DynamicActivityProperty> args)
       {
           ModelItemCollection argColletion = root.Properties[Name_Prpoerty].Collection;
           foreach (var arg in args)
           {
               argColletion.Add(arg);
           }
       }

       public static IEnumerable<DynamicActivityProperty> GetArgument(ModelItem model)
       {
           foreach (ModelProperty prop in model.Properties)
           {
               if (typeof(Argument).IsAssignableFrom(prop.PropertyType) && prop.Value != null)
               {
                   yield return ConvertArgument(prop.Name, prop.Value.GetCurrentValue() as Argument);
               }
           }
       }

       private static DynamicActivityProperty ConvertArgument(string name, Argument arg)
       {
           return new DynamicActivityProperty()
           {
               Name = name,
               Type = arg.GetType(),
           };
       }
        
       public static IEnumerable<DynamicActivityProperty> GetArgument(ActivityBuilder designerBuilder)
       {
           foreach (DynamicActivityProperty prop in designerBuilder.Properties)
           {
               if (typeof(Argument).IsAssignableFrom(prop.Type))
               {
                   yield return prop;
               }
           }
       }

       public static IEnumerable<Variable> GetAvailableVariables(ModelItem modelItem)
       {
           if (modelItem != null && modelItem.Properties["Variables"] != null)
           {
               foreach (var varialble in modelItem.Properties["Variables"].Collection)
               {
                   yield return varialble.GetCurrentValue() as Variable;
               }
           }

           if (modelItem.Parent != null)
           {
               foreach (var parent in GetAvailableVariables(modelItem.Parent))
               {
                   yield return parent;
               }
           }
       }

       public static ModelItem GetAvailableParent(ModelItem modelItem)
       {
           var vs = modelItem.Properties["Variables"];
           if (vs != null)
               return modelItem;

           if (modelItem.Parent != null)
               return GetAvailableParent(modelItem.Parent);

           return null;
       }

       public static Variable CovertArgumentToVariable(DynamicActivityProperty prop)
       {
           Type varType = prop.Type;
           if (typeof(Argument).IsAssignableFrom(prop.Type))
           {
               varType = prop.Type.GetGenericArguments().Single();
           }

           return Variable.Create(prop.Name, varType, VariableModifiers.None);
       }

       public static void OverwriteVarialble(Variable oldVar, Variable newVar)
       {
           oldVar = Variable.Create(oldVar.Name, newVar.Type, oldVar.Modifiers);
       }

       public static void AddVariable(ModelItem parent, IEnumerable<Variable> variables)
       {
           variables.ToList().ForEach(v => parent.Properties["Variables"].Collection.Add(v));
       }
    }
}
