using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MYQuizSupervisor
{
    public class RegistrationDevice
    {
        public string token { get; set; }
        public string id { get; set; }
        public string password { get; set; }

       
        //leerer Konstruktor für Deserialisierung
        public RegistrationDevice() { }
    }
}
