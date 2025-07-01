using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Data;

class DatabaseHelper
{
    // ✅ Connection String mein Busy Timeout aur Journal Mode add karo
    private static string connectionString = "Data Source=hostlity.db;Version=3;Pooling=True;BusyTimeout=3000;Journal Mode=Wal;";

    public static void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string createUserTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    FullName TEXT NOT NULL,
                    Email TEXT UNIQUE NOT NULL,
                    Password TEXT NOT NULL,
                    Phone TEXT DEFAULT 'N/A',
                    City TEXT DEFAULT 'N/A',
                    Age TEXT DEFAULT 'N/A',
                    Gender TEXT DEFAULT 'N/A',
                    SecurityQuestion TEXT DEFAULT 'N/A'
                );";
            using (SQLiteCommand command = new SQLiteCommand(createUserTableQuery, connection))
            {
                command.ExecuteNonQuery();

            }
            
            string createHostelTableQuery = @"
                CREATE TABLE IF NOT EXISTS Hostels (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID INTEGER,
                    ReferenceID TEXT UNIQUE NOT NULL,
                    Name TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    Rent INTEGER NOT NULL,
                    Ratings INTEGER DEFAULT 0,
                    Seats INTEGER DEFAULT 1,
                    Wifi INTEGER DEFAULT 0,
                    Laundry INTEGER DEFAULT 0,
                    Cleaning INTEGER DEFAULT 0,
                    AttachedWashroom INTEGER DEFAULT 0,
                    SecurityCameras INTEGER DEFAULT 0,
                    Geyser INTEGER DEFAULT 0,
                    Mess INTEGER DEFAULT 0,
                    WhatsApp TEXT DEFAULT 'N/A',
                    FOREIGN KEY(UserID) REFERENCES Users(ID)
                );";

            
            using (SQLiteCommand command = new SQLiteCommand(createHostelTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public static bool InsertHostel(
    int userId, string referenceID, string name, string location, int rent, int rating, int seats,
    bool wifi, bool laundry, bool cleaning, bool attachedWashroom,
    bool security, bool geyser, bool mess, string WhatsApp)
{
    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        string query = @"
            INSERT INTO Hostels 
            (UserID, ReferenceID, Name, Location, Rent, Ratings, Seats, Wifi, Laundry, Cleaning, 
            AttachedWashroom, SecurityCameras, Geyser, Mess, WhatsApp)
            VALUES 
            (@UserID, @ReferenceID, @Name, @Location, @Rent, @Ratings, @Seats, @Wifi, @Laundry, 
            @Cleaning, @AttachedWashroom, @SecurityCameras, @Geyser, @Mess, @WhatsApp);";


        using (SQLiteCommand command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ReferenceID", referenceID);
            command.Parameters.AddWithValue("@UserID", userId);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Location", location);
            command.Parameters.AddWithValue("@Rent", rent);
            command.Parameters.AddWithValue("@Ratings", rating);
            command.Parameters.AddWithValue("@Seats", seats);
            command.Parameters.AddWithValue("@Wifi", wifi ? 1 : 0);
            command.Parameters.AddWithValue("@Laundry", laundry ? 1 : 0);
            command.Parameters.AddWithValue("@Cleaning", cleaning ? 1 : 0);
            command.Parameters.AddWithValue("@AttachedWashroom", attachedWashroom ? 1 : 0);
            command.Parameters.AddWithValue("@SecurityCameras", security ? 1 : 0);
            command.Parameters.AddWithValue("@Geyser", geyser ? 1 : 0);
            command.Parameters.AddWithValue("@Mess", mess ? 1 : 0);
            command.Parameters.AddWithValue("@WhatsApp", WhatsApp);

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Hostel Insertion Failed: " + ex.Message);
                return false;
            }
        }
    }
}

public static List<Dictionary<string, object>> GetHostelsByUserId(int userId)
{
    var hostels = new List<Dictionary<string, object>>();

    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        string query = @"
            SELECT 
                ID, UserID, ReferenceID, Name, Location, Rent, Ratings, Seats,
                Wifi, Laundry, Cleaning, AttachedWashroom, SecurityCameras, Geyser, Mess 
            FROM Hostels 
            WHERE UserID = @UserID";

        using (var command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@UserID", userId);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var hostel = new Dictionary<string, object>
                    {
                        ["ID"] = reader["ID"],
                        ["UserID"] = reader["UserID"],
                        ["ReferenceID"] = reader["ReferenceID"],
                        ["Name"] = reader["Name"],
                        ["Location"] = reader["Location"],
                        ["Rent"] = reader["Rent"],
                        ["Ratings"] = reader["Ratings"],
                        ["Seats"] = reader["Seats"],
                        ["Wifi"] = reader["Wifi"],
                        ["Laundry"] = reader["Laundry"],
                        ["Cleaning"] = reader["Cleaning"],
                        ["AttachedWashroom"] = reader["AttachedWashroom"],
                        ["SecurityCameras"] = reader["SecurityCameras"],
                        ["Geyser"] = reader["Geyser"],
                        ["Mess"] = reader["Mess"]
                    };

                    hostels.Add(hostel);
                }
            }
        }
    }

    return hostels;
}

public static List<Dictionary<string, object>> GetAllHostels()
{
    var hostels = new List<Dictionary<string, object>>();

    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        string query = @"
            SELECT 
                ID, UserID, ReferenceID, Name, Location, Rent, Ratings, Seats,
                Wifi, Laundry, Cleaning, AttachedWashroom, SecurityCameras, Geyser, Mess
            FROM Hostels";

        using (var command = new SQLiteCommand(query, connection))
        {
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var hostel = new Dictionary<string, object>
                    {
                        ["ID"] = reader["ID"],
                        ["UserID"] = reader["UserID"],
                        ["ReferenceID"] = reader["ReferenceID"],
                        ["Name"] = reader["Name"],
                        ["Location"] = reader["Location"],
                        ["Rent"] = reader["Rent"],
                        ["Ratings"] = reader["Ratings"],
                        ["Seats"] = reader["Seats"],
                        ["Wifi"] = reader["Wifi"],
                        ["Laundry"] = reader["Laundry"],
                        ["Cleaning"] = reader["Cleaning"],
                        ["AttachedWashroom"] = reader["AttachedWashroom"],
                        ["SecurityCameras"] = reader["SecurityCameras"],
                        ["Geyser"] = reader["Geyser"],
                        ["Mess"] = reader["Mess"]
                    };

                    hostels.Add(hostel);
                }
            }
        }
    }

    return hostels;
}

public static Dictionary<string, object> GetHostelDetailsByReferenceId(string referenceId)
{
    var hostelDetails = new Dictionary<string, object>();

    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        string query = @"
            SELECT 
                ID, UserID, ReferenceID, Name, Location, Rent, Ratings, Seats,
                Wifi, Laundry, Cleaning, AttachedWashroom, SecurityCameras, Geyser, Mess, WhatsApp 
            FROM Hostels 
            WHERE ReferenceID = @ReferenceID";

        using (var command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ReferenceID", referenceId);
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    hostelDetails["ID"] = reader["ID"];
                    hostelDetails["UserID"] = reader["UserID"];
                    hostelDetails["ReferenceID"] = reader["ReferenceID"];
                    hostelDetails["Name"] = reader["Name"];
                    hostelDetails["Location"] = reader["Location"];
                    hostelDetails["Rent"] = reader["Rent"];
                    hostelDetails["Ratings"] = reader["Ratings"];
                    hostelDetails["Seats"] = reader["Seats"];
                    hostelDetails["Wifi"] = reader["Wifi"];
                    hostelDetails["Laundry"] = reader["Laundry"];
                    hostelDetails["Cleaning"] = reader["Cleaning"];
                    hostelDetails["AttachedWashroom"] = reader["AttachedWashroom"];
                    hostelDetails["SecurityCameras"] = reader["SecurityCameras"];
                    hostelDetails["Geyser"] = reader["Geyser"];
                    hostelDetails["Mess"] = reader["Mess"];
                    hostelDetails["WhatsApp"] = reader["WhatsApp"];
                }
            }
        }
    }

    return hostelDetails;
}

public static bool UpdateHostelInfo(string referenceId, string name, string rent, string location, string seats, string ratings, string whatsapp)
{
    try
    {
        using (var conn = new SQLiteConnection(connectionString)) // ✅ use shared connection string
        {
            conn.Open();
            string query = @"UPDATE Hostels 
                             SET Name=@Name, Rent=@Rent, Location=@Location, Seats=@Seats, 
                                 Ratings=@Ratings, WhatsApp=@WhatsApp 
                             WHERE ReferenceID=@ReferenceID";

            using (var cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Rent", rent);
                cmd.Parameters.AddWithValue("@Location", location);
                cmd.Parameters.AddWithValue("@Seats", seats);
                cmd.Parameters.AddWithValue("@Ratings", ratings);
                cmd.Parameters.AddWithValue("@WhatsApp", whatsapp); // ✅ added
                cmd.Parameters.AddWithValue("@ReferenceID", referenceId);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error updating hostel info: " + ex.Message);
        return false;
    }
}



public static bool RemoveHostel(string referenceId)
{
    using (var conn = new SQLiteConnection(connectionString))
    {
        conn.Open();
        string query = "DELETE FROM Hostels WHERE ReferenceID = @refId";

        using (var cmd = new SQLiteCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@refId", referenceId);
            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
    }
}

    public static int GetUserIdByEmail(string email)
{
    using (var connection = new SQLiteConnection(connectionString))
    {
        connection.Open();
        string query = "SELECT ID FROM Users WHERE Email=@Email";
        using (var command = new SQLiteCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Email", email);
            object result = command.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}

    public static bool ResetPassword(string email, string securityAnswer, string newPassword)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE Users SET Password=@NewPassword WHERE Email=@Email AND SecurityQuestion=@SecurityAnswer";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NewPassword", newPassword);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@SecurityAnswer", securityAnswer);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }

    public static bool UpdateSecurityQuestion(string email, string password, string newSecurityQuestion)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "UPDATE Users SET SecurityQuestion=@NewSecurityQuestion WHERE Email=@Email AND Password=@Password";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NewSecurityQuestion", newSecurityQuestion);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                return command.ExecuteNonQuery() > 0;
            }
        }
    }

    public static bool RegisterUser(string fullName, string email, string password, string securityQuestion)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Users (FullName, Email, Password, SecurityQuestion) VALUES (@FullName, @Email, @Password, @SecurityQuestion)";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", fullName);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@SecurityQuestion", securityQuestion);
                
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SQLiteException)
                {
                    return false;
                }
            }
        }
    }

    public static (string FullName, string Email, string SecurityQuestion, string Phone, string City, string Age, string Gender) GetUserDetails(string email)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT FullName, Email, SecurityQuestion, Phone, City, Age, Gender FROM Users WHERE Email=@Email";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.IsDBNull(3) ? "N/A" : reader.GetString(3),
                            reader.IsDBNull(4) ? "N/A" : reader.GetString(4),
                            reader.IsDBNull(5) ? "N/A" : reader.GetString(5),
                            reader.IsDBNull(6) ? "N/A" : reader.GetString(6)
                        );
                    }
                    return ("N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A");
                }
            }
        }
    }

    public static bool UpdateUserProfile(string email, string fullName, string phone, string city, string age, string gender)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = @"
                UPDATE Users 
                SET FullName=@FullName, Phone=@Phone, City=@City, Age=@Age, Gender=@Gender
                WHERE Email=@Email";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", fullName);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@Age", age);
                command.Parameters.AddWithValue("@Gender", gender);
                command.Parameters.AddWithValue("@Email", email);
                
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }

    public static bool AuthenticateUser(string email, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM Users WHERE Email=@Email AND Password=@Password";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                long count = (long)command.ExecuteScalar();
                return count > 0;
            }
        }
    }

    
public static void InsertFeedback(int userid, string name, string mail, string message)
{
    string dbPath = "data.db"; // make sure the path is correct
    using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
    {
        connection.Open();
        string query = "INSERT INTO Feedback (userid, Name, email, message) VALUES (@userid, @name, @mail, @message)";
        
        using (var cmd = new SQLiteCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@userid", userid);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@mail", mail);
            cmd.Parameters.AddWithValue("@message", message);

            cmd.ExecuteNonQuery();
        }

        connection.Close();
    }
}


    internal static DataTable GetMyHostels()
    {
        throw new NotImplementedException();
    }

    
}
