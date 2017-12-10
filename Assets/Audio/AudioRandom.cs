using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandom : MonoBehaviour {

    public AudioClip[] audioClips;  
    public bool cycle = false;      // cycle thru audio clips or play at random 

    private AudioSource source;
    private int currentIndex = 0;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play()
    {
        if (cycle)
        {
            if (++currentIndex >= audioClips.Length) currentIndex = 0;
            source.clip = audioClips[currentIndex];
        }
        else
        {
            source.clip = audioClips[Random.Range(0, audioClips.Length)];
        }

        source.Play();
    }
}
