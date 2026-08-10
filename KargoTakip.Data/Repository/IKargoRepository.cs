using KargoTakip.Core.Entity;

namespace KargoTakip.Data.Repository
{
    public interface IKargoRepository
    {
        Task<IEnumerable<Kargo>> GetAllAsync();
        Task<Kargo?> GetByIdAsync(int kargoId);
        Task<Kargo?> GetByTakipNoAsync(string takipNo);
        Task<int> AddAsync(Kargo kargo);
        Task<bool> UpdateDurumAsync(int kargoId, string yeniDurum);
        Task<bool> UpdateTakipNoAsync(int kargoId, string takipNo);
        Task<(IEnumerable<Kargo> Kargolar, int ToplamKayit)> SearchAsync(string? aramaMetni, string? durum, int sayfaNo, int sayfaBoyutu);
    }
}