using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Missions.Classes
{
    public class Project
    {
        public int IdProject { get; set; }
        public string NameProject { get; set; }
        public string Description { get; set; }
        public int IdTeam { get; set; }
        public double Deadline { get; set; }
    }

}
