using System;
using RestSharp;
using Newtonsoft.Json;
using RestSharp.Authenticators;
using System.Collections.Generic;

namespace StockClient
{
    class Program {
        public class Client {
            public String name { get; set; }

            public double funds { get; set; }

            public Dictionary<String, int> shares { get; set; }
        }

        public class shareInfo
        {
            public int time { get; set; }
            public double price { get; set; }
            public string buySell { get; set; }
            public int amount { get; set; }
        }

        public static void Offer(RestClient client, String stockName, String shareName, int shareAmount, double sharePrice, Boolean buySell)
        {

            String buySellString = buySell ? "buy" : "sell";
            var request = new RestRequest("offer", DataFormat.Json);
            request.AddJsonBody(new { stockExchange = stockName, share = shareName, amount = shareAmount, price = sharePrice, buySell = buySellString });
            var responseJson = client.Post(request);
            Console.WriteLine(responseJson.Content);
        }
        
        public static void getClient(RestClient client)
        {
            var request = new RestRequest("client", DataFormat.Json); 
            var responseJson = client.Get(request); 
            Client userTaken = JsonConvert.DeserializeObject<Client>(responseJson.Content); 
            Console.WriteLine(userTaken.name);
            Console.WriteLine(userTaken.funds);
            foreach (KeyValuePair<String, int> pair in userTaken.shares)
            {
                Console.WriteLine(pair.Key);
                Console.WriteLine(pair.Value);
            };
            Console.ReadKey();
            
        }

        public static void sellAll(RestClient client, String stockName)
        {
            var request = new RestRequest("client", DataFormat.Json);
            var responseJson = client.Get(request);
            Client userTaken = JsonConvert.DeserializeObject<Client>(responseJson.Content);
            foreach (KeyValuePair<String, int> pair in userTaken.shares)
            {
                Offer(client, "Warszawa", pair.Key, pair.Value, getPrice(client, stockName, pair.Key)[0], false);
            };
        }

        public static List<double> getPrice(RestClient client, String stockName, String shareName)
        {
            String htmlPriceEnd = "shareprice/" + stockName + "?share=" + shareName;
            var request = new RestRequest(htmlPriceEnd, DataFormat.Json);
            var responseJson = client.Get(request);
            List<shareInfo> share = JsonConvert.DeserializeObject<List<shareInfo>>(responseJson.Content);
            List<double> prices = new List<double>();
            prices.Add(share[0].price);
            prices.Add(share[1].price);
            return prices;
        }

        public static List<String> buyAllShares(RestClient client, String stockName)
        {
            var request = new RestRequest("shareslist/" + stockName, DataFormat.Json);
            var responseJson = client.Get(request);
            List<String> shareList = JsonConvert.DeserializeObject<List<String>>(responseJson.Content);
            foreach(String element in shareList)
            {
                Offer(client, stockName, element, 20, getPrice(client, stockName, element)[1], true);
            }
            return shareList;
        }

        public 
        static void Main(string[] args) { 
            var client = new RestClient("https://stockserver20201009223011.azurewebsites.net/");
            client.Authenticator = new HttpBasicAuthenticator("01149568@pw.edu.pl", "12345");

            //Offer(client, "Warszawa", "11BIT", 10, getPrice(client, "Warszawa", "11BIT")[1], true);
            //Offer(client, "Warszawa", "ALIOR", 10, getPrice(client, "Warszawa", "ALIOR")[1], true);
            //Offer(client, "Warszawa", "AMREST", 10, getPrice(client, "Warszawa", "AMREST")[1], true);
            //sellAll(client);
            //Offer(client, "Paryz", "CCC", 10, 33, false);
            //Console.WriteLine(getPrice(client, "Paryz", "CCC")[0]);
            //Console.WriteLine(getPrice(client, "Paryz", "CCC")[1]);
            sellAll(client, "Warszawa");
            


        }
    }
}