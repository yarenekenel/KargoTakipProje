using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Service.Interface
{
    public interface IMailService
    {
        Task SendMailAsync(string toMail, string subject, string body);
    }
}
