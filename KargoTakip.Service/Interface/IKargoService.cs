using KargoTakip.Core.Entity;

namespace KargoTakip.Service.Interface
{
    public interface IKargoService
    {
        Task<IEnumerable<Kargo>> GetAllKargolarAsync();
        Task<Kargo?> GetKargoByIdAsync(int kargoId);
        Task<Kargo?> GetKargoByTakipNoAsync(string takipNo);
        Task<int> KargoOlusturAsync(Kargo kargo);
        Task<bool> DurumGuncelleAsync(int kargoId, string yeniDurum, string? aciklama);
        Task<IEnumerable<KargoDurumGecmisi>> GetDurumGecmisiAsync(int kargoId);
        Task OtomatikLogKontrolAsync(int kargoId);
        Task<IEnumerable<KargoLog>> GetLogsByKargoIdAsync(int kargoId);
        Task ManuelLogEkleAsync(int kargoId, string logTuru, string aciklama);
        Task<bool> TakipNoGuncelleAsync(int kargoId, string yeniTakipNo);
        Task<(IEnumerable<Kargo> Kargolar, int ToplamKayit)> SearchKargolarAsync(string? aramaMetni, string? durum, int sayfaNo, int sayfaBoyutu);
    }
}