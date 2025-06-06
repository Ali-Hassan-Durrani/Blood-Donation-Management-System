using System.Data;
using Microsoft.Data.SqlClient;
using BloodBankManager.Models;
using BloodInventoryManager.Models;

public class InventoryService
{
    private readonly string _connectionString;

    public InventoryService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<BloodInventoryModel>> GetInventoryAsync()
    {
        List<BloodInventoryModel> list = new();

        using SqlConnection con = new(_connectionString);
        using SqlCommand cmd = new("SELECT * FROM BloodInventory", con);
        con.Open();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new BloodInventoryModel
            {
                InventoryID = Convert.ToInt32(reader["InventoryID"]),
                BloodGroup = reader["BloodGroup"].ToString(),
                UnitsAvailable = Convert.ToInt32(reader["UnitsAvailable"])
            });
        }

        return list;
    }

    public async Task<bool> AddInventoryAsync(BloodInventoryModel model)
    {
        using SqlConnection con = new(_connectionString);
        using SqlCommand cmd = new("INSERT INTO BloodInventory (BloodGroup, UnitsAvailable) VALUES (@bg, @units)", con);

        cmd.Parameters.AddWithValue("@bg", model.BloodGroup);
        cmd.Parameters.AddWithValue("@units", model.UnitsAvailable);

        con.Open();
        int rows = await cmd.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<Dictionary<string, int>> GetBloodGroupCountsAsync()
    {
        Dictionary<string, int> counts = new();

        using SqlConnection con = new(_connectionString);
        using SqlCommand cmd = new("SELECT BloodGroup, SUM(UnitsAvailable) AS TotalUnits FROM BloodInventory GROUP BY BloodGroup", con);
        con.Open();
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            counts[reader["BloodGroup"].ToString()] = Convert.ToInt32(reader["TotalUnits"]);
        }

        return counts;
    }
    public async Task<bool> DecrementBloodGroupAsync(string bloodGroup)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "UPDATE BloodInventory SET UnitsAvailable = UnitsAvailable - 1 WHERE BloodGroup = @BloodGroup AND UnitsAvailable > 0";
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@BloodGroup", bloodGroup);
        await conn.OpenAsync();
        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateInventoryAsync(BloodInventoryModel model)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "UPDATE BloodInventory SET UnitsAvailable = @Units WHERE InventoryID = @ID";
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Units", model.UnitsAvailable);
        cmd.Parameters.AddWithValue("@ID", model.InventoryID);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteInventoryAsync(int id)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "DELETE FROM BloodInventory WHERE InventoryID = @ID";
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@ID", id);
        await conn.OpenAsync();
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

}