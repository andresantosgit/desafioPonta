using System.Security.Cryptography;
using System.Text;

public interface ICryptoService
{
    string EncryptPassword(string password);
}

public class CryptoService : ICryptoService
{
    public string EncryptPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}
