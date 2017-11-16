using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.RestClient
{
    public class RestClient
    {

        private static HttpClient _client = new HttpClient();

        public RestClient()
        {
            _client.BaseAddress = new Uri("http://www.metaweather.com/api/");
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<string> RunAsync(string meetingLocation, DateTime date)
        {
            
            var location = await GetLocation(meetingLocation);
            if (location == null|| location.Count == 0 )
            {
                string nullError = $"Could not find the temperature, make sure you search city in english"
                               + " and that the date is current or past";
                return nullError;
            }
            var weather = await GetWeatherAsync($"location/{location[0].woeid}/{date.Year}" +
                                                $"/{date.Month}/{date.Day}");
            if (weather != null && weather.Count > 0)
            {
                if (weather[0].weather_state_name == "Snow")
                {
                    string snow = $"The temperature in {location[0].title} for the specified date is estimated" +
                                      $" to be {weather[0].the_temp} celsius and it will be snowing";
                    return snow;
                }
                if (weather[0].weather_state_name == "Sleet")
                {
                    string sleet = $"The temperature in {location[0].title} for the specified date is estimated" +
                                      $" to be {weather[0].the_temp} celsius and sleet is expected";
                    return sleet;
                }

                if (weather[0].weather_state_name == "Hail")
                {
                    string hail = $"The temperature in {location[0].title} for the specified date is estimated" +
                                      $" to be {weather[0].the_temp} celsius and hail is expected";
                    return hail;
                }
                if (weather[0].weather_state_name == "Thunderstorm")
                {
                    string thunderstorm = $"The temperature in {location[0].title} for the specified date is estimated" +
                                      $" to be {weather[0].the_temp} celsius and a thunderstorm is expected";
                    return thunderstorm;
                }
                if (weather[0].weather_state_name == "Heavy Rain")
                {
                    string heavyRain = $"The temperature in {location[0].title} for the specified date is estimated" +
                                      $" to be {weather[0].the_temp} celsius, heavy rain is expected";
                    return heavyRain;
                }
                if (weather[0].weather_state_name == "Light Rain")
                {
                    string lightRain = $"The temperature in {location[0].title} for the specified date is estimated" +
                                       $" to be {weather[0].the_temp} celsius, light rain is expected";
                    return lightRain;
                }
                if (weather[0].weather_state_name == "Showers")
                {
                    string showers = $"The temperature in {location[0].title} for the specified date is estimated" +
                                       $" to be {weather[0].the_temp} celsius and showers is expected";
                    return showers;
                }
                if (weather[0].weather_state_name == "Heavy Cloud")
                {
                    string heavyCloud = $"The temperature in {location[0].title} for the specified date is estimated" +
                                       $" to be {weather[0].the_temp} celsius and heavy clouds is expected";
                    return heavyCloud;
                }
                if (weather[0].weather_state_name == "Light Cloud")
                {
                    string lightCloud = $"The temperature in {location[0].title} for the specified date is estimated" +
                                       $" to be {weather[0].the_temp} celsius and light clouds is expected";
                    return lightCloud;
                }
                if (weather[0].weather_state_name == "Clear")
                {
                    string clear = $"The temperature in {location[0].title} for the specified date is estimated" +
                                      $" to be {weather[0].the_temp} celsius and the weather is expected to be clear";
                    return clear;
                }
                string response = $"The temperature in {location[0].title} for the specified date is estimated" +
                                  $" to be {weather[0].the_temp} celsius";
                return response;
            }
            string error = $"Could not find the temperature, make sure you search location in english"
                           + "and that the date is current or past";
            return error;

        }
        public async Task<List<Location>> GetLocation(string location)
        {

            List<Location> searchedLocation = null;
            var apiResponse = await _client.PostAsJsonAsync<Location>($"location/search/?query={location}", new Location());
            if (apiResponse.IsSuccessStatusCode)
            {

                searchedLocation = await apiResponse.Content.ReadAsAsync<List<Location>>();

            }
            return searchedLocation;
        }

        private async Task<List<Weather>> GetWeatherAsync(string path)
        {
            List<Weather> weather = null;
            var response = await _client.PostAsJsonAsync<Weather>(path, new Weather());
            if (response.IsSuccessStatusCode)
            {
                weather = await response.Content.ReadAsAsync<List<Weather>>();
            }
            return weather;
        }

    }
}
