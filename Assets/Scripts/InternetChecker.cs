
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InternetChecker : MonoBehaviour
{
    public float timeoutDuration = 5f; // timeout for the check in seconds
    [SerializeField] GameObject NoInternetParentPanel;
    [SerializeField] GameObject[] Checking, NoAcess;
    [SerializeField] GameObject TryAgainButton;

    private void Start()
    {
        gameObject.SetActive(false);
        //CheckInternetConnection();
    }

    public void CheckInternetConnection()
    {
        StartCoroutine(CheckInternetAccess());
    }

    private IEnumerator CheckInternetAccess()
    {
        TryAgainButton.SetActive(false);

        for(int i = 0; i < Checking.Length; i++)
        {
            Checking[i].SetActive(true);
        }
        for(int i = 0; i < NoAcess.Length; i++)
        {
            NoAcess[i].SetActive(false);
        }

        UnityWebRequest request = new UnityWebRequest("https://www.google.com");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            for (int i = 0; i < Checking.Length; i++)
            {
                Checking[i].SetActive(false);
            }
            for (int i = 0; i < NoAcess.Length; i++)
            {
                NoAcess[i].SetActive(true);
            }
            TryAgainButton.SetActive(true);
        }
        else
        {
            NoInternetParentPanel.SetActive(false);
        }
    }

    
    public void TryAgain()
    {
        SoundManager.soundManager.PlayButtonSound();
        CheckInternetConnection();
    }
}
