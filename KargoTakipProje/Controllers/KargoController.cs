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
        public async Task<IActionResult> Index()
        {
            var kargolar = await _kargoService.GetAllKargolarAsync();
            return View(kargolar);
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
                TakipNo = model.TakipNo,
                GonderenAdSoyad = model.GonderenAdSoyad,
                AliciAdSoyad = model.AliciAdSoyad,
                AliciMail = model.AliciMail
            };

            await _kargoService.KargoOlusturAsync(kargo);
            TempData["BasariMesaji"] = "Kargo başarıyla oluşturuldu.";

            return RedirectToAction("Index");
        }

        // Kargo detayı + durum geçmişi + dosyalar
        public async Task<IActionResult> Detay(int id)
        {
            var kargo = await _kargoService.GetKargoByIdAsync(id);
            if (kargo == null)
                return NotFound();

            var durumGecmisi = await _kargoService.GetDurumGecmisiAsync(id);
            var dosyalar = await _dosyaService.GetDosyalarByKargoIdAsync(id);

            ViewBag.DurumGecmisi = durumGecmisi;
            ViewBag.Dosyalar = dosyalar;

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
    }
}