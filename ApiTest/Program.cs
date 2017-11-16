using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.RestClient;

namespace ApiTest
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {

            RunAsync().Wait();

            Console.ReadKey();

        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://www.metaweather.com/api/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var location = await GetLocation("gothenburg");
            var weather = await GetWeatherAsync($"location/{location[0].woeid}/{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month}/{DateTime.UtcNow.Day}");

            Console.WriteLine($"The weather in {location[0].title} is {weather[0].the_temp} celsius");

        }
        public static async Task<List<Location>> GetLocation(string location)
        {
            List<Location> searchedLocation = null;
            var apiResponse = await client.PostAsJsonAsync<Location>($"location/search/?query={location}", new Location());
            if (apiResponse.IsSuccessStatusCode)
            {
                
                searchedLocation = await apiResponse.Content.ReadAsAsync<List<Location>>();

            }
            return searchedLocation;
        }

        private static async Task<List<Weather>> GetWeatherAsync(string path)
        {
            List<Weather> weather = null;
            var response = await client.PostAsJsonAsync<Weather>(path, new Weather());
            if (response.IsSuccessStatusCode)
            {
                weather = await response.Content.ReadAsAsync<List<Weather>>();
            }
            return weather;
        }

    }
}
