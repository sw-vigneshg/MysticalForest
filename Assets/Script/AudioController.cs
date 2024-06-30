using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [Header("Audio Sources ********** ")]
    public AudioSource Bgm;
    public AudioSource ButtonClick;
    public AudioSource ReelStarts;
    public AudioSource Landing;
    public AudioSource ReesStop;
    public AudioSource Winning;
    public AudioSource Error;


    private void Awake()
    {
        Instance = this;
    }

    public void PlayButtonClick()
    {
        if (ButtonClick.isPlaying)
            ButtonClick.Stop();
        ButtonClick.Play();
    }

    public void PlayReelSpin()
    {
        if (ReelStarts.isPlaying)
            ReelStarts.Stop();
        ReelStarts.Play();
    }

    public void PlayReelStops()
    {
        if (ReesStop.isPlaying)
            ReesStop.Stop();
        ReesStop.Play();
    }

    public void PlayWinning()
    {
        if (Winning.isPlaying)
            Winning.Stop();
        Winning.Play();
    }

    public void PlayError()
    {
        if (Error.isPlaying)
            Error.Stop();
        Error.Play();
    }

    public void PlayLanding()
    {
        if (ReelStarts.isPlaying)
            ReelStarts.Stop();
        if (Landing.isPlaying)
            Landing.Stop();
        Landing.Play();
    }
}
