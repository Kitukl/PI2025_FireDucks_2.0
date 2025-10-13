using Npgsql;
using ConsoleApp.database;

namespace ConsoleApp.models
{
    class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public Subject createSubject(string name)
        {
            using var conn = DB.GetConnection();
            string query = "INSERT INTO Subjects(name) VALUES (@name) RETURNING subject_id";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("name", name);
            Id = Convert.ToInt32(cmd.ExecuteScalar());
            Name = name;
            return this;
        }

        public void EditSubject(int id, string name)
        {
            using var conn = DB.GetConnection();
            string query = "UPDATE Subjects SET name=@name WHERE subject_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteSubject(int id)
        {
            using var conn = DB.GetConnection();
            string query = "DELETE FROM Subjects WHERE subject_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
