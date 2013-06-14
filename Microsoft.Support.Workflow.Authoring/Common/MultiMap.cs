using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.Support.Workflow.Authoring.Common
{
    [Serializable]
    public class MultiMap<K,V> : Dictionary<K, List<V>>
    {
        public MultiMap() { }

        protected MultiMap(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public void AddValue(K key, V value)
        {
            List<V> values;
            if (!TryGetValue(key, out values))
            {
                values = new List<V>();
                Add(key, values);
            }
            values.Add(value);
        }

        public IEnumerable<V> GetValues(K key)
        {
            List<V> values;
            if (TryGetValue(key, out values))
            {
                foreach (var v in values)
                    yield return v;
            }
        }
    }
}
