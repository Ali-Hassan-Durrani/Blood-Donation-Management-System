using Microsoft.Data.SqlClient;
using HospitalManager.Models;
public class HospitalService
{
    private readonly string _connectionString;

    public HospitalService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
    }

    public async Task<HospitalModel> GetHospitalByIdAsync(int hospitalId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM HospitalsTable WHERE HospitalID = @HospitalID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@HospitalID", hospitalId);

        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new HospitalModel
            {
                HospitalID = hospitalId,
                Name = reader["Name"]?.ToString() ?? "",
                Address = reader["Address"]?.ToString() ?? "",
                PhoneNo = reader["PhoneNo"]?.ToString() ?? "",
                Location = reader["Location"]?.ToString() ?? "",
                Website = reader["Website"]?.ToString(),
                Email = reader["Email"]?.ToString() ?? "",
                City = reader["City"]?.ToString() ?? "",
            };
        }
        return new HospitalModel();
    }
    public async Task<List<HospitalModel>> GetAllHospitalsAsync()
    {
        var hospitals = new List<HospitalModel>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM HospitalsTable";
        using SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            hospitals.Add(new HospitalModel
            {
                HospitalID = reader.GetInt32(reader.GetOrdinal("HospitalID")),
                Name = reader["Name"].ToString(),
                Address = reader["Address"].ToString(),
                PhoneNo = reader["PhoneNo"].ToString(),
                Location = reader["Location"].ToString(),
                Website = reader["Website"].ToString(),
                Email = reader["Email"].ToString(),
                City = reader["City"].ToString()
            });
        }
        return hospitals;
    }

    public async Task<bool> DeleteHospitalAsync(int hospitalId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "DELETE FROM HospitalsTable WHERE HospitalID = @HospitalID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@HospitalID", hospitalId);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateHospitalAsync(HospitalModel hospital)
    {
        using SqlConnection conn = new(_connectionString);
        string query = @"UPDATE HospitalsTable SET 
                            Name = @Name,
                            Address = @Address,
                            PhoneNo = @PhoneNo,
                            Location = @Location,
                            Website = @Website,
                            Email = @Email,
                            City = @City,
                         WHERE HospitalID = @HospitalID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Name", hospital.Name);
        cmd.Parameters.AddWithValue("@Address", hospital.Address);
        cmd.Parameters.AddWithValue("@PhoneNo", hospital.PhoneNo);
        cmd.Parameters.AddWithValue("@Location", hospital.Location);
        cmd.Parameters.AddWithValue("@Website", hospital.Website);
        cmd.Parameters.AddWithValue("@Email", hospital.Email);
        cmd.Parameters.AddWithValue("@City", hospital.City);
        cmd.Parameters.AddWithValue("@HospitalID", hospital.HospitalID);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> RegisterHospitalAsync(HospitalModel hospital)
    {
        try
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new(
                "INSERT INTO HospitalsTable (Name, Address, PhoneNo, Location, Website, Email, City) " +
                "VALUES (@Name, @Address, @PhoneNo, @Location, @Website, @Email, @City)",
                conn);

            cmd.Parameters.AddWithValue("@Name", hospital.Name ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", hospital.Address ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNo", hospital.PhoneNo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Location", hospital.Location ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Website", (object?)hospital.Website ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", hospital.Email ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@City", hospital.City ?? (object)DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR DURING HOSPITAL REGISTRATION: " + ex.Message);
            return false;
        }
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

    public async Task<List<HospitalModel>> GetHospitalsByCityAsync(string city)
    {
        var hospitals = new List<HospitalModel>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            string query = "SELECT HospitalID, Name, Address, PhoneNo, Location, Website, Email, City FROM HospitalsTable WHERE City = @City";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@City", city);

                await conn.OpenAsync();
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        hospitals.Add(new HospitalModel
                        {
                            HospitalID = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Address = reader.GetString(2),
                            PhoneNo = reader.GetString(3) ,
                            Location = reader.GetString(4),
                            Website = reader.GetString(5),
                            Email = reader.GetString(6),
                            City = reader.GetString(7)
                        });
                    }
                }
            }
        }

        return hospitals;
    }
}