public static class Utilities
{
    public static bool None<TSource>(this IEnumerable<TSource> source)
    {
        return !source.Any();
    }

    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        return !source.Any(predicate);
    }

    public static string CapitalizeFirstWord(this string str)
    {
        if (str.Length == 0)
            return str;
        else if (str.Length == 1)
            str.ToUpper();
        else
            str = char.ToUpper(str[0]) + str.Substring(1).ToLower();
        return str;
    }
}