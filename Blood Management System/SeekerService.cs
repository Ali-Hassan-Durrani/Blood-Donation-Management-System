using Microsoft.Data.SqlClient;
using SeekerManager.Models;
using System.Security.Claims;
public class SeekerService
{
    private readonly string _connectionString = string.Empty;

    public SeekerService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
    }

    public async Task<List<SeekerModel>> GetAllSeekersAsync()
    {
        var seekers = new List<SeekerModel>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM SeekersTable";
        using SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            seekers.Add(new SeekerModel
            {
                SeekerID = reader.GetInt32(reader.GetOrdinal("SeekerID")),
                Name = reader["Name"].ToString(),
                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                City = reader["City"].ToString(),
                BloodGroup = reader["BloodGroup"].ToString(),
                ContactNo = reader["ContactNo"].ToString(),
                CNIC = reader["CNIC"].ToString(),
                Gender = reader["Gender"].ToString(),
                RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate"))
            });
        }
        return seekers;
    }

    public async Task<SeekerModel> GetSeekerByIdAsync(int id)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM SeekersTable WHERE SeekerID = @SeekerID";
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@SeekerID", id);

        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new SeekerModel
            {
                SeekerID = id,
                Name = reader["Name"]?.ToString() ?? "",
                Age = Convert.ToInt32(reader["Age"]),
                City = reader["City"]?.ToString() ?? "",
                BloodGroup = reader["BloodGroup"]?.ToString() ?? "",
                ContactNo = reader["ContactNo"]?.ToString() ?? "",
                CNIC = reader["CNIC"]?.ToString() ?? "",
                Gender = reader["Gender"]?.ToString() ?? "",
                RegistrationDate = Convert.ToDateTime(reader["RegistrationDate"])
            };
        }

        return new SeekerModel();
    }

    public async Task<bool> RegisterSeekerAsync(SeekerModel seeker)
    {
        try
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new("INSERT INTO SeekersTable (Name, Age, City, BloodGroup, ContactNo, CNIC, Gender, RegistrationDate) VALUES (@Name, @Age, @City, @BloodGroup, @ContactNo, @CNIC, @Gender, @RegistrationDate)", conn); 

            cmd.Parameters.AddWithValue("@Name", seeker.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Age", seeker.Age);
            cmd.Parameters.AddWithValue("@City", seeker.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", seeker.BloodGroup ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactNo", seeker.ContactNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CNIC", seeker.CNIC ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Gender", seeker.Gender ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@RegistrationDate", seeker.RegistrationDate);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR DURING REGISTRATION: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteSeekerAsync(int seekerId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "DELETE FROM SeekersTable WHERE SeekerID = @SeekerID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@SeekerID", seekerId);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateSeekerAsync(SeekerModel seeker)
    {
        using SqlConnection conn = new(_connectionString);
        string query = @"UPDATE SeekersTable SET 
                            Name = @Name,
                            Age = @Age,
                            City = @City,
                            BloodGroup = @BloodGroup,
                            ContactNo = @ContactNo,
                            CNIC = @CNIC,
                            Gender = @Gender,
                            RegistrationDate = @RegistrationDate
                         WHERE SeekerID = @SeekerID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Name", seeker.Name);
        cmd.Parameters.AddWithValue("@Age", seeker.Age);
        cmd.Parameters.AddWithValue("@City", seeker.City);
        cmd.Parameters.AddWithValue("@BloodGroup", seeker.BloodGroup);
        cmd.Parameters.AddWithValue("@ContactNo", seeker.ContactNo);
        cmd.Parameters.AddWithValue("@CNIC", seeker.CNIC);
        cmd.Parameters.AddWithValue("@Gender", seeker.Gender);
        cmd.Parameters.AddWithValue("@RegistrationDate", seeker.RegistrationDate);
        cmd.Parameters.AddWithValue("@SeekerID", seeker.SeekerID);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<List<KeyValuePair<int, string>>> GetBloodGroupsAsync()
    {
        var list = new List<KeyValuePair<int, string>>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT BloodGroupID, BloodGroup FROM BloodsGroupsTable";
        SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new KeyValuePair<int, string>(
    reader.GetInt32(reader.GetOrdinal("BloodGroupID")),
    reader["BloodGroup"]?.ToString() ?? string.Empty));

        }
        return list;
    }

    public async Task<List<KeyValuePair<int, string>>> GetGendersAsync()
    {
        var list = new List<KeyValuePair<int, string>>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT GenderID, Gender FROM GenderTable";
        SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new KeyValuePair<int, string>(
    reader.GetInt32(reader.GetOrdinal("GenderID")),
    reader["Gender"]?.ToString() ?? string.Empty));
        }
        return list;
    }

    public async Task<List<string>> GetCitiesAsync()
    {
        var list = new List<string>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT City FROM CityTable";
        SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string city = reader["City"] as string ?? string.Empty;
            list.Add(city);
        }
        return list;
    }
}
