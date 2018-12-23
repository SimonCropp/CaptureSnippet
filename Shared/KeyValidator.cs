using System;

class KeyValidator
{
    public static void ValidateKeyDoesNotStartOrEndWithSymbol(string key)
    {
        if (key.Contains(" "))
        {
            throw new Exception($"Key should not contain whitespace. Key: {key}");
        }
        if (char.IsLetterOrDigit(key, 0) && char.IsLetterOrDigit(key, key.Length - 1))
        {
            return;
        }
        throw new Exception($"Key should not start or end with symbols. Key: {key}");
    }
}