using Dapper;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Context;

namespace KargoTakip.Data.Repositories
{
    public class KargoDosyaRepository : IKargoDosyaRepository
    {
        private readonly DapperContext _context;

        public KargoDosyaRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KargoDosya>> GetByKargoIdAsync(int kargoId)
        {
            var query = "SELECT * FROM KargoDosya WHERE KargoId = @KargoId ORDER BY YuklemeTarihi DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<KargoDosya>(query, new { KargoId = kargoId });
        }

        public async Task<KargoDosya?> GetByIdAsync(int kargoDosyaId)
        {
            var query = "SELECT * FROM KargoDosya WHERE KargoDosyaId = @KargoDosyaId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<KargoDosya>(query, new { KargoDosyaId = kargoDosyaId });
        }

        public async Task<int> AddAsync(KargoDosya kargoDosya)
        {
            var query = @"INSERT INTO KargoDosya (KargoId, DosyaYolu, DosyaTuru, YuklemeTarihi)
                          VALUES (@KargoId, @DosyaYolu, @DosyaTuru, @YuklemeTarihi);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, kargoDosya);
        }

        public async Task<bool> DeleteAsync(int kargoDosyaId)
        {
            var query = "DELETE FROM KargoDosya WHERE KargoDosyaId = @KargoDosyaId";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { KargoDosyaId = kargoDosyaId });
            return affectedRows > 0;
        }
    }
}