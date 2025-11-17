using System;
using ConsoleApp.database;
using Npgsql;

namespace ConsoleApp.models
{
    class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int UserId { get; set; }
        public int SubjectId { get; set; }


        public Task(int id, string title, string description, DateTime deadline, string status, int userId, int subjectId)
        {
            Id = id;
            Title = title;
            Description = description;
            Deadline = deadline;
            Status = status;
            UserId = userId;
            SubjectId = subjectId;
        }

        public List<Task> GetAll()
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Tasks";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var list = new List<Task>();
            while (reader.Read())
            {
                var t = ReadTask(reader);
                list.Add(t);
            }
            return list;
        }

        public Task GetById(int id)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Tasks WHERE task_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return ReadTask(reader);
        }


        public Task CreateTask()
        {
            using var conn = DB.GetConnection();
            string query = "INSERT INTO Tasks (user_id, description, title, deadline, status) VALUES (@user, @desc, @title, @deadline, @status) RETURNING task_id, creationDate";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@user", UserId);
            cmd.Parameters.AddWithValue("@desc", Description);
            cmd.Parameters.AddWithValue("@title", Title);
            cmd.Parameters.AddWithValue("@deadline", Deadline);
            cmd.Parameters.AddWithValue("@status", Status);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Id = reader.GetInt32(reader.GetOrdinal("task_id"));
                CreationDate = reader.GetDateTime(reader.GetOrdinal("creationDate"));
            }

            return this;
        }

        public void UpdateTask(int id, string title, string desc)
        {
            using var conn = DB.GetConnection();
            string query = "UPDATE Tasks SET title=@title, description=@desc WHERE task_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@desc", (object?)desc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteTask(int id)
        {
            using var conn = DB.GetConnection();
            string query = "DELETE FROM Tasks WHERE task_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
        }

        public void ChangeStatus(string newStatus)
        {
            using var conn = DB.GetConnection();
            string query = "UPDATE Tasks SET status=@newStatus WHERE task_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@newStatus", newStatus);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.ExecuteNonQuery();
            Status = newStatus;
        }

        public void SetDeadline(DateTime newDeadline)
        {
            using var conn = DB.GetConnection();
            string query = "UPDATE Tasks SET deadline = @newDeadline WHERE task_id = @id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@newDeadline", newDeadline);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.ExecuteNonQuery();
            Deadline = newDeadline;
        }

        public List<Task> FilterByStatus(string status)
        {
            using var conn = DB.GetConnection();
            string query = "SELECT * FROM Tasks WHERE status=@status AND user_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@id", UserId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Task>();

            while(reader.Read())
            {
                list.Add(ReadTask(reader));
            }

            return list;
        }

        public List<Task> FilterBySubject(int subjectId)
        {
            using var conn = DB.GetConnection();
            string query = "SELECT * FROM Tasks WHERE subject_id=@subject AND user_id=@id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@subject", subjectId);
            cmd.Parameters.AddWithValue("@id", UserId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Task>();

            while (reader.Read())
            {
                list.Add(ReadTask(reader));
            }

            return list;
        }

        public List<Task> SearchTask(string query)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Tasks WHERE (title ILIKE @q OR description ILIKE @q) AND user_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@q", query);
            cmd.Parameters.AddWithValue("@id", UserId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Task>();

            while (reader.Read())
            {
                list.Add(ReadTask(reader));
            }

            return list;
        }




        private Task ReadTask(NpgsqlDataReader r)
        {
            var task = new Task(
            r.GetInt32(r.GetOrdinal("task_id")),
            r["title"] as string,
            r["description"] as string,
            r.GetDateTime(r.GetOrdinal("deadline")),
            r["status"] as string,
            r.GetInt32(r.GetOrdinal("user_id")),
            r.GetInt32(r.GetOrdinal("subject_id")));

            task.CreationDate = r.GetDateTime(r.GetOrdinal("creationDate"));

            return task;
        }
    }
}
