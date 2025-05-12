using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NS_Model
{
    // This class represents the container for the state file.
    public class StateFileContainer
    {
        public List<State> States { get; set; } = new List<State>();
    }
}