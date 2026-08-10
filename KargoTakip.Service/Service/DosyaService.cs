using KargoTakip.Core.Entity;
using KargoTakip.Data.Repositories;
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
            var kargoKlasoru = Path.Combine(_uploadRootPath, "uploads", "kargo", kargoId.ToString());
            if (!Directory.Exists(kargoKlasoru))
                Directory.CreateDirectory(kargoKlasoru);

            var dosyaAdi = $"{Guid.NewGuid()}_{dosya.FileName}";
            var fizikselYol = Path.Combine(kargoKlasoru, dosyaAdi);

            using (var stream = new FileStream(fizikselYol, FileMode.Create))
            {
                await dosya.CopyToAsync(stream);
            }

            var relativeYol = $"/uploads/kargo/{kargoId}/{dosyaAdi}";  //dosya yolu oluşturuluyor

            var kargoDosya = new KargoDosya
            {
                KargoId = kargoId,
                DosyaYolu = relativeYol, //bu yol veritabanına gönderilecek nesneye yazılıyor
                DosyaTuru = dosyaTuru,
                YuklemeTarihi = DateTime.Now
            };

            return await _kargoDosyaRepository.AddAsync(kargoDosya); //burada veritabanına gönderiliyor
        }

        public async Task<IEnumerable<KargoDosya>> GetDosyalarByKargoIdAsync(int kargoId)
        {
            return await _kargoDosyaRepository.GetByKargoIdAsync(kargoId);
        }

        public async Task<bool> DosyaSilAsync(int kargoDosyaId)
        {
            var dosyaKaydi = await _kargoDosyaRepository.GetByIdAsync(kargoDosyaId);
            if (dosyaKaydi == null)
                return false;

            var fizikselYol = Path.Combine(_uploadRootPath, dosyaKaydi.DosyaYolu.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fizikselYol))
            {
                File.Delete(fizikselYol);
            }

            return await _kargoDosyaRepository.DeleteAsync(kargoDosyaId);
        }
    }
}