using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static string PLAYER_PREFS_MUSIC_VOLUME = "MUSIC_VOLUME";
    
    public static MusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private float volume = 0.5f;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, defaultValue: 0.5f);
        audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += 0.1f;

        if (volume > 1f)
        {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();

        audioSource.volume = volume;
    }

    public float GetVolume()
    {
        return volume;
    }

}
