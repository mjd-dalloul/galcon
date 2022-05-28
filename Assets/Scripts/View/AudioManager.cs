using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public AudioSource clickSource;

    // Start is called before the first frame update
    void Awake()
    {
        clickSource.volume = PlayerPrefs.GetFloat("volume", 1.0f);
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
   

    public void PlayClickSound()
    {
        if(!clickSource.isPlaying)
        clickSource.Play();
    }

    public void setVolume(float vol)
    {
        clickSource.volume = vol;
    }

    public void saveVolume(float vol)
    {
        PlayerPrefs.SetFloat("volume", vol);
    }

    public float getVolume()
    {
        return clickSource.volume;
    }
}
