using Microsoft.Data.SqlClient;
using BloodBankManager.Models;
using HospitalManager.Models;
public class BloodBankService
    {
        private readonly string _connectionString;

        public BloodBankService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
        }

    public async Task<BloodBankModel> GetBloodBankByIdAsync(int bloodbankId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM BloodBanksTable WHERE BloodBankID = @BloodBankID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@BloodBankID", bloodbankId);

        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new BloodBankModel
            {
                BloodBankID = bloodbankId,
                Name = reader["Name"]?.ToString() ?? "",
                Address = reader["Address"]?.ToString() ?? "",
                PhoneNo = reader["PhoneNo"]?.ToString() ?? "",
                Location = reader["Location"]?.ToString() ?? "",
                Website = reader["Website"]?.ToString(),
                Email = reader["Email"]?.ToString() ?? "",
                City = reader["City"]?.ToString() ?? "",
            };
        }
        return new BloodBankModel();
    }

    public async Task<List<BloodBankModel>> GetAllBloodBanksAsync()
    {
        var bloodbanks = new List<BloodBankModel>();
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM BloodBanksTable";
        using SqlCommand cmd = new(query, conn);
        await conn.OpenAsync();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bloodbanks.Add(new BloodBankModel
            {
                BloodBankID = reader.GetInt32(reader.GetOrdinal("BloodBankID")),
                Name = reader["Name"].ToString(),
                Address = reader["Address"].ToString(),
                PhoneNo = reader["PhoneNo"].ToString(),
                Location = reader["Location"].ToString(),
                Website = reader["Website"].ToString(),
                Email = reader["Email"].ToString(),
                City = reader["City"].ToString()
            });
        }
        return bloodbanks;
    }

    public async Task<bool> DeleteBloodBankAsync(int bloodbankId)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "DELETE FROM BloodBanksTable WHERE BloodBankID = @BloodBankID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@BloodBankId", bloodbankId);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateBloodBankAsync(BloodBankModel bloodbank)
    {
        using SqlConnection conn = new(_connectionString);
        string query = @"UPDATE BloodBanksTable SET 
                            Name = @Name,
                            Address = @Address,
                            PhoneNo = @PhoneNo,
                            Location = @Location,
                            Website = @Website,
                            Email = @Email,
                            City = @City,
                         WHERE BloodBankID = @ID";
        using SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Name", bloodbank.Name);
        cmd.Parameters.AddWithValue("@Address", bloodbank.Address);
        cmd.Parameters.AddWithValue("@PhoneNo", bloodbank.PhoneNo);
        cmd.Parameters.AddWithValue("@Location", bloodbank.Location);
        cmd.Parameters.AddWithValue("@Website", bloodbank.Website);
        cmd.Parameters.AddWithValue("@Email", bloodbank.Email);
        cmd.Parameters.AddWithValue("@City", bloodbank.City);
        cmd.Parameters.AddWithValue("@BloodBankID", bloodbank.BloodBankID);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
    public async Task<bool> RegisterBloodBankAsync(BloodBankModel bank)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new(
                    "INSERT INTO BloodBanksTable (Name, Address, PhoneNo, Location, Website, Email, City) " +
                    "VALUES (@Name, @Address, @PhoneNo, @Location, @Website, @Email, @City)",
                    conn);

                cmd.Parameters.AddWithValue("@Name", bank.Name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", bank.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNo", bank.PhoneNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Location", bank.Location ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Website", (object?)bank.Website ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", bank.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@City", bank.City ?? (object)DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR DURING BLOOD BANK REGISTRATION: " + ex.Message);
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
    }