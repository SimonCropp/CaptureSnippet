using System;

class NoOpUsingExtractor
{
    public static Func<string, string> Extract = line => null;
}