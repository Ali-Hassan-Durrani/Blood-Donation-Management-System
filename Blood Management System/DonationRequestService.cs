using BloodDonationApp.Components.Pages;
using Microsoft.Data.SqlClient;
public class DonationRequestService
{
    private readonly IConfiguration _configuration;

    public DonationRequestService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task<bool> SendRequestAsync(int donorId, int bloodBankId, string message)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("INSERT INTO DonationRequests (DonorID, BloodBankID, Message, Status, RequestDate) VALUES (@DonorID, @BloodBankID, @Message, 'Pending', @RequestDate)", con);
        cmd.Parameters.AddWithValue("@DonorID", donorId);
        cmd.Parameters.AddWithValue("@BloodBankID", bloodBankId);
        cmd.Parameters.AddWithValue("@Message", message);
        cmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);

        con.Open();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<List<DonationRequestModel>> GetRequestsForDonorAsync(int donorId)
    {
        var list = new List<DonationRequestModel>();
        using var con = GetConnection();
        using var cmd = new SqlCommand("SELECT * FROM DonationRequests WHERE DonorID = @DonorID", con);
        cmd.Parameters.AddWithValue("@DonorID", donorId);
        con.Open();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new DonationRequestModel
            {
                RequestID = (int)reader["RequestID"],
                DonorID = (int)reader["DonorID"],
                BloodBankID = (int)reader["BloodBankID"],
                Message = reader["Message"].ToString(),
                Status = reader["Status"].ToString(),
                RequestDate = (DateTime)reader["RequestDate"]
            });
        }
        return list;
    }

    public async Task<bool> RespondToRequestAsync(int requestId, int donorId, bool accept)
    {
        using var con = GetConnection();
        using var cmd = new SqlCommand("UPDATE DonationRequests SET Status = @Status WHERE RequestID = @RequestID AND DonorID = @DonorID", con);
        cmd.Parameters.AddWithValue("@Status", accept ? "Accepted" : "Rejected");
        cmd.Parameters.AddWithValue("@RequestID", requestId);
        cmd.Parameters.AddWithValue("@DonorID", donorId);
        con.Open();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateRequestStatusAsync(int requestId, string newStatus)
    {
        using var con = GetConnection();
        using var command = new SqlCommand(
            "UPDATE DonationRequests SET Status = @Status WHERE RequestID = @RequestID", con);

        command.Parameters.AddWithValue("@Status", newStatus);
        command.Parameters.AddWithValue("@RequestID", requestId);

        await con.OpenAsync();
        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<(int DonorId, int BloodBankId)?> GetRequestInfoAsync(int requestId)
    {
        using var con = GetConnection();
        using var command = new SqlCommand(
            "SELECT DonorID, BloodBankID FROM DonationRequests WHERE RequestID = @RequestID", con);

        command.Parameters.AddWithValue("@RequestID", requestId);

        await con.OpenAsync();
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int donorId = reader.GetInt32(0);
            int bloodBankId = reader.GetInt32(1);
            return (donorId, bloodBankId);
        }
        return null;
    }
}