using System.Collections.Generic;

namespace cyber
{
    internal class ResponseRule
    {
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> Responses { get; set; } = new List<string>();
    }
}