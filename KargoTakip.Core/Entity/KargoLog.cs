using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Core.Entity
{
    public class KargoLog
    {
        public int LogId { get; set; }
        public int KargoId { get; set; }
        public string LogTuru { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public DateTime Tarih { get; set; }
    }
}
