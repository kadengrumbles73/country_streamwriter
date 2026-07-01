namespace PopulationDensity;

class Program
{
    static async Task Main(string[] args)
    {
        // safety net
        try
        {
            
        }
        catch (Exception e)
        {
            // catch any network or file drops here cleanly
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}
