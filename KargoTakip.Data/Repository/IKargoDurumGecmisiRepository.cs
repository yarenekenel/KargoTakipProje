using KargoTakip.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Data.Repository
{
    public interface IKargoDurumGecmisiRepository
    {
        Task<IEnumerable<KargoDurumGecmisi>> GetByKargoIdAsync(int kargoId);
        Task<int> AddAsync(KargoDurumGecmisi kargoDurumGecmisi);
    }
}
