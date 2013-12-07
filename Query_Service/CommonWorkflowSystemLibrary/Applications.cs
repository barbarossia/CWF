using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    public enum ApplicationTypes
    {
        /// <summary>
        /// Web based internet application
        /// </summary>
        Internet,
        /// <summary>
        /// Windows Based application
        /// </summary>
        Windows,
        /// <summary>
        /// Interactive Voice Recognition System
        /// </summary>
        IVR, 
        /// <summary>
        /// Other Type of Application
        /// </summary>
        Other
    }

    /// <summary>
    /// Class for applications which support workflows authored and
    /// executed by the common workflow framework.
    /// </summary>

    [Serializable]
    [DataContract]
    public class WFApplication
    {
        /// <summary>
        /// Added in the conversion from EDm to ER model DB
        /// </summary>
        [DataMember]
        public Guid NEWApplicationId { get; set; }
        [DataMember]
        public Int32 NEWId { get; set; }

        /// <summary>
        /// used only in EDm model
        /// Unique identifer for the application
        /// </summary>
        /// 
        [DataMember]
        public Guid Id { get; set; }
        /// <summary>
        /// Application name.
        /// </summary>
        /// 
        [DataMember]
        public String Name { get; set; }
        /// <summary>
        /// Description of the application
        /// </summary>
        /// 
        [DataMember]
        public String Description { get; set; } 
        /// <summary>
        /// Attribute used to indicate the type of application, which 
        /// may impact the type of workflows which are able to be used
        /// by the application.
        /// </summary>
        [DataMember]
        public ApplicationTypes ApplicationType { get; set; }
        

        public WFApplication ()
        {
            Id = Guid.NewGuid();
            ApplicationType = ApplicationTypes.Other;


        }

        public static IList<WFApplication> GetList()
        {
            IList<WFApplication> appList = new List<WFApplication>();
            appList.Add(new WFApplication() { Id = Guid.NewGuid(), Name = "OAS", ApplicationType = ApplicationTypes.Internet, Description = "" });
            
            appList.Add(new WFApplication() { Id = Guid.NewGuid(), Name = "DummyApp", ApplicationType = ApplicationTypes.Windows, Description = "" });

            return appList;
        }

        public static WFApplication Get(Guid ID)
        {
            return new WFApplication() { Id = ID, Name = "DummyApp", ApplicationType = ApplicationTypes.Windows, Description = "" };
        }
    }
}
