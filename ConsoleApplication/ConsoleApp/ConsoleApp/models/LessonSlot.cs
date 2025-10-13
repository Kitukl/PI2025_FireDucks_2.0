using Npgsql;
using ConsoleApp.database;

namespace ConsoleApp.models
{
    public class LessonSlot
    {
        public int Id { get; set; }
        public TimeOnly StartLesson { get; set; }
        public TimeOnly EndLesson { get; set; }


        public void SetTimeSlot(TimeOnly start, TimeOnly end)
        {
            StartLesson = start;
            EndLesson = end;
        }


        public void UpdateTimeSlot(int id, TimeOnly start, TimeOnly end)
        {
            using var conn = DB.GetConnection();
            string sql = "UPDATE Lessons_Slots SET startLesson=@start, endLesson=@end WHERE slot_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@start", NpgsqlTypes.NpgsqlDbType.Time, start.ToTimeSpan());
            cmd.Parameters.AddWithValue("@end", NpgsqlTypes.NpgsqlDbType.Time, end.ToTimeSpan());
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public LessonSlot CreateSlot()
        {
            using var conn = DB.GetConnection();
            string sql = @"INSERT INTO Lessons_Slots (startLesson, endLesson)
                       VALUES (@start, @end)
                       RETURNING slot_id;";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@start", NpgsqlTypes.NpgsqlDbType.Time, StartLesson.ToTimeSpan());
            cmd.Parameters.AddWithValue("@end", NpgsqlTypes.NpgsqlDbType.Time, EndLesson.ToTimeSpan());
            Id = Convert.ToInt32(cmd.ExecuteScalar());
            return this;
        }
    }
}
