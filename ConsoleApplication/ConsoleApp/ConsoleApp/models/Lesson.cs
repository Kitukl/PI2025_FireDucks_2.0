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
