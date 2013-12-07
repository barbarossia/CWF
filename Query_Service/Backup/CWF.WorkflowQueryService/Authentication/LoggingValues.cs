

namespace CWF.WorkflowQueryService.Authentication
{
    using System.Diagnostics.CodeAnalysis;

     /// <summary>
        /// Common location for all error constants - both integer error codes, and string descriptions
        /// </summary>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "self documentation in each triplet")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "const int and strings require upper case and underscore")]
        [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Large error constant section with Doc, id, and message in sequence")]
    public class LoggingValues
    {
       
        //// 55180 "Security Error"
        public const int InvalidCredentials = 55180;
        public const string InvalidCredentialsMsg = "Access is denied for user {0}.";
    }
}