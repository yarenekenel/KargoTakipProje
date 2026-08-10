using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Core.Entity
{
    public class KargoDurumGecmisi
    {
        public int KargoDurumId { get; set; }
        public int KargoId { get; set; }
        public string Durum { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public DateTime Tarih { get; set; }
    }
}
