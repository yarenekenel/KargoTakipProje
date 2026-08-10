using System.ComponentModel.DataAnnotations;

namespace KargoTakipProje.Models
{
    public class KargoOlusturViewModel
    {
        [Required(ErrorMessage = "Gönderen ad soyad zorunludur.")]
        [Display(Name = "Gönderen Ad Soyad")]
        public string GonderenAdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alıcı ad soyad zorunludur.")]
        [Display(Name = "Alıcı Ad Soyad")]
        public string AliciAdSoyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alıcı mail adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        [Display(Name = "Alıcı Mail Adresi")]
        public string AliciMail { get; set; } = string.Empty;
    }
}