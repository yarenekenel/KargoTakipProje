using KargoTakip.Core.Entity;
using KargoTakip.Service.Interface;
using KargoTakipProje.Models;
using Microsoft.AspNetCore.Mvc;

namespace KargoTakipProje.Controllers
{
    public class KargoController : Controller
    {
        private readonly IKargoService _kargoService;
        private readonly IDosyaService _dosyaService;

        public KargoController(IKargoService kargoService, IDosyaService dosyaService)
        {
            _kargoService = kargoService;
            _dosyaService = dosyaService;
        }

        // Index: Tüm kargoların listesi
        public async Task<IActionResult> Index(string? aramaMetni, string? durumFiltre, int sayfaNo = 1)
        {
            int sayfaBoyutu = 10;

            var (kargolar, toplamKayit) = await _kargoService.SearchKargolarAsync(aramaMetni, durumFiltre, sayfaNo, sayfaBoyutu);

            var model = new KargoFiltreViewModel
            {
                AramaMetni = aramaMetni,
                DurumFiltre = durumFiltre,
                Kargolar = kargolar.ToList(),
                SayfaNo = sayfaNo,
                SayfaBoyutu = sayfaBoyutu,
                ToplamKayit = toplamKayit
            };

            return View(model);
        }

        // Yeni kargo kaydı oluşturma formu (GET)
        public IActionResult Olustur()
        {
            return View();
        }

        // Yeni kargo kaydı oluşturma (POST)
        [HttpPost]
        public async Task<IActionResult> Olustur(KargoOlusturViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var kargo = new Kargo
            {
                GonderenAdSoyad = model.GonderenAdSoyad,
                AliciAdSoyad = model.AliciAdSoyad,
                AliciMail = model.AliciMail
            };

            await _kargoService.KargoOlusturAsync(kargo);
            TempData["BasariMesaji"] = "Kargo başarıyla oluşturuldu.";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detay(int id)
        {
            var kargo = await _kargoService.GetKargoByIdAsync(id);
            if (kargo == null)
                return NotFound();

            // Sayfa her açıldığında otomatik log kontrolü yapılır
            await _kargoService.OtomatikLogKontrolAsync(id);

            var durumGecmisi = await _kargoService.GetDurumGecmisiAsync(id);
            var dosyalar = await _dosyaService.GetDosyalarByKargoIdAsync(id);
            var loglar = await _kargoService.GetLogsByKargoIdAsync(id);

            ViewBag.DurumGecmisi = durumGecmisi;
            ViewBag.Dosyalar = dosyalar;
            ViewBag.Loglar = loglar;

            return View(kargo);
        }

        // Durum güncelleme (POST) — otomatik mail bu adımda tetiklenir
        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(KargoDurumGuncelleViewModel model)
        {
            await _kargoService.DurumGuncelleAsync(model.KargoId, model.YeniDurum, model.Aciklama);
            TempData["BasariMesaji"] = "Durum başarıyla güncellendi ve alıcıya bilgilendirme maili gönderildi.";
            return RedirectToAction("Detay", new { id = model.KargoId });
        }

        // Dosya/fotoğraf yükleme (POST)
        [HttpPost]
        public async Task<IActionResult> DosyaYukle(DosyaYukleViewModel model)
        {
            if (model.Dosya != null && model.Dosya.Length > 0)
            {
                await _dosyaService.DosyaKaydetAsync(model.KargoId, model.Dosya, model.DosyaTuru);
                TempData["BasariMesaji"] = "Dosya başarıyla yüklendi.";
            }
            else
            {
                TempData["HataMesaji"] = "Lütfen bir dosya seçin.";
            }

            return RedirectToAction("Detay", new { id = model.KargoId });
        }

        [HttpPost]
        public async Task<IActionResult> DosyaSil(int kargoDosyaId, int kargoId)
        {
            var basarili = await _dosyaService.DosyaSilAsync(kargoDosyaId);

            if (basarili)
                TempData["BasariMesaji"] = "Dosya başarıyla silindi.";
            else
                TempData["HataMesaji"] = "Dosya silinemedi.";

            return RedirectToAction("Detay", new { id = kargoId });
        }

        // Müşteri: Takip numarası arama ekranı (GET)
        [HttpGet]
        public IActionResult MusteriTakip()
        {
            return View();
        }

        // Müşteri: Takip numarasına göre kargo detayını gösterir
        [HttpGet]
        public async Task<IActionResult> MusteriTakipSonuc(string takipNo)
        {
            if (string.IsNullOrWhiteSpace(takipNo))
            {
                TempData["HataMesaji"] = "Lütfen bir takip numarası girin.";
                return RedirectToAction("MusteriTakip");
            }

            var kargo = await _kargoService.GetKargoByTakipNoAsync(takipNo);
            if (kargo == null)
            {
                TempData["HataMesaji"] = "Bu takip numarasına ait bir kargo kaydı bulunamadı. Lütfen takip numaranızı kontrol edip tekrar deneyin.";
                return RedirectToAction("MusteriTakip");
            }

            var durumGecmisi = await _kargoService.GetDurumGecmisiAsync(kargo.KargoId);
            var dosyalar = await _dosyaService.GetDosyalarByKargoIdAsync(kargo.KargoId);

            ViewBag.DurumGecmisi = durumGecmisi;
            ViewBag.Dosyalar = dosyalar;

            return View("MusteriTakipSonuc", kargo);
        }

        [HttpPost]
        public async Task<IActionResult> LogEkle(int kargoId, string logTuru, string aciklama)
        {
            await _kargoService.ManuelLogEkleAsync(kargoId, logTuru, aciklama);
            TempData["BasariMesaji"] = "Log kaydı eklendi.";
            return RedirectToAction("Detay", new { id = kargoId });
        }

        [HttpPost]
        public async Task<IActionResult> KargoIptal(int kargoId)
        {
            var basarili = await _kargoService.DurumGuncelleAsync(kargoId, "İptal Edildi", "Kargo iptal edildi.");

            if (basarili)
                TempData["BasariMesaji"] = "Kargo başarıyla iptal edildi.";
            else
                TempData["HataMesaji"] = "Kargo iptal edilemedi.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> TakipNoDuzenle(int kargoId, string yeniTakipNo)
        {
            try
            {
                var basarili = await _kargoService.TakipNoGuncelleAsync(kargoId, yeniTakipNo);

                if (basarili)
                    TempData["BasariMesaji"] = "Takip numarası başarıyla güncellendi.";
                else
                    TempData["HataMesaji"] = "Takip numarası güncellenemedi.";
            }
            catch (Exception ex) when (ex.Message.Contains("UNIQUE") || (ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE")))
            {
                TempData["HataMesaji"] = $"'{yeniTakipNo}' takip numarası zaten başka bir kargoda kullanılıyor. Lütfen farklı bir numara girin.";
            }

            return RedirectToAction("Detay", new { id = kargoId });
        }
    }
}