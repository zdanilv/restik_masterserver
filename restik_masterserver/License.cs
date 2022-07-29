using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restik_masterserver
{
    internal class License
    {
        public int ID { get; set; }
        public DateTime ExpirationLicense { get; set; }
        public bool LicenseActive { get; set; }
    }
}
