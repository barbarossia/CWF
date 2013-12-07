using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Support.Workflow.Context.HelperClasses.MenuClasses;
using Microsoft.Support.Workflow.Context.HelperClasses.MessageClasses;


namespace Microsoft.Support.Workflow.Context
{
    public class ContextObj2
    {
        private ConsumerType _ConsumerType;
        private LocaleTypes _Locale;
        private string _UserInfo;
        private string _Activity;
        private string _Control;
        private Dictionary<string, string> _Context;
        private string product;
        private string support;

        //public List<QnA> QAMenu = new List<QnA>();
        
        
        //public bool IsFinish;

        //public DisplayMessage msg;

        
        //public string soultion;
        //public bool IsSoution;


        //public bool IsSystemUp;
    
        public string UserInfo
        {
            get
            {
              return _UserInfo;
            }
            set
            {
                if (value != null)
                {
                    _UserInfo = value;
                }
                else _UserInfo = "";
            }
        }
        public LocaleTypes Locale
        {
            get
            {
                return _Locale;
            }
            set
            {
                if (value != null)
                {
                    _Locale = value;
                }
                else _Locale =LocaleTypes.ENGLISH;
            }
        }
        public ConsumerType CosumerType
        {
            get
            {
                return _ConsumerType;
            }
            set
            {
                if (value != null)
                {
                    _ConsumerType = value;
                }
                else _ConsumerType = ConsumerType.REGULAR;
            }
        }

        public string Activity
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public string Control
        {
            get
            {
                return null; 
            }
            set
            {
            }
        }

        public Dictionary<string, string> Context
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public int Cu
        {
            get
            {
                return 1;
            }
            set
            {
            }
        }

        public CurrentWFinfo CurWFcontext
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public string Products
        {
           get
            {
                return product;
            }
            set
            {
                if (value != null)
                {
                    product = value;
                }
            }
        }

        public string Support
        {
            get
            {
                return support;
            }
            set
            {
                if (value != null)
                {
                    support = value;
                }
                
            }
        }

    }
}
