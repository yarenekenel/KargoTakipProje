namespace KargoTakipProje.Models
{
    public class DosyaYukleViewModel
    {
        public int KargoId { get; set; }
        public IFormFile Dosya { get; set; } = null!;
        public string? DosyaTuru { get; set; }
    }
}
