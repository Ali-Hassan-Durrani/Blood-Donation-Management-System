using Microsoft.Data.SqlClient;
using UserManager.Models;

public class UserService
{
    private readonly string _connectionString;

    public UserService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");
    }

    public UserModel? CurrentUser { get; set; }
    public async Task<UserModel?> LoginAsync(string username, string password)
    {
        using SqlConnection conn = new(_connectionString);
        string query = "SELECT * FROM UserTable WHERE Username=@Username AND Password=@Password";
        SqlCommand cmd = new(query, conn);
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@Password", password);

        await conn.OpenAsync();
        SqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (reader.Read())
        {
            return new UserModel
            {
                UserID = (int)reader["UserID"],
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                Role = reader["Role"].ToString(),
                LinkedID = (int)reader["LinkedID"]
            };
        }

        return null;
    }

    public async Task<bool> RegisterAsync(UserModel user)
    {
        Console.WriteLine($"[Register] Name: {user.Name}, Username: {user.Username}, Role: {user.Role}");

        if (string.IsNullOrWhiteSpace(user.Name) ||
            string.IsNullOrWhiteSpace(user.Username) ||
            string.IsNullOrWhiteSpace(user.Password) ||
            string.IsNullOrWhiteSpace(user.Role))
        {
            Console.WriteLine("One or more required fields are missing.");
            return false;
        }

        using SqlConnection conn = new(_connectionString);
        await conn.OpenAsync();
        SqlTransaction transaction = conn.BeginTransaction();

        try
        {
            int linkedId = 0;

            if (user.Role == "Admin")
            {
                linkedId = 0;
            }
            else
            {
                string roleTable = user.Role switch
                {
                    "Seeker" => "SeekersTable",
                    "Donor" => "DonorsTable",
                    "Hospital" => "HospitalsTable",
                    "BloodBank" => "BloodBanksTable",
                    _ => throw new Exception("Invalid role selected.")
                };

                string query = $"SELECT {user.Role}ID FROM {roleTable} WHERE Name = @Name";
                SqlCommand getIdCmd = new(query, conn, transaction);
                getIdCmd.Parameters.AddWithValue("@Name", user.Name);

                object? result = await getIdCmd.ExecuteScalarAsync();
                if (result == null)
                {
                    throw new Exception($"No {user.Role} found with Name: {user.Name}");
                }

                linkedId = Convert.ToInt32(result);
            }

            SqlCommand userCmd = new SqlCommand(
                "INSERT INTO UserTable (Name, Username, Password, Role, LinkedID) VALUES (@Name, @Username, @Password, @Role, @LinkedID)",
                conn, transaction);

            userCmd.Parameters.AddWithValue("@Name", user.Name);
            userCmd.Parameters.AddWithValue("@Username", user.Username);
            userCmd.Parameters.AddWithValue("@Password", user.Password);
            userCmd.Parameters.AddWithValue("@Role", user.Role);
            userCmd.Parameters.AddWithValue("@LinkedID", linkedId);

            await userCmd.ExecuteNonQueryAsync();
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Register Error] {ex.Message}");
            transaction.Rollback();
            return false;
        }
    }

}
