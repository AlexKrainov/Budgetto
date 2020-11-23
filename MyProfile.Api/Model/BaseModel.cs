using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MyProfile.API.Model
{
    [Serializable]
    [DataContract]
    [XmlRootAttribute("root")]
    public class BaseModel
    {
        [DataMember(Name = "session_id")]
        [XmlElement("session_id")]
        public Guid SessionID { get; set; }
        
        [DataMember(Name = "user_id")]
        [XmlElement("user_id")]
        public Guid UserID { get; set; }

        [DataMember(Name = "isOk")]
        [XmlElement("isOk")]
        public bool IsOk { get; set; }
    }
}
