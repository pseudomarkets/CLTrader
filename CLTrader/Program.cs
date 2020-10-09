using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Figgle;
using Newtonsoft.Json;
using Internal.ReadLine;

/*
 * CLTrader - Command Line Trading Client for Pseudo Markets
 * Author: Shravan Jambukesan <shravan@shravanj.com>
 * Date: 3/11/2020
 */

namespace CLTrader
{
    class Program
    {
        public static string BASE_URL = "";
        private static string username = "";
        private static string token = "";
        private static string VERSION_STRING = "1.0.3";
        public static void Main(string[] args)
        {
            Console.WriteLine(FiggleFonts.Standard.Render("CLTrader"));
            Console.WriteLine("RELEASE VERSION " + VERSION_STRING);
            Console.WriteLine("(c) 2020 Pseudo Markets");
            Console.WriteLine("https://github.com/PseudoMarkets");
            BASE_URL = GetActiveAppServer();
            Console.WriteLine("Connected to: " + BASE_URL);
            StartupMenu();
        }

        public static async Task<string> StartSession()
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = ReadLine.ReadPassword();
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

        public static void StartupMenu()
        {
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create new account");
            Console.WriteLine("3. Exit");
            Console.WriteLine();
            Console.Write("Enter selection: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    token = StartSession().GetAwaiter().GetResult();
                    ClientMenu();
                    break;
                case "2":
                    CreateNewAccount();
                    break;
                case "3":
                    break;
                default:
                    Console.WriteLine("Please enter a valid selection (1 - 3)");
                    StartupMenu();
                    break;
            }

        }

        public static void CreateNewAccount()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();


            LoginInput loginInput = new LoginInput()
            {
                username = username,
                password = password
            };

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(BASE_URL);
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/Users/Register");
                string jsonRequest = JsonConvert.SerializeObject(loginInput);
                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                var response = client.SendAsync(request).GetAwaiter().GetResult();
                var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var responseJson = JsonConvert.DeserializeObject<StatusOutput>(responseString);

                Console.WriteLine(responseJson.message);
                if (responseJson.message == "User created")
                {
                    Console.WriteLine("Login to your newly created account");
                    token = StartSession().GetAwaiter().GetResult();
                    ClientMenu();
                }
                else if (responseJson.message == "User already exists")
                {
                    Console.WriteLine("Please try again with a different username");
                    CreateNewAccount();
                }
                else
                {
                    Console.WriteLine("An error occured while trying to create an account, please try again");
                    CreateNewAccount();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
            Console.WriteLine("4. EQUITY/ETF RESEARCH");
            Console.WriteLine("5. EXECUTE TRADE");
            Console.WriteLine("6. VIEW POSITIONS");
            Console.WriteLine("7. VIEW TRANSACTIONS");
            Console.WriteLine("8. ACCOUNT SUMMARY");
            Console.WriteLine("9. EXIT");
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
                    ViewResearch();
                    break;
                case "5":
                    ExecuteTrade();
                    break;
                case "6":
                    ViewPositions();
                    break;
                case "7":
                    ViewTransactions();
                    break;
                case "8":
                    ViewAccountSummary();
                    break;
                case "9":
                    break;
                default:
                    Console.WriteLine("Please enter a valid selection (1 - 8)");
                    ClientMenu();
                    break;
            }
        }

        public static void GetIndices()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("US MARKET INDICES" + "\n");
                var client = new HttpClient();
                var response = client.GetAsync(BASE_URL + "/api/Quotes/Indices");
                var responseString = response.Result.Content.ReadAsStringAsync();
                var resonseJson = JsonConvert.DeserializeObject<IndicesOutput>(responseString.Result);
                var indices = resonseJson?.indices;
                foreach (StockIndex index in indices)
                {
                    Console.WriteLine("INDEX: " + index.name);
                    Console.WriteLine("POINTS: " + index.points + "\n");
                }
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve indices, please try again later");
                ClientMenu();
            }
        }

        public static void GetQuote()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("LATEST PRICE QUOTE");
                Console.Write("Enter symbol: ");
                string input = Console.ReadLine().ToUpper();
                var client = new HttpClient();
                var response = client.GetAsync(BASE_URL + "/api/Quotes/LatestPrice/" + input);
                var responseString = response.Result.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<LatestPriceOutput>(responseString.Result);
                Console.WriteLine("SYMBOL: " + responseJson.symbol);
                Console.WriteLine("PRICE: $" + responseJson.price);
                Console.WriteLine("SOURCE: " + responseJson.source);
                Console.WriteLine("QUOTE TIMESTAMP: " + responseJson.timestamp);
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve quote, please try again later. Note that only ETF and US Equity symbols are supported.");
                ClientMenu();
            }
        }

        public static void GetSmartQuote()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("SMART PRICE QUOTE");
                Console.Write("Enter symbol: ");
                string input = Console.ReadLine().ToUpper();
                var client = new HttpClient();
                var response = client.GetAsync(BASE_URL + "/api/Quotes/SmartQuote/" + input);
                var responseString = response.Result.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<LatestPriceOutput>(responseString.Result);
                Console.WriteLine("SYMBOL: " + responseJson.symbol);
                Console.WriteLine("PRICE: $" + responseJson.price);
                Console.WriteLine("SOURCE: " + responseJson.source);
                Console.WriteLine("QUOTE TIMESTAMP: " + responseJson.timestamp);
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve quote, please try again later. Note that only ETF and US Equity symbols are supported.");
                ClientMenu();
            }

        }

        public static void ViewResearch()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("EQUITY/ETF RESEARCH");
                Console.Write("Enter symbol: ");
                string input = Console.ReadLine().ToUpper();
                var client = new HttpClient();
                var response = client.GetAsync(BASE_URL + "/api/Quotes/DetailedQuote/" + input + "/1day");
                var responseString = response.Result.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<DetailedQuoteOutput>(responseString.Result);
                Console.WriteLine("SYMBOL: " + responseJson.symbol);
                Console.WriteLine("NAME: " + responseJson.name);
                Console.WriteLine("OPEN: $" + responseJson.open);
                Console.WriteLine("CLOSE: $" + responseJson.close);
                Console.WriteLine("HIGH: $" + responseJson.high);
                Console.WriteLine("LOW: $" + responseJson.low);
                Console.WriteLine("PREV CLOSE: $" + responseJson.previousClose);
                Console.WriteLine("CHANGE: $" + responseJson.change);
                Console.WriteLine("PERCENT CHANGE: " + responseJson.changePercentage + "%");
                Console.WriteLine("TIMESTAMP: " + responseJson.timestamp);
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve data, please try again later. Note that only ETF and US Equity symbols are supported.");
                ClientMenu();
            }

        }

        public static void ExecuteTrade()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("EXECUTE TRADE - PLACE ORDER");
                Console.Write("Enter symbol: ");
                string symbol = Console.ReadLine().ToUpper();
                Console.Write("Order type (BUY/SELL/SELLSHORT): ");
                string orderType = Console.ReadLine().ToUpper();
                Console.Write("Quantity: ");
                string quantity = Console.ReadLine();

                var quoteClient = new HttpClient();
                var quoteResponse = quoteClient.GetAsync(BASE_URL + "/api/Quotes/SmartQuote/" + symbol);
                var quoteResponseString = quoteResponse.Result.Content.ReadAsStringAsync();
                var quoteJson = JsonConvert.DeserializeObject<LatestPriceOutput>(quoteResponseString.Result);

                Console.WriteLine("ORDER SUMMARY");
                Console.WriteLine("SYMBOL: " + symbol);
                Console.WriteLine("QUANTITY: " + quantity);
                Console.WriteLine("PRICE: $" + quoteJson.price);
                double totalValue = quoteJson.price * Int32.Parse(quantity);
                Console.WriteLine("TOTAL VALUE: $" + totalValue);

                Console.Write("Execute trade? (Y/N): ");
                string input = Console.ReadLine()?.ToUpper();
                if (input == "Y")
                {
                    TradeExecInput tradeExecInput = new TradeExecInput()
                    {
                        Token = token,
                        Symbol = symbol.ToUpper(),
                        Quantity = Int32.Parse(quantity),
                        Type = orderType
                    };
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(BASE_URL);
                    var request = new HttpRequestMessage(HttpMethod.Post, "/api/Trade/Execute");
                    string jsonRequest = JsonConvert.SerializeObject(tradeExecInput);
                    var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    request.Content = stringContent;
                    var response = client.SendAsync(request);
                    var responseString = response.Result.Content.ReadAsStringAsync();
                    var responseJson = JsonConvert.DeserializeObject<TradeExecOutput>(responseString.Result);
                    Console.WriteLine("===========================================");
                    Console.WriteLine("ORDER SUCCESSFULLY EXECUTED");
                    Console.WriteLine("SYMBOL: " + responseJson.Symbol);
                    Console.WriteLine("PRICE: $" + responseJson.Price);
                    Console.WriteLine("QUANTITY: " + responseJson.Quantity);
                    Console.WriteLine("ORDER TYPE: " + responseJson.Type);
                    Console.WriteLine("ORDER ID: " + responseJson.Id);
                    Console.WriteLine("TRANSACTION ID: " + responseJson.TransactionID);
                    Console.WriteLine("ORDER DATE: " + responseJson.Date);
                    Console.WriteLine("===========================================");
                    Console.WriteLine("Enter to return back to menu...");
                    Console.ReadKey();
                    ClientMenu();
                }
                else
                {
                    Console.WriteLine("Order cancelled");
                    ClientMenu();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to execute trade, please try again later. Please note that only ETF and US Equity trades are supported.");
                ClientMenu();
            }

        }

        public static void ViewPositions()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("ACCOUNT - POSITIONS" + "\n");
                AccountViewInput accountViewInput = new AccountViewInput()
                {
                    token = token
                };
                var client = new HttpClient();
                client.BaseAddress = new Uri(BASE_URL);
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Positions");
                string jsonRequest = JsonConvert.SerializeObject(accountViewInput);
                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                var response = client.SendAsync(request);
                var responseString = response.Result.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<IList<Position>>(responseString.Result);
                foreach (Position position in responseJson)
                {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("SYMBOL: " + position.symbol);
                    Console.WriteLine("QUANTITY: " + position.quantity);
                    Console.WriteLine("INVESTED VALUE: $" + position.value);
                    Console.WriteLine("---------------------------------------" + "\n");
                }
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve positions, please try again later.");
                ClientMenu();
            }
        }

        public static void ViewAccountSummary()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("ACCOUNT - SUMMARY" + "\n");
                AccountViewInput accountViewInput = new AccountViewInput()
                {
                    token = token
                };
                var client = new HttpClient();
                client.BaseAddress = new Uri(BASE_URL);
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Summary");
                string jsonRequest = JsonConvert.SerializeObject(accountViewInput);
                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                var response = client.SendAsync(request);
                var responseString = response.Result.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<AccountSummaryOutput>(responseString.Result);
                Console.WriteLine("ACCOUNT BALANCE: $" + responseJson.AccountBalance);
                Console.WriteLine("TOTAL INVESTED VALUE: $" + responseJson.TotalInvestedValue);
                Console.WriteLine("TOTAL CURRENT VALUE: $" + responseJson.TotalCurrentValue);
                Console.WriteLine("INVESTMENT GAIN OR LOSS: $" + responseJson.InvestmentGainOrLoss);
                Console.WriteLine("INVESTMENT GAIN OR LOSS PERCENTAGE: " + responseJson.InvestmentGainOrLossPercentage + "%");
                Console.WriteLine("NUMBER OF POSITIONS: " + responseJson.NumberOfPositions);
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve account summary, please try again later.");
                ClientMenu();
            }
        }

        public static void ViewTransactions()
        {
            try
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("ACCOUNT - TRANSACTIONS" + "\n");
                AccountViewInput accountViewInput = new AccountViewInput()
                {
                    token = token
                };
                var client = new HttpClient();
                client.BaseAddress = new Uri(BASE_URL);
                var request = new HttpRequestMessage(HttpMethod.Post, "/api/Account/Transactions");
                string jsonRequest = JsonConvert.SerializeObject(accountViewInput);
                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                var response = client.SendAsync(request);
                var responseString = response.Result.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<IList<Transaction>>(responseString.Result);
                foreach (Transaction transaction in responseJson)
                {
                    Console.WriteLine("---------------------------------------");
                    Console.WriteLine("TYPE: " + transaction.Type);
                    Console.WriteLine("SYMBOL: " + transaction.Symbol);
                    Console.WriteLine("QUANTITY: " + transaction.Quantity);
                    Console.WriteLine("PRICE: " + transaction.Price);
                    Console.WriteLine("DATE: " + transaction.Date);
                    Console.WriteLine("---------------------------------------" + "\n");
                }
                Console.WriteLine("===========================================");
                Console.WriteLine("Enter to return back to menu...");
                Console.ReadKey();
                ClientMenu();
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to retrieve positions, please try again later.");
                ClientMenu();
            }
        }

        public static string GetActiveAppServer()
        {
            var client = new HttpClient();
            var response = client.GetAsync("https://commandcenter.pseudomarkets.live" + "/api/ServerPool");
            var responseString = response.Result.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<ServerPoolList>(responseString.Result);

            var activeServer = responseJson.ServerList
                .First(x => x.ActiveCluster == true && x.IsFailover == false).Address;

            return activeServer;
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

    public class AccountViewInput
    {
        public string token { get; set; }
    }

    public class IndicesOutput
    {
        public List<StockIndex> indices;
        public string source { get; set; }
        public DateTime timestamp { get; set; }
    }

    public class StockIndex
    {
        public string name { get; set; }
        public double points { get; set; }
    }

    public class LatestPriceOutput
    {
        public string symbol { get; set; }
        public double price { get; set; }
        public DateTime timestamp { get; set; }
        public string source { get; set; }
    }

    public class DetailedQuoteOutput
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public DateTime timestamp { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public long volume { get; set; }
        public double previousClose { get; set; }
        public double change { get; set; }
        public double changePercentage { get; set; }
    }

    public class TradeExecInput
    {
        public string Token { get; set; }
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
    }

    public class TradeExecOutput
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string TransactionID { get; set; }
    }


    public class PositionsOutput
    {
        public List<Position> Positions { get; set; }
    }

    public class Position
    {
        public int id { get; set; }
        public int accountId { get; set; }
        public int orderId { get; set; }
        public float value { get; set; }
        public string symbol { get; set; }
        public int quantity { get; set; }
    }

    public class AccountSummaryOutput
    {
        public int AccountId { get; set; }
        public double AccountBalance { get; set; }
        public double TotalInvestedValue { get; set; }
        public double TotalCurrentValue { get; set; }
        public double InvestmentGainOrLoss { get; set; }
        public double InvestmentGainOrLossPercentage { get; set; }
        public int NumberOfPositions { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public string TransactionID { get; set; }
    }


    public class ServerPoolList
    {
        public List<ServerPool> ServerList { get; set; }
    }

    public class ServerPool
    {
        public int Id { get; set; }
        public string ServerName { get; set; }
        public string Address { get; set; }
        public bool ActiveCluster { get; set; }
        public bool IsFailover { get; set; }
    }

    public class StatusOutput
    {
        public string message { get; set; }
    }

}