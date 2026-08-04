using KargoTakip.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Data.Repository
{
    public interface IKargoRepository
    {
        Task<IEnumerable<Kargo>> GetAllAsync();
        Task<Kargo?> GetByIdAsync(int kargoId);
        Task<Kargo?> GetByTakipNoAsync(string takipNo);
        Task<int> AddAsync(Kargo kargo);
        Task<bool> UpdateDurumAsync(int kargoId, string yeniDurum);
    }
}
