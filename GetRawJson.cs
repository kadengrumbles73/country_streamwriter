using System;
using System.Net.Http;
using System.Threading.Tasks;
/*
class GetRawJson
{
    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer rc_live_7cfd9f4705bd4374b2e3a48a29ef1970");

        string url = "https://api.restcountries.com/countries/v5?all=true";
        string jsonContent = await client.GetStringAsync(url);

        Console.WriteLine("--- RAW JSON RESPONSE ---");
        // Print the first 3000 characters to verify structure
        Console.WriteLine(jsonContent);
        Console.WriteLine("\n-------------------------");
    }
}
*/

// to make it work put this in the project file <StartupObject>GetRawJson</StartupObject>