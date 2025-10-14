using ConsoleApp.database;
using Npgsql;
using System.Collections.Generic;

namespace ConsoleApp.models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public void GenerateCategories()
    {
        using var conn = DB.GetConnection();
        var names = new List<string>
        {
            "Technology",
            "Health",
            "Education",
            "Entertainment",
            "Sports"
        };

        foreach (var n in names)
        {
            string query = @"INSERT INTO Categories(""name"") VALUES (@n)";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@n", n);
            cmd.ExecuteNonQuery();
        }
    }
}