using System.Text;

public class AuthenticationHelper
{
    public static string Encrypt(string username, string password)
    {
        string credentials = $"{username}:{password}";
        byte[] bytes = Encoding.UTF8.GetBytes(credentials);
        return $"Basic {Convert.ToBase64String(bytes)}";
    }

    public static void Decrypt(string encryptedHeader, out string username, out string password)
    {
        var auth = encryptedHeader.Split(' ')[1];
        var usernameAndPassword = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
        username = usernameAndPassword.Split(':')[0];
        password = usernameAndPassword.Split(':')[1];
    }
}
