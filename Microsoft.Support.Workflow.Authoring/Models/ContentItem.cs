namespace Microsoft.Support.Workflow.Authoring.Models
{
    using System;

    [Serializable]
    public class ContentItem
    {
        public String ContentFileName { get; set; }
        public String Key { get; set; }
        public String Value { get; set; }
        public String Section { get; set; }

        public ContentItem()
        {
        }

        public ContentItem(string text)
        {
            Value = text;
        }
    }
}
