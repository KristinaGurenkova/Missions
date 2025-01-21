using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Missions.Classes
{
    public class Task
    {
        public int IdTask { get; set; }
        public string NameTask { get; set; }
        public string Description { get; set; }
        public double Deadline { get; set; }
        public int Status { get; set; }
        public int IdProject { get; set; }
    }

}
