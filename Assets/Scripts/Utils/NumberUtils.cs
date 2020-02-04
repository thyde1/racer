using System;

public static class NumberUtils
{
    public static string GetOrdinal(int value)
    {
        switch (value % 100)
        {
            case 11:
            case 12:
            case 13:
                return value + "th";
        }

        switch (value % 10)
        {
            case 1:
                return value + "st";
            case 2:
                return value + "nd";
            case 3:
                return value + "rd";
            default:
                return value + "th";
        }
    }

    public static string NumberToWords(int value)
    {
        switch (value)
        {
            case 1:
                return "One";
            case 2:
                return "Two";
            case 3:
                return "Three";
            case 4:
                return "Four";
            default:
                throw new NotImplementedException("Can only convert 1, 2, 3, 4 to words");
        }
    }
}
