using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS_Model
{
    // This class represents the state of the works.
    public class State
    {
        public string Name { get; set; } 
        public int TotalFile { get; set; }
        public long TotalSize { get; set; }
        public int Progress { get; set; }
        public int LeftFile { get; set; }
        public long LeftSize { get; set; }
        public string CurrentPathSrc { get; set; }
        public string CurrentPathDst { get; set; }
        public string CurrentDateTime { get; set; }
        public string StateStatus { get; set; }

        // Constructor to initialize the state
        public State() { }
    }
}

