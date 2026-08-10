using KargoTakip.Core.Entity;

namespace KargoTakipProje.Models
{
    public class KargoFiltreViewModel
    {
        public string? AramaMetni { get; set; }
        public string? DurumFiltre { get; set; }
        public List<Kargo> Kargolar { get; set; } = new();

        public int SayfaNo { get; set; } = 1;
        public int SayfaBoyutu { get; set; } = 10;
        public int ToplamKayit { get; set; }
        public int ToplamSayfa => (int)Math.Ceiling((double)ToplamKayit / SayfaBoyutu);
    }
}