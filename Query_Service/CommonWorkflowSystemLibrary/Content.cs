using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Support.Workflow.Activity;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [Serializable]
    [DataContract]
    public class xxContent
    {
        /// <summary>
        /// added during conversion of EDm to ER model DB
        /// </summary>
        public Guid NEWContentId { get; set; }
        public Int32 NEWId { get; set; }

        /// <summary>
        /// Old EDm model
        /// </summary>
        public Guid Id { get; set; }

        public String Text { get; set; }

        public xxContent()
        {
            Id = Guid.NewGuid();
        }

        public xxContent(string text)
        {
            Text = text;
            
        }

        public static List<xxContent> GetDummyList()
        {
            List<xxContent> alist = new List<xxContent>();
            alist.Add(new xxContent("How are you connected to the Internet"));
            alist.Add(new xxContent("Wired"));
            alist.Add(new xxContent("Wireless"));
            alist.Add(new xxContent("Where do you use this product?"));
            alist.Add(new xxContent("Home"));
            alist.Add(new xxContent("Office"));

            return alist;
        }

    }
}
