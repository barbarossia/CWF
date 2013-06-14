namespace Microsoft.Support.Workflow.Authoring.ExpressionEditor
{
    using System.Collections.Generic;

    internal class TreeNodeComparer : IComparer<TreeNodes>
    {           
        public int Compare(TreeNodes nodeX, TreeNodes nodeY)
        {
            if(nodeX == null)
            {
                if(nodeY == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if(nodeY == null)
                {
                    return 1;
                }
                else
                {
                    return nodeX.Name.CompareTo(nodeY.Name);
                }
                
            }
        }
    }
}
