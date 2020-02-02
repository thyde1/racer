using UnityEngine;

public static class AudioSourceConfigurer
{
    public static void ConfigureDefault(AudioSource audioSource)
    {
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 10;
        audioSource.maxDistance = 50;
        audioSource.spread = 180;
        audioSource.playOnAwake = false;
    }
}
