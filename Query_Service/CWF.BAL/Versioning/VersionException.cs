using Microsoft.Support.Workflow.Service.BusinessServices;
namespace CWF.BAL.Versioning
{
    /// <summary>
    /// Extends the BusinessException class to provide information about version exceptions
    /// </summary>
    public class VersionException : BusinessException
    {
        /// <summary>
        /// requires that this exception type specify a message and the rule that was broken upon construction
        /// </summary>
        /// <param name="message">Message for the base class</param>
        /// <param name="rule">The versioning rule that was broken</param>
        public VersionException(string message, Rule rule)
            : base(-1, message) 
            // TODO: VersionException may be replaced by BusinessException.  Providing -1 for error code temporarily.
        {
            Rule = rule;
        }

        /// <summary>
        /// The rule that was tested against
        /// </summary>
        public Rule Rule { get; set; }
    }
}
