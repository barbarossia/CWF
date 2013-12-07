using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow
{
    [DataContract]
    public class Icon
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public byte[] Image { get; set; }

        public Icon()
        {
            this.Name = "";
            this.Image = null;
        }

        public Image ConvertToImage()
        {
            return null;
        }


    }
}
