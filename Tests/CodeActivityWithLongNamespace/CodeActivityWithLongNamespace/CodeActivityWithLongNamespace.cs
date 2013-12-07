using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TestInput_Library1;

/*                10        20        30        40        50        60        70        80        90       100       110       120       130       140       150       160       170       180       190       200       210       220       230       240
/*        012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789                      */
namespace CodeActivityLibraryaaaaaaaaaaaaaaaaaaaaaaaaaaaaa50aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa100aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa150aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa200aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa240 
{
    public sealed class CodeActivityWithLongNamespace : CodeActivity<int>
    {
        // Define an activity input argument of type string        
        public InArgument<int> Number1 { get; set; }
        public InArgument<int> Number2 { get; set; }
        public Activity1 Sibling { get; set; } // force dependency on TestInput_Library1

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override int Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument            
            int num1 = context.GetValue(this.Number1);
            int num2 = context.GetValue(this.Number2);
            int sum = num1 + num2;

            return sum;
        }
    }
}
