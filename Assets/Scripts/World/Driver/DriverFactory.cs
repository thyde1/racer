using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class DriverFactory
{
    private readonly List<string> otherNames;

    public DriverFactory()
    {
        var namesFile = Resources.Load<TextAsset>("FamilyNames");
        this.otherNames = namesFile.text.Split('\n').ToList();
    }

    public Driver Create(int player)
    {
        return new Driver(player, player == 0 ? this.GetRandomDriverName() : $"Player {NumberUtils.NumberToWords(player)}");
    }

    private string GetRandomDriverName()
    {
        if (this.driverNames.Any())
        {
            return PopRandom(this.driverNames);
        }

        var asciiA = Encoding.ASCII.GetBytes("A")[0];
        var randomLetterIndex = Mathf.FloorToInt(UnityEngine.Random.Range(0, 26));
        var asciiLetter = Encoding.ASCII.GetString(new[] { Convert.ToByte(asciiA + randomLetterIndex) });
        return $"{asciiLetter}. {PopRandom(this.otherNames)}";
    }

    private List<string> driverNames = new List<string>
    {
        "T. Wombats",
        "P. Dickman",
        "J. Jigglebot",
        "G. Smiggleput",
        "A. Raider",
        "A. J. Muggins",
        "G. Mark"
    };

    private static T PopRandom<T>(List<T> list)
    {
        var index = Mathf.FloorToInt(UnityEngine.Random.Range(0, list.Count()));
        var value = list[index];
        list.RemoveAt(index);
        return value;
    }
}