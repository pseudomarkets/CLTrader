using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Figgle;
using Newtonsoft.Json;

/*
 * CLTrader - Command Line Trading Client for Pseudo Markets
 * Author: Shravan Jambukesan <shravan@shravanj.com>
 * Date: 3/11/2020
 */

namespace CLTrader
{
    class Program
    {
        public static string BASE_URL = "https://app.pseudomarkets.live";
        private static string username = "";
        private static string token = "";
        
        public static void Main(string[] args)
        {
            Console.WriteLine(FiggleFonts.Standard.Render("CLTrader"));
            Console.WriteLine("(c) 2020 Pseudo Markets");
            Console.WriteLine("https://github.com/PseudoMarkets");
            Console.WriteLine("Connected to: " + BASE_URL);
            token = StartSession().GetAwaiter().GetResult();
            if (!String.IsNullOrEmpty(token))
            {
                ClientMenu();
            }
            else
            {
                Console.WriteLine("Could not login to account. Please restart CLTrader and try again.");
            }
        }

        public static async Task<string> StartSession()
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            LoginInput loginInput = new LoginInput()
            {
                username = username,
                password = password
            };
            var client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URL);
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Users/Login");
            string jsonRequest = JsonConvert.SerializeObject(loginInput);
            var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            request.Content = stringContent;
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<LoginOutput>(responseString);
            string userToken = responseJson.Token;
            return userToken;
        }

        public static void ClientMenu()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("PSEUDO MARKETS COMMAND LINE TRADING CLIENT");
            Console.WriteLine("LOGGED IN AS: " + username);
            Console.WriteLine("===========================================");
            Console.WriteLine("1. VIEW INDICES");
            Console.WriteLine("2. GET QUOTE");
            Console.WriteLine("3. GET SMART QUOTE");
            Console.WriteLine("4. EXECUTE TRADE");
            Console.WriteLine("5. VIEW POSITIONS");
            Console.WriteLine("6. ACCOUNT SUMMARY");
            Console.WriteLine("7. EXIT");
            Console.Write("Enter selection: ");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    GetIndices();
                    break;
                case "2":
                    GetQuote();
                    break;
                case "3":
                    GetSmartQuote();
                    break;
                case "4": 
                    ExecuteTrade();
                    break;
                case "5":
                    ViewPositions();
                    break;
                case "6":
                    ViewAccountSummary();
                    break;
                case "7":
                    break;
            }
        }

        public static void GetIndices()
        {
            Console.WriteLine("Indices here");
            ClientMenu();
        }

        public static void GetQuote()
        {
            Console.WriteLine("Quotes here");
            ClientMenu();
        }

        public static void GetSmartQuote()
        {
            Console.WriteLine("Smart Quote Here");
            ClientMenu();
        }

        public static void ExecuteTrade()
        {
            Console.WriteLine("Trade here");
            ClientMenu();
        }

        public static void ViewPositions()
        {
            Console.WriteLine("View positions");
            ClientMenu();
        }

        public static void ViewAccountSummary()
        {
            Console.WriteLine("View account summary");
            ClientMenu();
        }
    }
    
    public class LoginOutput
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; }
    }
    
    public class LoginInput
    {
        public string username { get; set; }
        public string password { get; set; }
    }

}