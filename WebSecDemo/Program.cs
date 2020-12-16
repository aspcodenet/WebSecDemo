/*
 * (C) Stefan Holmberg Systementor AB
 * Made for webinar https://education.systementor.se
 *
 */

using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebSecDemo.Models;

namespace WebSecDemo
{
    class Program
    {
        const string ConnectionString = "";
        static void Main(string[] args)
        {
            using (var context = new MyCoolContext())
            {
                context.Database.Migrate();
                //Setup data
                SetupPlaintextUsers(context);
                SetupHashedUsers(context);
                SetupCrack();

                while (true)
                {
                    Console.WriteLine("1. Login plaintext");
                    Console.WriteLine("2. Login hash");
                    Console.WriteLine("3. Crack a hash");
                    string action = Console.ReadLine();
                    if (action == "1")
                    {
                        Login(context);
                    }
                    if (action == "2")
                    {
                        LoginHash(context);
                    }
                    if (action == "3")
                    {
                        Crack();
                    }
                }
            }



            Console.WriteLine("Hello World!");
        }

        static void Crack()
        {
            Console.WriteLine("***** enter hash ******");
            string hash = Console.ReadLine();
            using (var f = File.OpenText("hashes.txt"))
            {
                while (true)
                {
                    var line = f.ReadLine();
                    if (line == null) break;
                    var parts = line.Split(':');
                    var hashPart = parts[1];
                    if (hashPart == hash)
                    {
                        Console.WriteLine($"Ok...lösenordet är:{parts[0]}");
                        Console.WriteLine("Press enter to continue");
                        Console.ReadLine();
                        return;
                    }
                }

            }
            Console.WriteLine("Not found...");
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
            return;



        }

        private static void SetupCrack()
        {
            if (File.Exists("hashes.txt")) return;
            
            using (var hashes = File.AppendText("hashes.txt"))
            {
                foreach(var pwd in pwds)
                    hashes.WriteLine(pwd + ":" + CreateHash(pwd));
                using (var f = File.OpenText("example.dict"))
                {
                    while (true)
                    {
                        var line = f.ReadLine();
                        if (line == null) return;
                        hashes.WriteLine(line + ":" + CreateHash(line));
                    }
                }
            }

        }

        private static void SetupHashedUsers(MyCoolContext context)
        {
            if (context.HashedAccounts.Any()) return;

            foreach (var acc in context.Accounts)
            {
                var ha = new UserAccountHashed
                {
                    UserId = acc.UserId,
                    Password = CreateHash(acc.Password)
                };
                context.HashedAccounts.Add(ha);
            }


            context.SaveChanges();
        }

        private static Random random = new Random();

        private static void SetupPlaintextUsers(MyCoolContext context)
        {
            if (context.Accounts.Any()) return;

            for (var i = 0; i < 500; i++)
            {
                context.Accounts.Add(CreateBadAccount());
                context.Accounts.Add(CreateGoodAccount());

            }


            context.SaveChanges();
        }

        private static UserAccount CreateGoodAccount()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int len = random.Next(5, 11);

            var uname = new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
            var pwd = new string(Enumerable.Repeat(chars, 14).Select(s => s[random.Next(s.Length)]).ToArray());
            return new UserAccount
            {
                UserId = uname,
                Password = pwd
            };

        }

        static UserAccount CreateBadAccount()
        { 
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int len = random.Next(5, 11);

            var uname = new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
            var pwd = pwds[random.Next(0,pwds.Length)];
            return new UserAccount { 
                UserId = uname,
                Password =  pwd
            };
        }

        private static void Login(MyCoolContext context)
        {
            Console.WriteLine("***** login ******");
            Console.Write("Enter username:");
            string uid = Console.ReadLine();
            Console.Write("Enter password:");
            string pwd = Console.ReadLine();
            var account = context.Accounts.FirstOrDefault(a => a.UserId == uid);
            if (account == null || account.Password != pwd)
            {
                Console.WriteLine("Invalid username or password");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($"You are logged in as {account.UserId}");
            Console.WriteLine("Press enter to logout");
            Console.ReadLine();
        }


        private static void LoginHash(MyCoolContext context)
        {
            Console.WriteLine("***** login ******");
            Console.Write("Enter username:");
            string uid = Console.ReadLine();
            Console.Write("Enter password:");
            string pwd = Console.ReadLine();

            
            // HÄR ÄR ÄNDRINGEN !!!!!
            pwd = CreateHash(pwd);       
            var account = context.HashedAccounts.FirstOrDefault(a => a.UserId == uid);



            if (account == null || account.Password != pwd)
            {
                Console.WriteLine("Invalid username or password");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($"You are logged in as {account.UserId}");
            Console.WriteLine("Press enter to logout");
            Console.ReadLine();
        }



        private static string []pwds = {"123456",
            "123456789",
            "qwerty",
            "password",
            "1234567",
            "12345678",
            "12345",
            "iloveyou",
            "111111",
            "123123",
            "abc123",
            "qwerty123",
            "1q2w3e4r",
            "admin",
            "qwertyuiop",
            "654321",
            "555555",
            "lovely",
            "7777777",
            "888888",
            "princess",
            "dragon",
            "password1",
            "123qwe",
            "666666"};


        public static string CreateHash(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
