using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WinDriver
{
    public class Command
    {
        public string Name { get; set; }
        public Dictionary<string, JObject> Parameters { get; set; }
        public string SessionId { get; set; }
    }
}