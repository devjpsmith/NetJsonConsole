using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace NetJsonConsole
{
    /// <summary>
    /// Simple class used to represent some real-world data for our 
    ///     serializer to work with
    /// </summary>
    [DataContract]
    public class ObjectModel
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool Deleted { get; set; }

    }
}
