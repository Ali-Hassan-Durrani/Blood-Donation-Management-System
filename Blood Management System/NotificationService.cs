using BloodDonationApp.Components.Pages;
using Microsoft.Data.SqlClient;
using DonorManager.Models;
public class NotificationService
{
    private readonly IConfiguration _configuration;

    public NotificationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task<bool> NotifyBloodBankAsync(int requestId, string donorname, bool accept)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("INSERT INTO Notifications (BloodBankID, Message, CreatedAt) VALUES (@BloodBankID, @Message, @CreatedAt)", con);

        // Get blood bank id from the request
        var bloodBankId = await GetBloodBankIdFromRequestAsync(requestId);
        var message = $"Donor #{donorname} has {(accept ? "accepted" : "rejected")} the request (ID #{requestId}).";

        cmd.Parameters.AddWithValue("@BloodBankID", bloodBankId);
        cmd.Parameters.AddWithValue("@Message", message);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        con.Open();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    private async Task<int> GetBloodBankIdFromRequestAsync(int requestId)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("SELECT BloodBankID FROM DonationRequests WHERE RequestID = @RequestID", con);
        cmd.Parameters.AddWithValue("@RequestID", requestId);
        con.Open();
        var result = await cmd.ExecuteScalarAsync();
        return result != null ? Convert.ToInt32(result) : 0;
    }

    public async Task<List<NotificationModel>> GetNotificationsForBloodBankAsync(int bloodBankId)
    {
        var list = new List<NotificationModel>();
        using var con = GetConnection();
        using var cmd = new SqlCommand("SELECT * FROM Notifications WHERE BloodBankID = @BloodBankID ORDER BY CreatedAt DESC", con);
        cmd.Parameters.AddWithValue("@BloodBankID", bloodBankId);
        con.Open();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new NotificationModel
            {
                NotificationID = (int)reader["NotificationID"],
                BloodBankID = (int)reader["BloodBankID"],
                Message = reader["Message"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"]
            });
        }
        return list;
    }
    public async Task<bool> SendNotificationAsync(int bloodBankId, int donorId, string message)
    {
        using var con = GetConnection();
        using var command = new SqlCommand(
            @"INSERT INTO Notifications (BloodBankID, DonorID, Message, Status, CreatedAt)
          VALUES (@BloodBankID, @DonorID, @Message, 'Unread', @CreatedAt)", con);

        command.Parameters.AddWithValue("@BloodBankID", bloodBankId);
        command.Parameters.AddWithValue("@DonorID", donorId);
        command.Parameters.AddWithValue("@Message", message);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        await con.OpenAsync();
        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }
}
