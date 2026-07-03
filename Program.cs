namespace PopulationDensity;
using System.IO;       // StreamWriter and Stream
using System.Net.Http; // HttpClient
using System.Text.Json; // Parser
using System.Linq; // to sort the countries
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetEnv; // Needed for the secure .env token

class Program
{
    static async Task Main(string[] args) 
    {
        // safety net
        try
        {
            // Load environment variables from .env file
            DotNetEnv.Env.Load();
            string token = Environment.GetEnvironmentVariable("API_TOKEN") ?? "";

            //empty list to get all countries so we can sort them later
            List<Country> allCountries = new List<Country>();

            // connect to API
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            
            // Pagination setup
            int currentOffset = 0;
            int limit = 100; 
            bool hasMoreData = true;

            Console.WriteLine("Starting data ingestion pipeline...");

            while (hasMoreData)
            {
                // get the data as a string
                string url = $"https://api.restcountries.com/countries/v5?limit={limit}&offset={currentOffset}";
                string jsonContent = await client.GetStringAsync(url);

                // parser
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
                JsonElement root = doc.RootElement;
                
                // Navigate the wrapper: root -> data -> objects
                if (root.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("objects", out JsonElement objects))
                {
                    int recordsInChunk = objects.GetArrayLength();
                    Console.WriteLine($"Fetched {recordsInChunk} records at offset {currentOffset}...");

                    // If no more data, break loop
                    if (recordsInChunk == 0)
                    {
                        hasMoreData = false;
                        break;
                    }

                    // loop through each country and get data
                    foreach (JsonElement e in objects.EnumerateArray())
                    {
                        // pull the strings
                        string name = "";
                        if (e.TryGetProperty("names", out var namesObj) && namesObj.TryGetProperty("common", out var comm1)) name = comm1.GetString() ?? "";
                        else if (e.TryGetProperty("name", out var nameObj))
                        {
                            if (nameObj.ValueKind == JsonValueKind.String) name = nameObj.GetString() ?? "";
                            else if (nameObj.TryGetProperty("common", out var comm2)) name = comm2.GetString() ?? "";
                        }
                
                        string region = e.TryGetProperty("region", out var r) ? r.GetString() ?? "" : "";
                        string subregion = e.TryGetProperty("subregion", out var s) ? s.GetString() ?? "" : "";
                        
                        // pul the numbers
                        int population = 0;
                        if (e.TryGetProperty("population", out var popObj) && popObj.ValueKind == JsonValueKind.Number) population = popObj.GetInt32();

                        double area = 0.0;
                        if (e.TryGetProperty("area", out var areaObj))
                        {
                            if (areaObj.ValueKind == JsonValueKind.Number) area = areaObj.GetDouble();
                            else if (areaObj.ValueKind == JsonValueKind.Object && areaObj.TryGetProperty("kilometers", out var km)) area = km.GetDouble();
                        }
                        
                        // Prevent blank lines in CSV
                        if (!string.IsNullOrEmpty(name))
                        {
                            // connect to class
                            Country country = new Country
                            {
                                Name = name,
                                Region = region,
                                SubRegion = subregion,
                                Population = population,
                                Area = area
                            };
                            // add to collection
                            allCountries.Add(country);
                        }
                    }
                    // Increment offset for next page
                    currentOffset += limit;
                }
                else
                {
                    Console.WriteLine("Data mapping failed: Could not find data.objects wrapper.");
                    break;
                }
            }

            // lambda to calculate the population density for each country then sort the countries
            var sortedCountries = allCountries.OrderByDescending(c => c.CalculateDensity()).ToList();

            // csv files with UTF-8 capability
            using StreamWriter topWriter = new StreamWriter("high_density_countries.csv", false, System.Text.Encoding.UTF8);
            using StreamWriter bottomWriter = new StreamWriter("low_density_countries.csv", false, System.Text.Encoding.UTF8);
            
            // write headers
            topWriter.WriteLine("Name,Region,SubRegion,Population,Area,Density");
            bottomWriter.WriteLine("Name,Region,SubRegion,Population,Area,Density");
            
            //  write  top 25 to csv
            foreach(var country in sortedCountries.Take(25))
            {
                topWriter.WriteLine(country.CleanString());
            }
            // write bottom 25 to csv
            foreach(var country in sortedCountries.TakeLast(25))
            {
                bottomWriter.WriteLine(country.CleanString());
            }

            Console.WriteLine($"Pipeline executed successfully. Processed {allCountries.Count} countries.");
        }
        catch (Exception e)
        {
            // catches any network or file drops
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}