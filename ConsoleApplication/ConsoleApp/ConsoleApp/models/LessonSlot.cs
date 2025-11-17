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
        public List<LessonSlot> GetAll()
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Lessons_Slots";
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            var list = new List<LessonSlot>();
            while (reader.Read())
            {
                list.Add(new LessonSlot
                {
                    Id = reader.GetInt32(reader.GetOrdinal("slot_id")),
                    StartLesson = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("startLesson"))),
                    EndLesson = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("endLesson")))
                });
            }
            return list;
        }

        public LessonSlot GetById(int id)
        {
            using var conn = DB.GetConnection();
            string sql = "SELECT * FROM Lessons_Slots WHERE slot_id=@id";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new LessonSlot
            {
                Id = reader.GetInt32(reader.GetOrdinal("slot_id")),
                StartLesson = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("startLesson"))),
                EndLesson = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("endLesson")))
            };
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
        
        public void GenerateDefaultSlots()
        {
            var slots = new List<(TimeOnly, TimeOnly)>
            {
                (new TimeOnly(8,30), new TimeOnly(9,50)),
                (new TimeOnly(10,10), new TimeOnly(11,30)),
                (new TimeOnly(11,50), new TimeOnly(13,10)),
                (new TimeOnly(13,30), new TimeOnly(14,50)),
                (new TimeOnly(15,5), new TimeOnly(16,25))
            };

            using var conn = DB.GetConnection();

            foreach (var (start, end) in slots)
            {
                string sql = @"INSERT INTO Lessons_Slots (startLesson, endLesson)
                               VALUES (@start, @end);";

                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@start", NpgsqlTypes.NpgsqlDbType.Time, start.ToTimeSpan());
                cmd.Parameters.AddWithValue("@end", NpgsqlTypes.NpgsqlDbType.Time, end.ToTimeSpan());

                cmd.ExecuteNonQuery();
            }
        }
    }
}
