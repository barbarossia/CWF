using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Support.Workflow.Context.HelperClasses.MenuClasses;
using Microsoft.Support.Workflow.Context.HelperClasses.MessageClasses;


namespace Microsoft.Support.Workflow.Context
{
    public class ContextObj
    {
        private ConsumerType _ConsumerType;
        private LocaleTypes _Locale;
        private string _UserInfo;
        private string _Activity;
        private string _Control;
        private Dictionary<string, string> _Context;
        private string product;
        private string support;

        public string BookMarkName;

        //private List<QnA> _QAMenu = new List<QnA>();
        public List<QnA> QAMenu = new List<QnA>();// { get; set; }

        public QnA currQnA;
        public bool IsNextQnA =true;


        private bool isFinish;
        public bool IsFinish
        {
            get { return isFinish; }
            set
            {
                 { isFinish = value; }
               
            }
        }

        private DisplayMessage _msg;
        public DisplayMessage msg
        {
            get { return _msg; }
            set
            {
                if (value != null) { _msg = value; }
                //else _msg = new DisplayMessage("Default _msg String");
            }
        }

        private string _solution;
        public string solution
        {
            get { return _solution; }
            set
            {
                if (value != null) { _solution = value; }
                else _solution = "Default _solution string";
            }
        }

        private bool isSolution;
        public bool IsSolution
        {
            get { return isSolution; }
            set
            {
                if (value != null) { isSolution = value; }
                else isSolution = false;
            }
        }


        private bool isSystemUp;
        public bool IsSystemUp
        {
            get { return isSystemUp; }
            set
            {
                if (value != null) { isSystemUp = value; }
                else isSystemUp = false;
            }
        }
    
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
        public CurrentWFinfo CurWFcontext { get; set;}
     
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
