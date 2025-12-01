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
        public List<Subject> GetAll()
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Subjects";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var list = new List<Subject>();
            while (reader.Read())
            {
                list.Add(new Subject
                {
                    Id = reader.GetInt32(reader.GetOrdinal("subject_id")),
                    Name = reader["name"] as string
                });
            }
            return list;
        }

        public Subject GetById(int id)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Subjects WHERE subject_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Subject
            {
                Id = reader.GetInt32(reader.GetOrdinal("subject_id")),
                Name = reader["name"] as string
            };
        }


        public void GenerateSubjects()
        {
            var subjects = new List<string>
            {
                "Дискретна математика",
                "Матаналіз",
                "Програмування",
                "Навчальна практика",
                "ООЕІ",
                "Алгебра",
                "ТІМС",
                "Матлогіка",
                "Диф рівняння",
                "ТІК",
                "Чисельні методи",
                "Бази даних",
                "Програмна інженерія",
                "Веб розробка",
                "Теорія алгоритмів",
                "СШІ",
                "ПТАРО"
            };

            using var conn = DB.GetConnection();

            foreach (var name in subjects)
            {
                using var cmd = new NpgsqlCommand("INSERT INTO Subjects(name) VALUES (@name);", conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
