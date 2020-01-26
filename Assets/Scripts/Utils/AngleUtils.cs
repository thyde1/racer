public static class AngleUtils
{
    public static float GetSignedSmallestAngleBetween(float from, float to)
    {
        var diff = (to - from + 180) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }
}
