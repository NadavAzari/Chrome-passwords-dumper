using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Chrome_passwords_dump
{
    internal class LocalState
    {
        private readonly string Path;
        private string EncryptedKey;
        public byte[] Key { get; private set; }

        public LocalState(string path) { this.Path = path; DecryptKey(); }


        private void DecryptKey()
        {
            FindPlainKey();

            byte[] decodedKey = Convert.FromBase64String(EncryptedKey);
            byte[] encryptedKey = decodedKey.Skip(5).ToArray();
            this.Key = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
        }


        private void FindPlainKey()
        {
            using(StreamReader file = File.OpenText(Path))
            {
                string localState = file.ReadToEnd();
                JsonDocument doc = JsonDocument.Parse(localState);
                JsonElement root = doc.RootElement;
                JsonElement osCrypt =  root.GetProperty("os_crypt");
  
                this.EncryptedKey = osCrypt.GetProperty("encrypted_key").ToString();
            }
        }
    }
}
