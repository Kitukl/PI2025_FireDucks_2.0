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
    public List<Category> GetAll()
    {
        using var conn = DB.GetConnection();
        string sql = "SELECT * FROM Categories";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        var list = new List<Category>();
        while (reader.Read())
        {
            list.Add(new Category
            {
                Id = reader.GetInt32(reader.GetOrdinal("category_id")),
                Name = reader["name"] as string
            });
        }

        return list;
    }

    public Category GetById(int id)
    {
        using var conn = DB.GetConnection();
        string sql = "SELECT * FROM Categories WHERE category_id=@id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new Category
        {
            Id = reader.GetInt32(reader.GetOrdinal("category_id")),
            Name = reader["name"] as string
        };
    }

}