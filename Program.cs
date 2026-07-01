namespace PopulationDensity;

class Program
{
    static async Task Main(string[] args)
    {
        // STEP 1: Set up the outer safety net
        try
        {
            // STEP 2: Open your local file gates and write CSV headers
            // (Using StreamWriter)

            // STEP 3: Now you are completely safe to initialize your HttpClient
            // and open your StreamReader to pull the API data!
        }
        catch (Exception e)
        {
            // Catch any network or file drops here cleanly
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}
