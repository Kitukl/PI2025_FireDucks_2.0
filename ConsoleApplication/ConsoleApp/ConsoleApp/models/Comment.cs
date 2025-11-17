using Npgsql;
using ConsoleApp.database;

namespace ConsoleApp.models
{
    class Comment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int TaskId { get; set; }
        

        public Comment AddComment(string description, int task_id)
        {
            using var conn = DB.GetConnection();
            string query = "INSERT INTO Comments(task_id, description) VALUES (@task, @desc) RETURNING comment_id, creationDate";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@task", task_id);
            cmd.Parameters.AddWithValue("@desc", description);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Id = reader.GetInt32(reader.GetOrdinal("comment_id"));
                CreationDate = reader.GetDateTime(reader.GetOrdinal("creationDate"));
            }
            Description = description;
            TaskId = task_id;

            return this;
        }

        public void EditComment(int id, string newDescription)
        {
            using var conn = DB.GetConnection();
            string sql = "UPDATE Comments SET description=@desc WHERE comment_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@desc", newDescription);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteComment(int id)
        {
            using var conn = DB.GetConnection();
            string sql = "DELETE FROM Comments WHERE comment_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Comment> GetByTask(int taskId)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Comments WHERE task_id=@task";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@task", taskId);
            using var reader = cmd.ExecuteReader();
            var list = new List<Comment>();
            while (reader.Read())
            {
                list.Add(new Comment
                {
                    Id = reader.GetInt32(reader.GetOrdinal("comment_id")),
                    Description = reader["description"] as string,
                    CreationDate = reader.GetDateTime(reader.GetOrdinal("creationDate")),
                    TaskId = taskId
                });
            }
            return list;
        }
        public List<Comment> GetAll()
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Comments";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var list = new List<Comment>();
            while (reader.Read())
            {
                list.Add(new Comment
                {
                    Id = reader.GetInt32(reader.GetOrdinal("comment_id")),
                    Description = reader["description"] as string,
                    CreationDate = reader.GetDateTime(reader.GetOrdinal("creationDate")),
                    TaskId = reader.GetInt32(reader.GetOrdinal("task_id"))
                });
            }
            return list;
        }

        public Comment GetById(int id)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Comments WHERE comment_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Comment
            {
                Id = reader.GetInt32(reader.GetOrdinal("comment_id")),
                Description = reader["description"] as string,
                CreationDate = reader.GetDateTime(reader.GetOrdinal("creationDate")),
                TaskId = reader.GetInt32(reader.GetOrdinal("task_id"))
            };
        }
    }
}
