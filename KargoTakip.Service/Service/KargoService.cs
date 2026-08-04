using KargoTakip.Core.Entity;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Repositories;
using KargoTakip.Data.Repository;
using KargoTakip.Service.Interface;

namespace KargoTakip.Service.Service
{
    public class KargoService : IKargoService
    {
        private readonly IKargoRepository _kargoRepository;
        private readonly IKargoDurumGecmisiRepository _kargoDurumGecmisiRepository;
        private readonly IMailService _mailService;

        public KargoService(
            IKargoRepository kargoRepository,
            IKargoDurumGecmisiRepository kargoDurumGecmisiRepository,
            IMailService mailService)
        {
            _kargoRepository = kargoRepository;
            _kargoDurumGecmisiRepository = kargoDurumGecmisiRepository;
            _mailService = mailService;
        }

        public async Task<IEnumerable<Kargo>> GetAllKargolarAsync()
        {
            return await _kargoRepository.GetAllAsync();
        }

        public async Task<Kargo?> GetKargoByIdAsync(int kargoId)
        {
            return await _kargoRepository.GetByIdAsync(kargoId);
        }

        public async Task<Kargo?> GetKargoByTakipNoAsync(string takipNo)
        {
            return await _kargoRepository.GetByTakipNoAsync(takipNo);
        }

        public async Task<int> KargoOlusturAsync(Kargo kargo)
        {
            kargo.OlusturmaTarihi = DateTime.Now;
            kargo.MevcutDurum = "Hazırlanıyor";

            var yeniKargoId = await _kargoRepository.AddAsync(kargo);

            // Kargo oluşturulunca ilk durum kaydı da geçmişe düşsün
            var ilkDurum = new KargoDurumGecmisi
            {
                KargoId = yeniKargoId,
                Durum = "Hazırlanıyor",
                Aciklama = "Kargo sisteme kaydedildi.",
                Tarih = DateTime.Now
            };
            await _kargoDurumGecmisiRepository.AddAsync(ilkDurum);

            return yeniKargoId;
        }

        public async Task<bool> DurumGuncelleAsync(int kargoId, string yeniDurum, string? aciklama)
        {
            var kargo = await _kargoRepository.GetByIdAsync(kargoId);
            if (kargo == null)
                return false;

            // 1. Kargo tablosundaki mevcut durumu güncelle
            var guncellendi = await _kargoRepository.UpdateDurumAsync(kargoId, yeniDurum);
            if (!guncellendi)
                return false;

            // 2. Durum geçmişine yeni kayıt ekle
            var durumGecmisi = new KargoDurumGecmisi
            {
                KargoId = kargoId,
                Durum = yeniDurum,
                Aciklama = aciklama,
                Tarih = DateTime.Now
            };
            await _kargoDurumGecmisiRepository.AddAsync(durumGecmisi);

            // 3. Alıcıya otomatik bilgilendirme maili gönder
            var subject = $"Kargonuzun Durumu Güncellendi: {yeniDurum}";
            var body = $"Sayın {kargo.AliciAdSoyad},\n\n" +
                       $"{kargo.TakipNo} takip numaralı kargonuzun durumu \"{yeniDurum}\" olarak güncellenmiştir.\n\n" +
                       (string.IsNullOrWhiteSpace(aciklama) ? "" : $"Not: {aciklama}\n\n") +
                       "İyi günler dileriz.";

            await _mailService.SendMailAsync(kargo.AliciMail, subject, body);

            return true;
        }

        public async Task<IEnumerable<KargoDurumGecmisi>> GetDurumGecmisiAsync(int kargoId)
        {
            return await _kargoDurumGecmisiRepository.GetByKargoIdAsync(kargoId);
        }
    }
}