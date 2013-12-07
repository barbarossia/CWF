using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CWF.WorkflowQueryService.UserError.Config
{
    public static class UserErrorConfigConstant
    {
        public static class AttributeName
        {
            public const string ErrorCode = "errorCode";
            public const string UserMessage = "userMessage";
            public const string FaultType = "faultType";
        }

        public static class NodeName
        {
            public const string UserErrorMessageConfiguration = "userErrorConfiguration";
            public const string Errors = "errors";
        }
    }
}