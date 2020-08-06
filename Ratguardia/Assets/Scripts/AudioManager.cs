using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager main;

    public AudioSource cardTheme;

    public AudioSource sfxDraw;
    public AudioSource sfxDiscard;
    public AudioSource sfxFlip;
    public AudioSource sfxShuffle;

    private void Awake()
    {
        // persistent singleton
        if (main == null)
        {
            main = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
