using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSound : MonoBehaviour {

    //public SpeechTest ST;
    private AudioSource audio;
    private AudioClip clip;
	void Start () {
        //ST = GameObject.Find("Surface").GetComponent<SpeechTest>();
        //audio = FindObjectOfType<AudioSource>();
        //audio.loop = false;
        //audio.playOnAwake = false;
        //clip = Resources.Load<AudioClip>("Audios/Correct/s07_dropsound.mp3");
    }

    void OnCollisionEnter()
    {
        //if (!audio.isPlaying)
        //{
        //audio.clip = clip;// ST.clips[7]; 
        //    audio.Play();
        //}
    }
}
