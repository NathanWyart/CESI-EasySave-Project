using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_WPF.Model
{
    // This class represents the container for the state file.
    public class StateFileContainer
    {
        public List<State> States { get; set; } = new List<State>();
    }
}