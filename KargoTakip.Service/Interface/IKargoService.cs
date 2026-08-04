using KargoTakip.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
