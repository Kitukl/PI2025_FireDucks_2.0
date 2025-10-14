using System.Globalization;
using ConsoleApp.database;
using Npgsql;

namespace ConsoleApp.models;

public class Lecture
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public string Surname { get; set; }

    public Lecture AddLecture(string name, string surname)
    {
        using var conn = DB.GetConnection();
        string query = @"INSERT INTO Lectures (""name"", ""surname"") VALUES (@name, @surname) RETURNING lecture_id, ""name"", surname";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@surname", surname);

        using var reader = cmd.ExecuteReader();
        
        if (reader.Read())
        {
            var id = reader.GetInt32(reader.GetOrdinal("lecture_id"));
            return new Lecture{Id = id, Name = name, Surname = surname};
        }
        else
        {
            throw new Exception("Викладача не занесено в базу даних");
        }
    }

    public void RemoveLecture(int id)
    {
        using var conn = DB.GetConnection();
        string query = "DELETE FROM Lectures WHERE lecture_id = @id RETURNING lecture_id";
        
        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
        {
            throw new Exception("Викладача не видалено з бази даних");
        }
    }

    public void EditLecture(int id, string name, string surname)
    {
        using var conn = DB.GetConnection();
        string query = "UPDATE Lectures SET name=@name, surname=@surname WHERE lecture_id=@id RETURNING lecture_id";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@surname", surname);
        cmd.Parameters.AddWithValue("@id", id);

        Name = name;
        Surname = surname;

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
        {
            throw new Exception("Не вдалось змінити викладача");
        }
    }
    
    public void GenerateLectures()
    {
        using var conn = DB.GetConnection();

        for (int i = 1; i <= 20; i++)
        {
            string name = $"LectureName{i}";
            string surname = $"LectureSurname{i}";

            string query = @"INSERT INTO Lectures (""name"", ""surname"") 
                             VALUES (@name, @surname);";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@surname", surname);

            cmd.ExecuteNonQuery();
        }
    }
}