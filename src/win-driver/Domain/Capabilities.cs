using System.Collections.Generic;

namespace WinDriver.Domain
{
    public class Capabilities
    {
        private readonly Dictionary<string, object> _capabilities;

        public Capabilities(Dictionary<string, object> capabilities)
        {
            _capabilities = capabilities;
        }

        public object Dto
        {
            get { return _capabilities; }
        }
    }
}