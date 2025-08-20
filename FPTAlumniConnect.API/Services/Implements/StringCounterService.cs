namespace StringUpdateApi.Services;

public class StringCounterService
{
    private int _counter = 0;

    public string GetNextString()
    {
        _counter++;
        return $"Update-{_counter}";
    }
}