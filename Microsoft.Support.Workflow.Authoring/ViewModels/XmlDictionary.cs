using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Schema;

namespace Microsoft.Support.Workflow.Authoring.ViewModels
{
    //[Serializable]
    //public sealed class XmlDictionary<T, V> : Dictionary<T, V>, IXmlSerializable
    //{
    //    [XmlType("Entry")]
    //    public struct Entry
    //    {
    //        public Entry(T key, V value) : this() { Key = key; Value = value; }
    //        [XmlElement("Key")]
    //        public T Key { get; set; }
    //        [XmlElement("Value")]
    //        public V Value { get; set; }
    //    }

    //    public XmlSchema GetSchema()
    //    {
    //        return null;
    //    }

    //    public void ReadXml(System.Xml.XmlReader reader)
    //    {
    //        this.Clear();
    //        var serializer = new XmlSerializer(typeof(List<Entry>));
    //        reader.Read();
    //        var list = (List<Entry>)serializer.Deserialize(reader);
    //        foreach (var entry in list) this.Add(entry.Key, entry.Value);
    //        reader.ReadEndElement();
    //    }

    //    public void WriteXml(System.Xml.XmlWriter writer)
    //    {
    //        var list = new List<Entry>(this.Count);
    //        foreach (var entry in this) list.Add(new Entry(entry.Key, entry.Value));
    //        XmlSerializer serializer = new XmlSerializer(list.GetType());
    //        serializer.Serialize(writer, list);
    //    }

    //    public XmlDictionary<T, V>() : base() { }
    //    public XmlDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
    //} 

}
