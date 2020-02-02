using UnityEngine;

public static class VehicleColors
{
    public static Color[] PlayerColors => new[] {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green
    };

    public static Color AIColor => new Color32(255, 128, 0, 255); // Orange
}