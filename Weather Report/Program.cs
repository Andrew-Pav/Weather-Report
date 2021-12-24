using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Weather_Report
{
    class Program
    {
        private static string token { get; set; } = "5011973786:AAHwLkhBCHTQJlugGaOhHlSVdkGL_EhVK18";
        private static TelegramBotClient client;

        static string NameCity;
        static float tempOfCity;
        static string nameOfCity;

        static string answerOnWether;
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"Bot_Id: {me.Id} \nBot_Name: {me.FirstName} ");

            client.OnMessage += Bot_OnMessage;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type == MessageType.Text)
            {
                NameCity = message.Text;
                Weather(NameCity);
                Celsius(tempOfCity);
                await client.SendTextMessageAsync(message.Chat.Id, $"{answerOnWether} \n\nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} °C");

                Console.WriteLine(message.Text);
            }
        }

        public static void Weather(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&unit=metric&appid=dd7adfbf32ae16342d7da522ab5efb6c";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                WeatherResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.Temp - 273;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Возникло исключение");
                return;
            }
        }

        public static void Celsius(float celsius)
        {
            if (celsius <= 10)
                answerOnWether = "Температура ниже средней, лучше одется по теплее";
            else
                answerOnWether = "Температура выше средней, можно одется по легче";
        }
    }
}
