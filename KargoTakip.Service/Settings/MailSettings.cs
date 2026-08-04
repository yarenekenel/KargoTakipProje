using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Bu sınıf, appsettings.json'daki MailSettings JSON bölümünü C# nesnesine otomatik eşleyecek (WebUI'de Program.cs içinde bağlayacağız)
namespace KargoTakip.Service.Settings
{
    public class MailSettings
    {
        public string SenderName { get; set; } = string.Empty;
        public string SenderMail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
    }
}
