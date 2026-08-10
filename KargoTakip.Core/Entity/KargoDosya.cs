using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Core.Entity
{
    public class KargoDosya
    {
        public int KargoDosyaId { get; set; }
        public int KargoId { get; set; }
        public string DosyaYolu { get; set; } = string.Empty;
        public string? DosyaTuru { get; set; }
        public DateTime YuklemeTarihi { get; set; }
    }
}
