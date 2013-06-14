using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Service.Common
{
    public static class EventCodeRange
    {
        public const int DatabaseErrorStart = 50000;
        public const int DatabaseErrorEnd = 50499;
        public const int DatabaseValidationStart = 50500;
        public const int DatabaseValidationEnd = 50899;
        public const int DatabaseInfoStart = 50900;
        public const int DatabaseInfoEnd = 50999;
        
        public const int DataAccessLayerErrorStart = 51000;
        public const int DataAccessLayerErrorEnd = 51499;
        public const int DataAccessLayerValidationStart = 51500;
        public const int DataAccessLayerValidationEnd = 51899;
        public const int DataAccessLayerInfoStart = 51900;
        public const int DataAccessLayerInfoEnd = 51999;

        public const int BusinessLayerErrorStart = 52000;
        public const int BusinessLayerErrorEnd = 52499;
        public const int BusinessLayerValidationStart = 52500;
        public const int BusinessLayerValidationEnd = 52899;
        public const int BusinessLayerInfoStart = 52900;
        public const int BusinessLayerInfoEnd = 52999;

        public const int WebServiceLayerErrorStart = 53000;
        public const int WebServiceLayerErrorEnd = 53499;
        public const int WebServiceLayerValidationStart = 53500;
        public const int WebServiceLayerValidationEnd = 53899;
        public const int WebServiceLayerInfoStart = 53900;
        public const int WebServiceLayerInfoEnd = 53999;

    }
}
