
using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public static BGMusic instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        CheckMusic();
    }

    

    public void CheckMusic()
    {
        if (PlayerPrefs.GetInt("BgMusicOff") == 1)
        {
            GetComponent<AudioSource>().volume = 0;
        }
        else
        {
            GetComponent<AudioSource>().volume = 1;
        }
    }


}
