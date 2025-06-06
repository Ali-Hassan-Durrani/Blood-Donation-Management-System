using Microsoft.Data.SqlClient;
using RequestBloodManager.Models;
using UserManager.Models;

public class AdminService
{
    private readonly string _connectionString;
    public AdminService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }
    // Get All Users
    public async Task<List<UserModel>> GetAllUsersAsync()
    {
        List<UserModel> users = new();

        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string query = "SELECT * FROM UserTable";
        SqlCommand cmd = new(query, conn);
        SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (reader.Read())
        {
            users.Add(new UserModel
            {
                UserID = (int)reader["UserID"],
                Name = reader["Name"].ToString(),
                Username = reader["Username"].ToString(),
                Role = reader["Role"].ToString(),
                LinkedID = (int)reader["LinkedID"]
            });
        }

        return users;
    }

    // Delete a user
    public async Task DeleteUserAsync(int userId)
    {
        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();
        string query = "DELETE FROM UserTable WHERE UserID = @id";
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@id", userId);
        await cmd.ExecuteNonQueryAsync();
    }

    // Get Dashboard Stats
    public async Task<Dictionary<string, int>> GetAdminDashboardStatsAsync()
    {
        var stats = new Dictionary<string, int>();

        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();

        string[] tables = new[]
        {
            "SeekersTable",
            "DonorsTable",
            "HospitalsTable",
            "BloodBanksTable",
            "RequestBloodTable"
        };

        foreach (var table in tables)
        {
            string query = $"SELECT COUNT(*) FROM {table}";
            using SqlCommand cmd = new(query, conn);
            int count = (int)await cmd.ExecuteScalarAsync();
            stats.Add(table, count);
        }

        return stats;
    }

    // Get Blood Requests (optionally filtered)
    public async Task<List<RequestBloodModel>> GetBloodRequestsAsync(string? status = null)
    {
        var list = new List<RequestBloodModel>();

        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM RequestBloodTable" + (status != null ? " WHERE Status = @Status" : "");
        using SqlCommand cmd = new(query, conn);

        if (status != null)
            cmd.Parameters.AddWithValue("@Status", status);

        await conn.OpenAsync();
        var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new RequestBloodModel
            {
                RequestID = (int)reader["RequestID"],
                RequesterRole = reader["RequesterRole"].ToString()!,
                RequesterID = (int)reader["RequesterID"],
                FullName = reader["FullName"].ToString()!,
                BloodGroup = reader["BloodGroup"].ToString()!,
                City = reader["City"].ToString()!,
                DonorName = reader["DonorName"].ToString()!,
                RequiredDate = (DateTime)reader["RequiredDate"],
                ContactNumber = reader["ContactNumber"].ToString()!,
                Status = reader["Status"].ToString()!
            });
        }

        return list;
    }

    // Update Request Status
    public async Task UpdateRequestStatusAsync(int requestId, string status)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "UPDATE RequestBloodTable SET Status = @Status WHERE RequestID = @RequestID";

        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Status", status);
        cmd.Parameters.AddWithValue("@RequestID", requestId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    // Update Admin Account
    public async Task<bool> UpdateAdminAccountAsync(UserModel model)
    {
        using SqlConnection conn = new(_connectionString);
        string query = @"
        UPDATE UserTable 
        SET Name = @Name, Username = @Username, Password = @Password 
        WHERE Role = 'Admin' AND UserID = @UserID";

        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@Username", model.Username);
        cmd.Parameters.AddWithValue("@Password", model.Password);
        cmd.Parameters.AddWithValue("@UserID", model.UserID);

        await conn.OpenAsync();
        int rowsAffected = await cmd.ExecuteNonQueryAsync();

        Console.WriteLine($"UpdateAdminAccountAsync -> Rows affected: {rowsAffected}");

        return rowsAffected > 0;
    }

    public async Task<UserModel> GetCurrentAdminAsync()
    {
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT UserID, Name, Username, Password FROM UserTable WHERE Role = 'Admin'";

        using SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new UserModel
            {
                UserID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Username = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }
        return new UserModel();
    }

}