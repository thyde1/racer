﻿public static class Ordinal
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
}
