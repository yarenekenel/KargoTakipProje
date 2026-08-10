using KargoTakip.Service.Interface;
using KargoTakipProje.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KargoTakipProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IKargoService _kargoService;

        public HomeController(ILogger<HomeController> logger, IKargoService kargoService)
        {
            _logger = logger;
            _kargoService = kargoService;
        }

        public async Task<IActionResult> Index()
        {
            var tumKargolar = await _kargoService.GetAllKargolarAsync();
            var kargoListesi = tumKargolar.ToList();

            var toplamKargo = kargoListesi.Count;
            var bugunOlusturulan = kargoListesi.Count(k => k.OlusturmaTarihi.Date == DateTime.Today);
            var teslimEdilen = kargoListesi.Count(k => k.MevcutDurum == "Teslim Edildi");
            var teslimatOrani = toplamKargo > 0 ? (int)((double)teslimEdilen / toplamKargo * 100) : 0;

            ViewBag.ToplamKargo = toplamKargo;
            ViewBag.BugunOlusturulan = bugunOlusturulan;
            ViewBag.TeslimatOrani = teslimatOrani;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}