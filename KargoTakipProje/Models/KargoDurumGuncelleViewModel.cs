using System.ComponentModel.DataAnnotations;

namespace KargoTakipProje.Models
{
    public class KargoDurumGuncelleViewModel
    {
        [Required]
        public int KargoId { get; set; }

        [Required(ErrorMessage = "Lütfen bir durum seçin.")]
        [Display(Name = "Yeni Durum")]
        public string YeniDurum { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }
    }
}