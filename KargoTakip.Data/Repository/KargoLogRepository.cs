using Dapper;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Context;
using KargoTakip.Data.Repository;

namespace KargoTakip.Data.Repositories
{
    public class KargoLogRepository : IKargoLogRepository
    {
        private readonly DapperContext _context;

        public KargoLogRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KargoLog>> GetByKargoIdAsync(int kargoId)
        {
            var query = "SELECT * FROM KargoLog WHERE KargoId = @KargoId ORDER BY Tarih DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<KargoLog>(query, new { KargoId = kargoId });
        }

        public async Task<int> AddAsync(KargoLog kargoLog)
        {
            var query = @"INSERT INTO KargoLog (KargoId, LogTuru, Aciklama, Tarih)
                          VALUES (@KargoId, @LogTuru, @Aciklama, @Tarih);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, kargoLog);
        }

        // Aynı log türü daha önce bu kargo için eklenmiş mi kontrol eder (tekrar tekrar log düşmesini engellemek için)
        public async Task<bool> LogVarMiAsync(int kargoId, string logTuru)
        {
            var query = "SELECT COUNT(1) FROM KargoLog WHERE KargoId = @KargoId AND LogTuru = @LogTuru";
            using var connection = _context.CreateConnection();
            var sayi = await connection.QuerySingleAsync<int>(query, new { KargoId = kargoId, LogTuru = logTuru });
            return sayi > 0;
        }
    }
}