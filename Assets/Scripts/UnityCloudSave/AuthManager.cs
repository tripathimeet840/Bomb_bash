using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public TextMeshProUGUI logTxt;

    private async void Start()
    {
        // Always initialize services first
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Services Initialized");
        logTxt.text = "Player Id: " + AuthenticationService.Instance.PlayerId;

        SignIn();
    }

    public async void SignIn()
    {
        await SignInAnonymous();
    }

    public void ShowPlayerID()
    {
        logTxt.text = "Player Id: " + AuthenticationService.Instance.PlayerId;
    }

    private async Task SignInAnonymous()
    {
        try
        {
            // Check if already signed in
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign-in success");
                Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
                logTxt.text = "Player Id: " + AuthenticationService.Instance.PlayerId;
            }
            else
            {
                Debug.Log("Already signed in as: " + AuthenticationService.Instance.PlayerId);
            }
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError("Sign-in Failed!");
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError("Request Failed!");
            Debug.LogException(ex);
        }
    }
}
