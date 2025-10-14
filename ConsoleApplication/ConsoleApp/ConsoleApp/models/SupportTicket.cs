using ConsoleApp.database;
using Npgsql;

namespace ConsoleApp.models;

public class SupportTicket
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int CategoryId { get; set; }

    public void SendContactForm(string name, string email, string message, string type, int categoryId)
    {
        var conn = DB.GetConnection();

        string userQuery = "SELECT user_id FROM Users WHERE email=@email;";
        using var userCmd = new NpgsqlCommand(userQuery, conn);
        userCmd.Parameters.AddWithValue("@email", email);

        object result = userCmd.ExecuteScalar();
        if (result == null)
        {
            throw new Exception("Користувача з таким email не знайдено");
        }

        int userId = Convert.ToInt32(result);
        
        string query = "INSERT INTO Support_Tickets(category_id, user_id, description, type) VALUES (@categoryId, @userID, @message, @type)";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@categoryId", categoryId);
        cmd.Parameters.AddWithValue("@userID", userId);
        cmd.Parameters.AddWithValue("@message", message);
        cmd.Parameters.AddWithValue("@type", type);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            throw new Exception("Форму зворотнього звʼязку не надіслано");
        }

    }
}