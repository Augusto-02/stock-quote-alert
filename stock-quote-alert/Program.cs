
class Program
{
    static void Main(string[] args)
    {
        string request = Console.ReadLine();
        List<StockPriceAlert> requestData = new List<StockPriceAlert>();
        try
        {
            requestData = StockPriceAlertParser.Validate(request);
        }
        catch (ArgumentException error)
        {
            Console.WriteLine(error.Message);
            return;
        }

        Console.WriteLine(requestData);
    }
}