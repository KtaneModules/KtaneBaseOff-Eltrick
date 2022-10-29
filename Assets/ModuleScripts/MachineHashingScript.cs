using System;
using System.Security.Cryptography;
using System.Text;

public class MachineHashingScript
{
    public static string SHA512Hash(string input)
    {
        SHA512 sha512Instance = SHA512.Create();
        byte[] bytes = sha512Instance.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}