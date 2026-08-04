using KargoTakip.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KargoTakip.Data.Repository
{
    public interface IKargoDosyaRepository
    {
        Task<IEnumerable<KargoDosya>> GetByKargoIdAsync(int kargoId);
        Task<int> AddAsync(KargoDosya kargoDosya);
    }
}
