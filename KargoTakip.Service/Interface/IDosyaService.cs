using KargoTakip.Core.Entity;
using Microsoft.AspNetCore.Http;

namespace KargoTakip.Service.Interface
{
    public interface IDosyaService
    {
        Task<int> DosyaKaydetAsync(int kargoId, IFormFile dosya, string? dosyaTuru);
        Task<IEnumerable<KargoDosya>> GetDosyalarByKargoIdAsync(int kargoId);
        Task<bool> DosyaSilAsync(int kargoDosyaId);
    }
}