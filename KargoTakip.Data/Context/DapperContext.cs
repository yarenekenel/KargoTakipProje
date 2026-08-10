using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace KargoTakip.Data.Context
{
    //SqlConnection nesnesini merkezi bir yerden üretmek — her repository kendi bağlantı stringini tekrar yazmasın.
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
