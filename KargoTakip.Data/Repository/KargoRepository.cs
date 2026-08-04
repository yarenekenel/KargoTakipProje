using Dapper;
using KargoTakip.Core.Entity;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Context;
using KargoTakip.Data.Repository;

namespace KargoTakip.Data.Repositories
{
    public class KargoRepository : IKargoRepository
    {
        private readonly DapperContext _context;

        public KargoRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Kargo>> GetAllAsync()
        {
            var query = "SELECT * FROM Kargo ORDER BY OlusturmaTarihi DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Kargo>(query);
        }

        public async Task<Kargo?> GetByIdAsync(int kargoId)
        {
            var query = "SELECT * FROM Kargo WHERE KargoId = @KargoId";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Kargo>(query, new { KargoId = kargoId });
        }

        public async Task<Kargo?> GetByTakipNoAsync(string takipNo)
        {
            var query = "SELECT * FROM Kargo WHERE TakipNo = @TakipNo";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Kargo>(query, new { TakipNo = takipNo });
        }

        public async Task<int> AddAsync(Kargo kargo)
        {
            var query = @"INSERT INTO Kargo (TakipNo, GonderenAdSoyad, AliciAdSoyad, AliciMail, MevcutDurum, OlusturmaTarihi)
                          VALUES (@TakipNo, @GonderenAdSoyad, @AliciAdSoyad, @AliciMail, @MevcutDurum, @OlusturmaTarihi);
                          SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<int>(query, kargo);
        }

        public async Task<bool> UpdateDurumAsync(int kargoId, string yeniDurum)
        {
            var query = "UPDATE Kargo SET MevcutDurum = @MevcutDurum WHERE KargoId = @KargoId";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { MevcutDurum = yeniDurum, KargoId = kargoId });
            return affectedRows > 0;
        }
    }
}