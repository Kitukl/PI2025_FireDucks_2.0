using Npgsql;
using ConsoleApp.database;

namespace ConsoleApp.models
{
    class Lesson
    {
        public int Id { get; set; }
        public int LectureId { get; set; }
        public int SubjectId { get; set; }


        public Lesson createLesson(int lecturerId, int subjectId)
        {
            using var conn = DB.GetConnection();
            string query = "INSERT INTO Lessons(lecture_id, subject_id) VALUES (@lecturer_id, @subject_id) RETURNING lesson_id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@lecturer_id", lecturerId);
            cmd.Parameters.AddWithValue("@subject_id", subjectId);
            Id = Convert.ToInt32(cmd.ExecuteScalar());
            LectureId = lecturerId;
            SubjectId = subjectId;
            return this;
        }

        public void UpdateLesson(int lecturerId, int subjectId)
        {
            using var conn = DB.GetConnection();
            string sql = "UPDATE Lessons SET lecture_id=@lecturer, subject_id=@subject WHERE lesson_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@lecturer", lecturerId);
            cmd.Parameters.AddWithValue("@subject", subjectId);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.ExecuteNonQuery();
        }

        public void DeleteLesson()
        {
            using var conn = DB.GetConnection();
            string sql = "DELETE FROM Lessons WHERE lesson_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", Id);
            cmd.ExecuteNonQuery();
        }
    }
}
