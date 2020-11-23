using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyProfile.API.Model
{
    [Serializable]
    [DataContract]
    [XmlRootAttribute("root")]
    public class UserSettings : BaseModel
    {
        [DataMember(Name = "name")]
        [XmlElement("name")]
        public string Name { get; set; }
        [DataMember(Name = "email")]
        [XmlElement("email")]
        public string Email { get; set; }
    }
}
