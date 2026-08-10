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
        private readonly IKargoLogRepository _kargoLogRepository;
        private readonly IMailService _mailService;

        public KargoService(
            IKargoRepository kargoRepository,
            IKargoDurumGecmisiRepository kargoDurumGecmisiRepository,
            IKargoLogRepository kargoLogRepository,
            IMailService mailService)
        {
            _kargoRepository = kargoRepository;
            _kargoDurumGecmisiRepository = kargoDurumGecmisiRepository;
            _kargoLogRepository = kargoLogRepository;
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
            kargo.TakipNo = "GEÇİCİ";

            var yeniKargoId = await _kargoRepository.AddAsync(kargo);

            var takipNo = $"TR{DateTime.Now.Year}{yeniKargoId:D6}";
            await _kargoRepository.UpdateTakipNoAsync(yeniKargoId, takipNo);

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

            var guncellendi = await _kargoRepository.UpdateDurumAsync(kargoId, yeniDurum);
            if (!guncellendi)
                return false;

            var durumGecmisi = new KargoDurumGecmisi
            {
                KargoId = kargoId,
                Durum = yeniDurum,
                Aciklama = aciklama,
                Tarih = DateTime.Now
            };
            await _kargoDurumGecmisiRepository.AddAsync(durumGecmisi);

            var subject = $"📦 Kargonuz {yeniDurum} - {kargo.TakipNo}";
            var body = MailTemplateOlustur(kargo, yeniDurum, aciklama);

            await _mailService.SendMailAsync(kargo.AliciMail, subject, body);

            return true;
        }

        public async Task<IEnumerable<KargoDurumGecmisi>> GetDurumGecmisiAsync(int kargoId)
        {
            return await _kargoDurumGecmisiRepository.GetByKargoIdAsync(kargoId);
        }

        public async Task OtomatikLogKontrolAsync(int kargoId)
        {
            var kargo = await _kargoRepository.GetByIdAsync(kargoId);
            if (kargo == null)
                return;

            var durumGecmisi = await _kargoDurumGecmisiRepository.GetByKargoIdAsync(kargoId);
            if (durumGecmisi == null || !durumGecmisi.Any())
                return;

            var sonDurumKaydi = durumGecmisi
                .Where(d => d.Durum == kargo.MevcutDurum)
                .OrderByDescending(d => d.Tarih)
                .FirstOrDefault();

            if (sonDurumKaydi == null)
                return;

            var gecenSure = DateTime.Now - sonDurumKaydi.Tarih;

            if (kargo.MevcutDurum == "İade Sürecinde" && gecenSure.TotalDays >= 3)
            {
                var logTuru = "İade Gecikmesi";
                var zatenVarMi = await _kargoLogRepository.LogVarMiAsync(kargoId, logTuru);
                if (!zatenVarMi)
                {
                    await _kargoLogRepository.AddAsync(new KargoLog
                    {
                        KargoId = kargoId,
                        LogTuru = logTuru,
                        Aciklama = $"Kargo {(int)gecenSure.TotalDays} gündür iade sürecinde, müşteri iade işlemini tamamlamadı.",
                        Tarih = DateTime.Now
                    });
                }
            }

            if (kargo.MevcutDurum == "Kargoya Verildi" && gecenSure.TotalDays >= 5)
            {
                var logTuru = "Teslimat Gecikmesi";
                var zatenVarMi = await _kargoLogRepository.LogVarMiAsync(kargoId, logTuru);
                if (!zatenVarMi)
                {
                    await _kargoLogRepository.AddAsync(new KargoLog
                    {
                        KargoId = kargoId,
                        LogTuru = logTuru,
                        Aciklama = $"Kargo {(int)gecenSure.TotalDays} gündür 'Kargoya Verildi' durumunda, kargocu teslim alamamış olabilir.",
                        Tarih = DateTime.Now
                    });
                }
            }

            if (kargo.MevcutDurum == "Yolda" && gecenSure.TotalDays >= 4)
            {
                var logTuru = "Yolda Gecikmesi";
                var zatenVarMi = await _kargoLogRepository.LogVarMiAsync(kargoId, logTuru);
                if (!zatenVarMi)
                {
                    await _kargoLogRepository.AddAsync(new KargoLog
                    {
                        KargoId = kargoId,
                        LogTuru = logTuru,
                        Aciklama = $"Kargo {(int)gecenSure.TotalDays} gündür yolda, beklenenden uzun sürüyor.",
                        Tarih = DateTime.Now
                    });
                }
            }
        }

        public async Task<IEnumerable<KargoLog>> GetLogsByKargoIdAsync(int kargoId)
        {
            return await _kargoLogRepository.GetByKargoIdAsync(kargoId);
        }

        public async Task ManuelLogEkleAsync(int kargoId, string logTuru, string aciklama)
        {
            await _kargoLogRepository.AddAsync(new KargoLog
            {
                KargoId = kargoId,
                LogTuru = logTuru,
                Aciklama = aciklama,
                Tarih = DateTime.Now
            });
        }

        public async Task<bool> TakipNoGuncelleAsync(int kargoId, string yeniTakipNo)
        {
            return await _kargoRepository.UpdateTakipNoAsync(kargoId, yeniTakipNo);
        }

        public async Task<(IEnumerable<Kargo> Kargolar, int ToplamKayit)> SearchKargolarAsync(string? aramaMetni, string? durum, int sayfaNo, int sayfaBoyutu)
        {
            return await _kargoRepository.SearchAsync(aramaMetni, durum, sayfaNo, sayfaBoyutu);
        }

        private string MailTemplateOlustur(Kargo kargo, string yeniDurum, string? aciklama)
        {
            var (renk, ikon, mesaj) = yeniDurum switch
            {
                "Hazırlanıyor" => ("#4f46e5", "📦", "Kargonuz hazırlanıyor."),
                "Kargoya Verildi" => ("#1d4ed8", "🚚", "Kargonuz kargo firmasına teslim edildi."),
                "Yolda" => ("#b45309", "🛣️", "Kargonuz yolda, adresinize doğru ilerliyor."),
                "Teslim Edildi" => ("#15803d", "✅", "Kargonuz başarıyla teslim edildi."),
                "İade Sürecinde" => ("#b91c1c", "↩️", "Kargonuz için iade süreci başlatıldı."),
                "İade Edildi" => ("#7e22ce", "📮", "Kargonuzun iade işlemi tamamlandı."),
                "İptal Edildi" => ("#6b7280", "🚫", "Kargonuz iptal edilmiştir."),
                _ => ("#4f46e5", "📦", "Kargonuzun durumu güncellendi.")
            };

            return $@"
            <!DOCTYPE html>
            <html lang='tr'>
            <body style='margin:0; padding:0; background-color:#f4f4f7; font-family: Segoe UI, Arial, sans-serif;'>
                <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f4f7; padding: 30px 0;'>
                    <tr>
                        <td align='center'>
                            <table role='presentation' width='600' cellpadding='0' cellspacing='0' style='background-color:#ffffff; border-radius:12px; overflow:hidden; box-shadow: 0 2px 12px rgba(0,0,0,0.06);'>
                                <tr>
                                    <td style='background-color:{renk}; padding: 28px 32px;'>
                                        <span style='color:#ffffff; font-size:22px; font-weight:700;'>{ikon} KargoTakip</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='padding: 32px;'>
                                        <p style='font-size:16px; color:#111827; margin:0 0 16px 0;'>
                                            Sayın <strong>{kargo.AliciAdSoyad}</strong>,
                                        </p>
                                        <p style='font-size:15px; color:#374151; line-height:1.6; margin:0 0 24px 0;'>
                                            {mesaj}
                                        </p>
                                        <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f8f9fc; border-radius:10px; padding: 20px; margin-bottom: 24px;'>
                                            <tr>
                                                <td style='padding: 6px 0; font-size:14px; color:#6b7280;'>Takip Numarası</td>
                                                <td style='padding: 6px 0; font-size:14px; color:#111827; font-weight:600; text-align:right;'>{kargo.TakipNo}</td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 6px 0; font-size:14px; color:#6b7280;'>Gönderen</td>
                                                <td style='padding: 6px 0; font-size:14px; color:#111827; font-weight:600; text-align:right;'>{kargo.GonderenAdSoyad}</td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 6px 0; font-size:14px; color:#6b7280;'>Güncel Durum</td>
                                                <td style='padding: 6px 0; text-align:right;'>
                                                    <span style='background-color:{renk}22; color:{renk}; font-weight:600; font-size:13px; padding: 5px 14px; border-radius:20px;'>{yeniDurum}</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style='padding: 6px 0; font-size:14px; color:#6b7280;'>Güncellenme Tarihi</td>
                                                <td style='padding: 6px 0; font-size:14px; color:#111827; font-weight:600; text-align:right;'>{DateTime.Now:dd.MM.yyyy HH:mm}</td>
                                            </tr>
                                        </table>
                                        {(string.IsNullOrWhiteSpace(aciklama) ? "" : $@"
                                        <p style='font-size:14px; color:#6b7280; background-color:#fffbeb; border-left: 3px solid #d97706; padding: 12px 16px; border-radius: 6px; margin: 0 0 24px 0;'>
                                            <strong>Not:</strong> {aciklama}
                                        </p>")}
                                        <p style='font-size:13px; color:#9ca3af; margin-top: 32px;'>
                                            Bu e-posta, {kargo.TakipNo} takip numaralı kargonuzla ilgili otomatik olarak gönderilmiştir.
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td style='background-color:#f8f9fc; padding: 20px 32px; text-align:center;'>
                                        <span style='font-size:12px; color:#9ca3af;'>© 2026 KargoTakip. Tüm hakları saklıdır.</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";
        }
    }
}