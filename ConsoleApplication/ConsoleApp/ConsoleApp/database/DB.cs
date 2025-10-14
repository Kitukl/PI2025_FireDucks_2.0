using System;
using DotNetEnv;
using Npgsql;

namespace ConsoleApp.database
{
    public static class DB
    {
        static DB()
        {
            Env.Load();
            Console.WriteLine("DEBUG: DUCK_DB_CONN = " + Environment.GetEnvironmentVariable("DUCK_DB_CONN"));
        }

        public static NpgsqlConnection GetConnection()
        {
            string connectionString = Environment.GetEnvironmentVariable("DUCK_DB_CONN")
                ?? throw new Exception("Environment variable DUCK_DB_CONN not set");

            var conn = new NpgsqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}

