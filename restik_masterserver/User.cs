using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restik_masterserver
{
    internal class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
        public DateTime ExpirationLicense { get; set; }
        public bool LicenseActive { get; set; }
        public string GetText()
        {
            return string.Join("$", Id, Username, FullName, ExpirationLicense, LicenseActive);
        }
        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(GetText());
        }
    }
}
