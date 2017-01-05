using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public AudioSource backgroundAudioSource;
    public AudioSource soundAudioSource;

    [HideInInspector]
    public bool isMusicPlaying = true;

    // Use this for initialization
    void Start () {
        InitAudioSourceRefs();        

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    void InitAudioSourceRefs() {
        AudioSource[] listeners = GetComponents<AudioSource>();
        backgroundAudioSource = listeners[0];
        soundAudioSource = listeners[1];
    }



    public void StopBackgroundMusic() {
        backgroundAudioSource.Stop();
        isMusicPlaying = false;
    }
    public void PlayBackgroundMusic() {
        backgroundAudioSource.Play();
        isMusicPlaying = true;
    }



}
