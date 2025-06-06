using RequestBloodManager.Models;
using Microsoft.Data.SqlClient;
public class RequestBloodService
{
    private readonly SqlConnection _connection;

    public RequestBloodService(IConfiguration configuration)
    {
        _connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task AddRequestAsync(RequestBloodModel request)
    {
        string query = "INSERT INTO RequestBloodTable (RequesterRole, RequesterID, FullName, BloodGroup, City, DonorName, RequiredDate, ContactNumber, Status) " +
                       "VALUES (@RequesterRole, @RequesterID, @FullName, @BloodGroup, @City, @DonorName, @RequiredDate, @ContactNumber, @Status)";

        using var command = new SqlCommand(query, _connection);
        command.Parameters.AddWithValue("@RequesterRole", request.RequesterRole);
        command.Parameters.AddWithValue("@RequesterID", request.RequesterID);
        command.Parameters.AddWithValue("@FullName", request.FullName);
        command.Parameters.AddWithValue("@BloodGroup", request.BloodGroup);
        command.Parameters.AddWithValue("@City", request.City);
        command.Parameters.AddWithValue("@DonorName", request.DonorName);
        command.Parameters.AddWithValue("@RequiredDate", request.RequiredDate);
        command.Parameters.AddWithValue("@ContactNumber", request.ContactNumber);
        command.Parameters.AddWithValue("@Status", request.Status);

        await _connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
        _connection.Close();
    }

    public async Task<List<RequestBloodModel>> GetRequestsByRoleAndIdAsync(string role, int requesterId)
    {
        var requests = new List<RequestBloodModel>();
        try
        {
            string query = "SELECT * FROM RequestBloodTable WHERE RequesterRole = @Role AND RequesterID = @ID";
            using SqlCommand cmd = new SqlCommand(query, _connection);
            cmd.Parameters.AddWithValue("@Role", role);
            cmd.Parameters.AddWithValue("@ID", requesterId);

            await _connection.OpenAsync();
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                requests.Add(new RequestBloodModel
                {
                    RequestID = Convert.ToInt32(reader["RequestID"]),
                    RequesterRole = reader["RequesterRole"].ToString(),
                    RequesterID = Convert.ToInt32(reader["RequesterID"]),
                    FullName = reader["FullName"].ToString(),
                    BloodGroup = reader["BloodGroup"].ToString(),
                    City = reader["City"].ToString(),
                    DonorName = reader["DonorName"].ToString(),
                    RequiredDate = Convert.ToDateTime(reader["RequiredDate"]),
                    ContactNumber = reader["ContactNumber"].ToString(),
                    Status = reader["Status"].ToString()
                });
            }

            await reader.CloseAsync();
            _connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching requests: " + ex.Message);
        }

        return requests;
    }
}