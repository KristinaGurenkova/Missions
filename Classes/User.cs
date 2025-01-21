using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Missions.Classes
{
    public class User
    {
        public int IdUser { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Middlename { get; set; }
        public string Email { get; set; }
        public int IdRole { get; set; }

        public override string ToString() => $"{Surname} {Name} {Middlename}"; // Отображение в ComboBox
    }

}
