using KargoTakip.Core.Entity;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Repositories;
using KargoTakip.Data.Repository;
using KargoTakip.Service.Interface;
using Microsoft.AspNetCore.Http;

namespace KargoTakip.Service.Service
{
    public class DosyaService : IDosyaService
    {
        private readonly IKargoDosyaRepository _kargoDosyaRepository;
        private readonly string _uploadRootPath;

        public DosyaService(IKargoDosyaRepository kargoDosyaRepository, string uploadRootPath)
        {
            _kargoDosyaRepository = kargoDosyaRepository;
            _uploadRootPath = uploadRootPath;
        }

        public async Task<int> DosyaKaydetAsync(int kargoId, IFormFile dosya, string? dosyaTuru)
        {
            // Kargoya özel klasör oluştur: wwwroot/uploads/kargo/{kargoId}/
            var kargoKlasoru = Path.Combine(_uploadRootPath, "uploads", "kargo", kargoId.ToString());
            if (!Directory.Exists(kargoKlasoru))
                Directory.CreateDirectory(kargoKlasoru);

            // Dosya adını benzersiz yap (aynı isimli dosya üzerine yazılmasın)
            var dosyaAdi = $"{Guid.NewGuid()}_{dosya.FileName}";
            var fizikselYol = Path.Combine(kargoKlasoru, dosyaAdi);

            using (var stream = new FileStream(fizikselYol, FileMode.Create))
            {
                await dosya.CopyToAsync(stream);
            }

            // Veritabanına göreceli yolu kaydet (wwwroot'a göre)
            var relativeYol = $"/uploads/kargo/{kargoId}/{dosyaAdi}";

            var kargoDosya = new KargoDosya
            {
                KargoId = kargoId,
                DosyaYolu = relativeYol,
                DosyaTuru = dosyaTuru,
                YuklemeTarihi = DateTime.Now
            };

            return await _kargoDosyaRepository.AddAsync(kargoDosya);
        }

        public async Task<IEnumerable<KargoDosya>> GetDosyalarByKargoIdAsync(int kargoId)
        {
            return await _kargoDosyaRepository.GetByKargoIdAsync(kargoId);
        }
    }
}