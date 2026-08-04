using KargoTakip.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace KargoTakip.Service.Interface
{
    public interface IDosyaService
    {
        Task<int> DosyaKaydetAsync(int kargoId, IFormFile dosya, string? dosyaTuru);
        Task<IEnumerable<KargoDosya>> GetDosyalarByKargoIdAsync(int kargoId);
    }
}
