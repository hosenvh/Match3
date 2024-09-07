using System;
using System.Linq;
using System.Text.RegularExpressions;


public static class StringExtensions
{
    //    public static bool IsLetterOrDigit(this string self)
    //    {
    //        if (string.IsNullOrEmpty(self)) return false;
    //        for (var i = 0; i < self.Length; i++)
    //        {
    //            var isNotLetter = self[i] != ' ' && char.IsLetterOrDigit(self[i]) == false;
    //            var isSymbol = char.IsSymbol(self[i]);
    //            var isControl = char.IsControl(self[i]);
    //            if (isNotLetter || isSymbol || isControl)
    //                return false;
    //        }
    //        return true;
    //    }

    public static string SplitCamelCase(this string str)
    {
        return Regex.Replace(
            Regex.Replace(
                str,
                @"(\P{Ll})(\P{Ll}\p{Ll})",
                "$1 $2"
            ),
            @"(\p{Ll})(\P{Ll})",
            "$1 $2"
        );
    }

    public static string FirstCharToUpper(this string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var first = text.Take(1).ToArray()[0].ToString().ToUpper();
        var rest = text.Skip(1).Aggregate("", ((xs, x) => xs + x));
        return first + rest;
    }

    public static string TryRemoveSubString(this string sourceString, string toRemove)
    {
        int index = sourceString.IndexOf(toRemove, StringComparison.InvariantCulture);
        return index < 0 ? sourceString : sourceString.Remove(index, toRemove.Length);
    }

    public static int GetIndexOfFirstDifferenceWith(this string self, string target)
    {
        int index = -1;
        for (int i = 0; i < self.Length && i < target.Length; i++)
            if (self[i] != target[i])
            {
                index = i;
                break;
            }

        if (index == -1 && self.Length != target.Length)
            index = Math.Min(self.Length, target.Length);

        return index;
    }
}