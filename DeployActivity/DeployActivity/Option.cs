using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeployActivity
{
    // Eliminate chained nullchecks a la LINQ "Select"
    public static class Option
    {
        public static TResult IfNotNull<T, TResult>(this T val, Func<T, TResult> select) where T : class where TResult : class
        {
            if (val == null) 
                return null;
            else 
                return select(val);
        }
    }
}
