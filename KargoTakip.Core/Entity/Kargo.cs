using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Core.Entity
{
    public class Kargo
    {
        public int KargoId { get; set; }
        public string TakipNo { get; set; } = string.Empty;
        public string GonderenAdSoyad { get; set; } = string.Empty;
        public string AliciAdSoyad { get; set; } = string.Empty;
        public string AliciMail { get; set; } = string.Empty;
        public string MevcutDurum { get; set; } = string.Empty;
        public DateTime OlusturmaTarihi { get; set; }
    }
}
