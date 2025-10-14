using System.Security.Cryptography;
using ConsoleApp.database;
using Npgsql;

namespace ConsoleApp.models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserPhoto { get; set; }
    public bool isNotified { get; set; }
    public int daysForNotifications { get; set; }
    public bool isConnectedTelegram { get; set; }

    public bool Register(string name, string surname, string email, string password)
    {
        using var conn = DB.GetConnection();
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        string query = @"INSERT INTO Users(""name"", surname, email, password, userPhoto, isNotified, daysForNotification, isConnectedTelegram) VALUES (@name, @surname, @email, @hashedPassword, null, false, 0, false) RETURNING surname";

        using var cmd = new NpgsqlCommand(query, conn);
        
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@surname", surname);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@hashedPassword", hashedPassword);

        Name = name;
        Surname = surname;
        Email = email;
        Password = password;
        
        using var reader = cmd.ExecuteReader();

        bool isRegistered;

        if (reader.Read())
        {
            isRegistered = true;
        }
        else
        {
            isRegistered = false;
            throw new Exception("Не вдалось зареєструвати користувача");
        }

        return isRegistered;
    }

    public bool Login(string email, string password)
    {
        using var conn = DB.GetConnection();

        string userQuery = "SELECT password FROM Users WHERE email=@email";
        
        using var userCmd = new NpgsqlCommand(userQuery, conn);
        userCmd.Parameters.AddWithValue("@email", email);

        object result = userCmd.ExecuteScalar();
        if (result == null)
        {
            throw new Exception("Користувача з таким email не знайдено");
        }

        string userPass = Convert.ToString(result);

        var isCorrect = BCrypt.Net.BCrypt.Verify(password, userPass);

        if (isCorrect)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ForgotPassword(string email, int code)
    {
        using var conn = DB.GetConnection();

        string userQuery = "SELECT password FROM Users WHERE email=@email";
        
        using var userCmd = new NpgsqlCommand(userQuery, conn);
        userCmd.Parameters.AddWithValue("@email", email);

        object result = userCmd.ExecuteScalar();
        if (result == null)
        {
            throw new Exception("Користувача з таким email не знайдено");
        }
        else
        {
            if (code == 1234) Console.WriteLine("Correct code"); // test case
            else Console.WriteLine("Inncorrect code");
        }
    }

    public bool ResetPassword(string newPassword)
    {
        using var conn = DB.GetConnection();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
        string query = "UPDATE Users SET password=@hashedPassword WHERE email=@email RETURNING email";

        using var cmd = new NpgsqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@email", Email);
        cmd.Parameters.AddWithValue("hashedPassword", hashedPassword);

        using var reader = cmd.ExecuteReader();

        if (reader.Read()) return true;
        else return false;
    }

    public void UpdateProfile(string name, string surname, string photo, string email, string password, bool isNotified, int daysForNotification, bool isConnectedTG)
    {
        using var conn = DB.GetConnection();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        string query = @"UPDATE Users SET ""name""=@name, surname=@surname, photo=@photo, email=@email, isNotified=@isNotified, daysForNotification=@daysForNotification,isConnectedTG=@isConnectedTG, password=@password WHERE email=@email";

        using var cmd = new NpgsqlCommand(query, conn);

        Name = name;
        Surname = name;
        Email = name;
        UserPhoto = name;
        Password = name;
        this.isNotified = isNotified;
        isConnectedTG = isConnectedTelegram;
        daysForNotification = daysForNotifications;

        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@surname", surname);
        cmd.Parameters.AddWithValue("@photo", photo);
        cmd.Parameters.AddWithValue("@isNotified", isNotified);
        cmd.Parameters.AddWithValue("@daysForNotification", daysForNotification);
        cmd.Parameters.AddWithValue("@isConnectedTG", isConnectedTG);
        cmd.Parameters.AddWithValue("@password", hashedPassword);
    }

    public void SetNotificationPreferences(bool isNotified, int days)
    {
        using var conn = DB.GetConnection();
        
        string query = "UPDATE Users SET isNotified=@isNotified, daysForNotification=@days";

        using var cmd = new NpgsqlCommand(query, conn);

        cmd.Parameters.AddWithValue("@isNotified", isNotified);
        cmd.Parameters.AddWithValue("@days", days);
    }
}