using Dapper;
using KargoTakip.Core.Entity;
using KargoTakip.Data.Context;

namespace KargoTakip.Data.Repository
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
        public async Task<(IEnumerable<Kargo> Kargolar, int ToplamKayit)> SearchAsync(string? aramaMetni, string? durum, int sayfaNo, int sayfaBoyutu)
        {
            var whereClause = "WHERE 1=1";

            if (!string.IsNullOrWhiteSpace(aramaMetni))
            {
                whereClause += " AND (TakipNo LIKE @Arama OR AliciAdSoyad LIKE @Arama OR GonderenAdSoyad LIKE @Arama)";
            }

            if (!string.IsNullOrWhiteSpace(durum))
            {
                whereClause += " AND MevcutDurum = @Durum";
            }

            var countQuery = $"SELECT COUNT(*) FROM Kargo {whereClause}";

            var dataQuery = $@"SELECT * FROM Kargo {whereClause}
                        ORDER BY OlusturmaTarihi DESC
                        OFFSET @Offset ROWS FETCH NEXT @SayfaBoyutu ROWS ONLY";

            var parametreler = new
            {
                Arama = $"%{aramaMetni}%",
                Durum = durum,
                Offset = (sayfaNo - 1) * sayfaBoyutu,
                SayfaBoyutu = sayfaBoyutu
            };

            using var connection = _context.CreateConnection();

            var toplamKayit = await connection.QuerySingleAsync<int>(countQuery, parametreler);
            var kargolar = await connection.QueryAsync<Kargo>(dataQuery, parametreler);

            return (kargolar, toplamKayit);
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

        public async Task<bool> UpdateTakipNoAsync(int kargoId, string takipNo)
        {
            var query = "UPDATE Kargo SET TakipNo = @TakipNo WHERE KargoId = @KargoId";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(query, new { TakipNo = takipNo, KargoId = kargoId });
            return affectedRows > 0;
        }
    }
}