using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Support.Workflow.Catalog;

namespace Microsoft.Support.Workflow
{
    public partial class WFContent
    {
        public WFContent()
        {
            this.Id = Guid.Empty;
            this.Text = "";
            this.Keywords = new List<string>();
        }

        /// <summary>
        /// Create an instance of content using an existing content item
        /// </summary>
        /// <param name="Id"></param>
        internal WFContent(Guid Id) : base()
        {
            WFContent awid = WFContent.GetByID(Id);
            this.Id = awid.Id;
            this.Text = awid.Text;
            this.Keywords = awid.Keywords;
        }

        internal static WFContent GetByID(Guid Id)
        {
            WFContent aWFC = new WFContent();
            using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
            {
                Content aCitem = (from c in proxy.Contents
                                  where c.ID == Id
                                  select c).FirstOrDefault();

                if (aCitem == null)
                {
                    throw new ArgumentOutOfRangeException("Invalid GUID for Content Item: " + Id.ToString());
                }
                else
                {
                    aWFC.Id = Id;
                    aWFC.Text = aCitem.Text;
                    if (!(aCitem.Keywords == null))
                        aWFC.Keywords = aCitem.Keywords.Split(';').ToList();
                }
            }
            return aWFC;

        }

        internal WFContent(string Content) : base()
        {
            Guid aID = Guid.Empty;
            if (Guid.TryParse(Content, out aID))
            {
                WFContent awid = WFContent.GetByID(Id);
                this.Id = awid.Id;
                this.Text = awid.Text;
                this.Keywords = awid.Keywords;
            }
            else
            {
                using (CWFDummyDataSourceEntities proxy = new CWFDummyDataSourceEntities())
                {
                    Content aCitem = (from c in proxy.Contents
                                      where c.Text.Trim().ToUpper().Equals(Content.Trim().ToUpper())
                                      select c).FirstOrDefault();

                    if (aCitem == null)
                    {
                        this.Text = Content;
                    }
                    else
                    {
                        this.Id = aCitem.ID;
                        this.Text = aCitem.Text;
                        if (!(aCitem.Keywords == null))
                            this.Keywords = aCitem.Keywords.Split(';');
                    }
                }
            }
        }
    }
}