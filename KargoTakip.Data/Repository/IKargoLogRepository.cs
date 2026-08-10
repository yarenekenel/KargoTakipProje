using KargoTakip.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Data.Repository
{
    public interface IKargoLogRepository
    {
        Task<IEnumerable<KargoLog>> GetByKargoIdAsync(int kargoId);
        Task<int> AddAsync(KargoLog kargoLog);
        Task<bool> LogVarMiAsync(int kargoId, string logTuru);
    }
}
