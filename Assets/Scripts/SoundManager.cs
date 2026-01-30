using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager soundManager;
    AudioSource _audio;
    AudioSource enemyBombaudio;
    [SerializeField] AudioClip BigExplosion, BombExplosion,enemydeath,PlayerDeath,PlayerHurt,EnemyHurt,ThrowSound,LevelClear,LevelFailed,ButtonSound, tickSound,rewardSound;
    void Awake()
    {
        if (soundManager == null)
            soundManager = this;

        _audio = GetComponent<AudioSource>();
        enemyBombaudio = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        CheckMusic();
    }

    public void PlayEnemyBombSound()
    {
        if (!enemyBombaudio.isPlaying)
        {
            enemyBombaudio.clip = BombExplosion;
            enemyBombaudio.Play();
        }
    }

    public void EnemyDiedSound()
    {
        _audio.PlayOneShot(enemydeath);
    }


    public void PlayClip(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    public void PlayBigExplosionSound()
    {
        PlayClip(BigExplosion);
    }

    public void PlayPlayerHurt()
    {
        _audio.PlayOneShot(PlayerHurt);
    }

    public void PlayPlayerDeath()
    {
        _audio.PlayOneShot(PlayerDeath);
    }

  
    public void PlayEnemyHurtSound()
    {
        _audio.PlayOneShot(EnemyHurt);
    }

    public void PlayBombSound()
    {
        _audio.PlayOneShot(BombExplosion);
    }

    public void PlayThrowSound()
    {
        _audio.PlayOneShot(ThrowSound);
    }

    public void PlayLevelClear()
    {
        _audio.PlayOneShot(LevelClear);
    }
    public void PlayLevelFailed()
    {
        _audio.PlayOneShot(LevelFailed);
    }

    public void PlayButtonSound()
    {
        _audio.PlayOneShot(ButtonSound);
    }

    public void PlayTickSound()
    {
        _audio.PlayOneShot(tickSound);
    }
    public void PlayRewardSound()
    {
        _audio.PlayOneShot(rewardSound);
    }

    public void CheckMusic()
    {
       if( PlayerPrefs.GetInt("SoundOff") == 1)
        {
            _audio.volume = 0;
            enemyBombaudio.volume = 0;
        }
        else
        {
            _audio.volume = 1;
            enemyBombaudio.volume = 1;
        }
    }
}
