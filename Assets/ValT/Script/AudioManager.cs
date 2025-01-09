using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource soundEffectSource;
    public AudioSource musicSource;
    public AudioClip intro;
    public AudioClip levelMusic;

    //Ajoutez votre clip audio ici
    //Pour le jouer, utilisez AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.nomDuClip);
    public AudioClip codingSound2;
    public AudioClip codingSound1;
    public AudioClip electricSound;
    public AudioClip jumpSound;
    public AudioClip wallJumpSound;
    public AudioClip gunSound;
    public AudioClip collectibleSound;
    public AudioClip codeBreaking;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footste3;
    public AudioClip firstExplosion;
    public AudioClip secondExplosion;

    private void Start()
    {
        PlayMusic(2);
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectSource.PlayOneShot(clip);
    }

    public void PlayMusic(int musicIndex)
    {
        AudioClip selectedMusic = null;

        switch (musicIndex)
        {
            case 1:
                selectedMusic = intro;
                break;
            case 2:
                selectedMusic = levelMusic;
                break;
        }

        if (selectedMusic != null && musicSource.clip != selectedMusic)
        {
            musicSource.clip = selectedMusic;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}