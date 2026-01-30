
using UnityEngine;
using System.Collections;

public class ShareHandles : MonoBehaviour
{
    [SerializeField] string YoutubeURL, FacebookUrl, InstaUrl;
    [SerializeField] GameObject MultiplayerPanel;
    float multiplayerPanelTime=2f;
    public void OpenFacebook()
    {
        SoundManager.soundManager.PlayButtonSound();
        Application.OpenURL(FacebookUrl);
    }
    public void OpenInsta()
    {
        SoundManager.soundManager.PlayButtonSound();

        Application.OpenURL(InstaUrl);
    }

    public void OpenYoutube()
    {
        SoundManager.soundManager.PlayButtonSound();

        Application.OpenURL(YoutubeURL);
    }

    public void OpenWebsite()
    {
        SoundManager.soundManager.PlayButtonSound();
        Application.OpenURL("https://droxstudio.com/");
    }
    public void MultiPlayerButtonClicked()
    {
        StartCoroutine(MultiplayerSequence());
    }

    IEnumerator MultiplayerSequence()
    {
        SoundManager.soundManager.PlayButtonSound();
        MultiplayerPanel.SetActive(true);
        yield return new WaitForSeconds(multiplayerPanelTime);
        MultiplayerPanel.SetActive(false);
    }
    
}
