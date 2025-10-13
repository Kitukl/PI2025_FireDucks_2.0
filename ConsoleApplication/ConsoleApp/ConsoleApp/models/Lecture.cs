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
        string query = "INSERT INTO Lectures (name, surname) VALUES (@name, @surname) RETURNING lecture_id, name, surname";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue(@name, name);
        cmd.Parameters.AddWithValue(@surname, surname);

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
}