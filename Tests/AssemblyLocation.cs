public static class AssemblyLocation
{
    static AssemblyLocation()
    {
        CurrentDirectory = System.AppContext.BaseDirectory;
    }

    public static string CurrentDirectory;
}