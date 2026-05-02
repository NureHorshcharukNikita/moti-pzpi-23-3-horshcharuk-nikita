using Microsoft.Data.Sqlite;

namespace Lab3.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IConfiguration configuration, string connectionString)
        {
            var databaseFileName = configuration["AppFiles:DatabaseFileName"]!;
            var initScriptFileName = configuration["AppFiles:InitScriptFileName"]!;

            var dbPath = Path.Combine(AppContext.BaseDirectory, databaseFileName);

            if (File.Exists(dbPath))
                return;

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var sqlPath = Path.Combine(AppContext.BaseDirectory, initScriptFileName);
            var sql = File.ReadAllText(sqlPath);

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}