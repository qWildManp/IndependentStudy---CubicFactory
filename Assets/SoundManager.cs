using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioClip btnClickSFX;
    public AudioClip boxPushSFX;
    public AudioClip rewardSFX;

    public AudioSource SFXSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBtnClick()
    {
        SFXSource.clip = btnClickSFX;
        SFXSource.Play();
    }

    public void PlayBoxPush()
    {
        SFXSource.clip = boxPushSFX;
        SFXSource.Play();
    }

    public void PlayCollectReward()
    {
        SFXSource.clip = rewardSFX;
        SFXSource.Play();
    }
}
