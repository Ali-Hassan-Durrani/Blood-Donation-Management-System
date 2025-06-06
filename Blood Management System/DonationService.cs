using BloodDonationApp.Components.Pages;
using Microsoft.Data.SqlClient;
public class DonationService
{
    private readonly IConfiguration _configuration;

    public DonationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private SqlConnection GetConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

    public async Task<List<DonationModel>> GetDonationsByDonorIdAsync(int donorId)
    {
        var donations = new List<DonationModel>();

        using var con = GetConnection();
        {
            await con.OpenAsync();
            var query = @"
            SELECT d.DonationID, d.DonorID, d.BloodBankID, b.Name AS BloodBankName, d.DonationDate
            FROM Donations d
            INNER JOIN BloodBanksTable b ON d.BloodBankID = b.BloodBankID
            WHERE d.DonorID = @DonorID
            ORDER BY d.DonationDate DESC";

            using (var command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@DonorID", donorId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        donations.Add(new DonationModel
                        {
                            DonationID = (int)reader["DonationID"],
                            DonorID = (int)reader["DonorID"],
                            BloodBankID = (int)reader["BloodBankID"],
                            BloodBankName = reader["BloodBankName"].ToString()!,
                            DonationDate = (DateTime)reader["DonationDate"]
                        });
                    }
                }
            }
        }

        return donations;
    }

    public async Task<bool> AddDonationAsync(int donorId, int bloodBankId, DateTime donationDate)
    {
        using var con = GetConnection();
        using var command = new SqlCommand(
            @"INSERT INTO Donations (DonorID, BloodBankID, DonationDate) 
          VALUES (@DonorID, @BloodBankID, @DonationDate)", con);

        command.Parameters.AddWithValue("@DonorID", donorId);
        command.Parameters.AddWithValue("@BloodBankID", bloodBankId);
        command.Parameters.AddWithValue("@DonationDate", donationDate);

        await con.OpenAsync();
        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }


}
