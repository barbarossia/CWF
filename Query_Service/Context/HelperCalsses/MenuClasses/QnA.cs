using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Support.Workflow.Context.HelperClasses.MenuClasses
{
   [Serializable]
    public class QnA
    {

        public string title;
        public List<string> _Q;

        public bool IsQSent;
        public bool IsCurrentQA = false; // client will chekc if 'this' Objuect is curretn inthe QA-list

        public string Ans;
        public int ID;

        public QnA(string str)
        {
            string[] lstr = str.Split(';', ',');
            title = lstr[0];
            _Q = lstr.Skip(1).ToList();
        }
        public QnA(MenuString mnstr)
        {
            string[] lstr = mnstr.MenuStrings.Split(';', ',');
            title = lstr[0];
            _Q = lstr.Skip(1).ToList();

        }

        public string QString()
        {
            string str1 = "";
            foreach (string str in this._Q)
            {
                str1 = str1 + str;
            }
            return str1;
        }

    
    }
}
