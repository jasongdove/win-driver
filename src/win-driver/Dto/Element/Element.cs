using System.Runtime.Serialization;

namespace WinDriver.Dto.Element
{
    [DataContract]
    public class Element
    {

        [DataMember(Name = "ELEMENT")]
        public string Id { get; set; }
    }
}