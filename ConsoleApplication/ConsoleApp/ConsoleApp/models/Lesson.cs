using Npgsql;
using ConsoleApp.database;

namespace ConsoleApp.models
{
    class Lesson
    {
        public int Id { get; set; }
        public int LectureId { get; set; }
        public int SubjectId { get; set; }
        public int SlotId { get; set; }

        public void CreateLesson(int lectureId, int subjectId, int slotId)
        {
            using var conn = DB.GetConnection();
            string query = @"INSERT INTO Lessons(lecture_id, subject_id, slot_id) 
                             VALUES (@lecture_id, @subject_id, @slot_id)
                             RETURNING lesson_id;";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@lecture_id", lectureId);
            cmd.Parameters.AddWithValue("@subject_id", subjectId);
            cmd.Parameters.AddWithValue("@slot_id", slotId);

            Id = Convert.ToInt32(cmd.ExecuteScalar());
            LectureId = lectureId;
            SubjectId = subjectId;
            SlotId = slotId;
        }
        
        public List<Lesson> GetAll()
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Lessons";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var list = new List<Lesson>();
            while (reader.Read())
            {
                list.Add(new Lesson
                {
                    Id = reader.GetInt32(reader.GetOrdinal("lesson_id")),
                    LectureId = reader.GetInt32(reader.GetOrdinal("lecture_id")),
                    SubjectId = reader.GetInt32(reader.GetOrdinal("subject_id")),
                    SlotId = reader.GetInt32(reader.GetOrdinal("slot_id"))
                });
            }
            return list;
        }

        public Lesson GetById(int id)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Lessons WHERE lesson_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new Lesson
            {
                Id = reader.GetInt32(reader.GetOrdinal("lesson_id")),
                LectureId = reader.GetInt32(reader.GetOrdinal("lecture_id")),
                SubjectId = reader.GetInt32(reader.GetOrdinal("subject_id")),
                SlotId = reader.GetInt32(reader.GetOrdinal("slot_id"))
            };
        }


        public void UpdateLesson(int lectureId, int subjectId, int slotId)
        {
            using var conn = DB.GetConnection();
            string query = @"UPDATE Lessons 
                             SET lecture_id=@lecture_id, subject_id=@subject_id, slot_id=@slot_id 
                             WHERE lesson_id=@id;";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@lecture_id", lectureId);
            cmd.Parameters.AddWithValue("@subject_id", subjectId);
            cmd.Parameters.AddWithValue("@slot_id", slotId);
            cmd.Parameters.AddWithValue("@id", Id);

            cmd.ExecuteNonQuery();

            LectureId = lectureId;
            SubjectId = subjectId;
            SlotId = slotId;
        }

        public void DeleteLesson()
        {
            using var conn = DB.GetConnection();
            string query = "DELETE FROM Lessons WHERE lesson_id=@id;";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.ExecuteNonQuery();
        }

        public void GenerateLessons()
        {
            using var conn = DB.GetConnection();

            for (int i = 1; i <= 20; i++)
            {
                int lectureId = (i % 5 == 0) ? 5 : i % 5;
                int subjectId = (i % 5 == 0) ? 5 : i % 5;
                int slotId = (i % 5 == 0) ? 5 : i % 5;

                string query = @"INSERT INTO Lessons(lecture_id, subject_id, slot_id) 
                                 VALUES (@lecture_id, @subject_id, @slot_id);";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@lecture_id", lectureId);
                cmd.Parameters.AddWithValue("@subject_id", subjectId);
                cmd.Parameters.AddWithValue("@slot_id", slotId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
