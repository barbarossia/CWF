using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Microsoft.Support.Workflow.Context.HelperCalsses
{
   public static class Helper
    {

       /// <summary>
       /// log strings
       /// </summary>
       /// <param name="x"></param>
 
       public static void Log(string x ,bool mode)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\temp\drh.log",mode);
            sw.WriteLine(x);
            sw.Flush();
            sw.Close();
        }

       /// <summary>
       /// log Error messages.
       /// </summary>
       /// <param name="e"></param>
       public static void Log(Exception e, bool mode)
        {
           System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\temp\drh.log",mode);

           sw.WriteLine(e.ToString());
            sw.Flush();
            sw.Close();
        }



        /// <summary>
        /// Finds the  project base folder. It does this by starting in the currenly executing directory
        /// and searching upwards for a directroy containing a directory named "WorkflowSystem"
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo WorkflowSystemBaseFolder()
        {
            System.Reflection.Assembly current = System.Reflection.Assembly.GetExecutingAssembly();

            DirectoryInfo currDir = new DirectoryInfo(Path.GetDirectoryName(new Uri(current.CodeBase).LocalPath));
            while (currDir != null)
            {
                foreach (DirectoryInfo d in currDir.GetDirectories())
                {
                    if (d.Name.Equals("RehostedDesigner", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return currDir;
                    }
                }
                currDir = currDir.Parent;
            }
            return currDir;
        }
        /// <summary>
        /// Finds the External Library folder, used to get other assemply paths
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo ExternalLibraryFolder(string relativePathString)
        {
            return new DirectoryInfo(Path.Combine(WorkflowSystemBaseFolder().FullName, relativePathString));//"Externals"
        }

    }
}
