using Microsoft.Data.SqlClient;
using DonorManager.Models;

public class DonorService
{
    private readonly string _connectionString;

    public DonorService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
    }

    public async Task<DonorModel> GetDonorByIdAsync(int donorId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM DonorsTable WHERE DonorID = @DonorID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@DonorID", donorId);

        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new DonorModel
            {
                DonorID = donorId,
                Name = reader["Name"]?.ToString() ?? "",
                BloodGroup = reader["BloodGroup"]?.ToString() ?? "",
                FirstDonationDate = Convert.ToDateTime(reader["FirstDonationDate"]),
                LastDonationDate = Convert.ToDateTime(reader["LastDonationDate"]),
                ContactNo = reader["ContactNo"]?.ToString() ?? "",
                CNIC = reader["CNIC"]?.ToString() ?? "",
                Location = reader["Location"]?.ToString() ?? "",
                Gender = reader["Gender"]?.ToString() ?? "",
                City = reader["City"]?.ToString() ?? "",
                IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
            };
        }
        return new DonorModel();
    }

    public async Task<List<DonorModel>> GetAllDonorsAsync()
    {
        var donors = new List<DonorModel>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM DonorsTable WHERE IsAvailable = 1";
        using SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            donors.Add(new DonorModel
            {
                DonorID = Convert.ToInt32(reader["DonorID"]),
                Name = reader["Name"].ToString(),
                BloodGroup = reader["BloodGroup"].ToString(),
                FirstDonationDate = Convert.ToDateTime(reader["FirstDonationDate"]),
                LastDonationDate = Convert.ToDateTime(reader["LastDonationDate"]),
                ContactNo = reader["ContactNo"].ToString(),
                CNIC = reader["CNIC"].ToString(),
                Location = reader["Location"].ToString(),
                Gender = reader["Gender"].ToString(),
                City = reader["City"].ToString(),
                IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
            });
        }
        return donors;
}

    public async Task<bool> DeleteDonorAsync(int donorId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "DELETE FROM DonorsTable WHERE DonorID = @DonorID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@DonorID", donorId);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateDonorAsync(DonorModel donor)
    {
        using SqlConnection conn = new(_connectionString);
        string query = @"UPDATE DonorsTable SET 
                            Name = @Name,
                            BloodGroup = @BloodGroup,
                            FirstDonationDate = @FirstDonationDate,
                            LastDonationDate = @LastDonationDate,
                            ContactNo = @ContactNo,
                            CNIC = @CNIC,
                            Location = @Location,
                            Gender = @Gender,
                            City = @City,
                            IsAvailable = @IsAvailable
                         WHERE DonorID = @DonorID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Name", donor.Name);
        cmd.Parameters.AddWithValue("@BloodGroup", donor.BloodGroup);
        cmd.Parameters.AddWithValue("@FirstDonationDate", donor.FirstDonationDate);
        cmd.Parameters.AddWithValue("@LastDonationDate", donor.LastDonationDate);
        cmd.Parameters.AddWithValue("@ContactNo", donor.ContactNo);
        cmd.Parameters.AddWithValue("@CNIC", donor.CNIC);
        cmd.Parameters.AddWithValue("@Location", donor.Location);
        cmd.Parameters.AddWithValue("@Gender", donor.Gender);
        cmd.Parameters.AddWithValue("@City", donor.City);
        cmd.Parameters.AddWithValue("@IsAvailable", donor.IsAvailable);
        cmd.Parameters.AddWithValue("@DonorID", donor.DonorID);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> RegisterDonorAsync(DonorModel donor)
    {
        try
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new(
                "INSERT INTO DonorsTable " +
                "(Name, BloodGroup, FirstDonationDate, LastDonationDate, ContactNo, CNIC, Location, Gender, City, IsAvailable) " +
                "VALUES (@Name, @BloodGroup, @FirstDonationDate, @LastDonationDate, @ContactNo, @CNIC, @Location, @Gender, @City, @IsAvailable)",
                conn);

            cmd.Parameters.AddWithValue("@Name", donor.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@BloodGroup", donor.BloodGroup ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@FirstDonationDate", donor.FirstDonationDate);
            cmd.Parameters.AddWithValue("@LastDonationDate", donor.LastDonationDate);
            cmd.Parameters.AddWithValue("@ContactNo", donor.ContactNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CNIC", donor.CNIC ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", donor.Location ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Gender", donor.Gender ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@City", donor.City ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAvailable", donor.IsAvailable);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR DURING DONOR REGISTRATION: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateAvailabilityAsync(int donorId, bool isAvailable)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "UPDATE DonorsTable SET IsAvailable = @IsAvailable WHERE DonorID = @DonorID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);
        cmd.Parameters.AddWithValue("@DonorID", donorId);
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
                reader["BloodGroup"]?.ToString() ?? string.Empty
            ));
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
                reader["Gender"]?.ToString() ?? string.Empty
            ));
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
            list.Add(reader["City"]?.ToString() ?? string.Empty);
        }

        return list;
    }

    public async Task<List<DonorModel>> GetDonorsByBloodGroupAsync(string bloodGroup)
    {
        var list = new List<DonorModel>();
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SELECT * FROM DonorsTable WHERE BloodGroup = @BloodGroup", con);
        cmd.Parameters.AddWithValue("@BloodGroup", bloodGroup);
        await con.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new DonorModel
            {
                DonorID = (int)reader["DonorID"],
                Name = reader["Name"].ToString(),
                BloodGroup = reader["BloodGroup"].ToString(),
                City = reader["City"].ToString(),
                ContactNo = reader["ContactNo"].ToString(),
                LastDonationDate = Convert.ToDateTime(reader["LastDonationDate"]),
                IsAvailable = (bool)reader["IsAvailable"]
            });
        }
        return list;
    }

    public async Task<bool> UpdateLastDonationDateAsync(int donorId)
    {
        using var con = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("UPDATE DonorsTable SET LastDonationDate = @Date WHERE DonorID = @DonorID", con);
        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
        cmd.Parameters.AddWithValue("@DonorID", donorId);
        await con.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<int> GetTotalDonationsAsync(int donorId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT COUNT(*) FROM Donations WHERE DonorID = @DonorID", conn);
        cmd.Parameters.AddWithValue("@DonorID", donorId);

        return (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

    public async Task<int> GetApprovedRequestCountAsync(int donorId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT COUNT(*) FROM DonationRequests WHERE DonorID = @DonorID AND Status = 'Accepted'", conn);
        cmd.Parameters.AddWithValue("@DonorID", donorId);

        return (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

    public async Task<int> GetRejectedRequestCountAsync(int donorId)
    {
        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var cmd = new SqlCommand("SELECT COUNT(*) FROM DonationRequests WHERE DonorID = @DonorID AND Status = 'Rejected'", conn);
        cmd.Parameters.AddWithValue("@DonorID", donorId);

        return (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

}