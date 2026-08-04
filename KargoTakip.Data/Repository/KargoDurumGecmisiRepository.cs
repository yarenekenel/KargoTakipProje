using Dapper;
using KargoTakip.Core.Entity;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Context;
using KargoTakip.Data.Repository;

namespace KargoTakip.Data.Repositories
{
    public class KargoDurumGecmisiRepository : IKargoDurumGecmisiRepository
    {
        private readonly DapperContext _context;

        public KargoDurumGecmisiRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KargoDurumGecmisi>> GetByKargoIdAsync(int kargoId)
        {
            var query = "SELECT * FROM KargoDurumGecmisi WHERE KargoId = @KargoId ORDER BY Tarih DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<KargoDurumGecmisi>(query, new { KargoId = kargoId });
        }

        public async Task<int> AddAsync(KargoDurumGecmisi kargoDurumGecmisi)
        {
            var query = @"INSERT INTO KargoDurumGecmisi (KargoId, Durum, Aciklama, Tarih)
                          VALUES (@KargoId, @Durum, @Aciklama, @Tarih);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, kargoDurumGecmisi);
        }
    }
}