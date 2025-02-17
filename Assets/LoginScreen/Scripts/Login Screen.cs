using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using UnityEngine.UI;
using Unity.Services.Core;
using System;
using System.Text;
using TMPro;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Authentication.PlayerAccounts;
public class LoginScreen : MonoBehaviour
{
    public TMP_Text status;
    public TMP_InputField inputField;
    public byte[] file;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignedIn;
    }

    public async void Start()
    {
        //await UnityServices.InitializeAsync();
        //SetupEvents();
        //await SignInAnonymouslyAsync();
    }

    public async void SavePlayerFile()
    {
        file = Encoding.UTF8.GetBytes(inputField.text);
        await CloudSaveService.Instance.Files.Player.SaveAsync("fakeFile", file);
    }

    public async void GetPlayerFileAsByteArray()
    {
        file = await CloudSaveService.Instance.Files.Player.LoadBytesAsync("fakeFile");
        status.text = Encoding.UTF8.GetString(file);
    }

    private async void SignedIn()
    {
        try
        {
            var accessToken = PlayerAccountService.Instance.AccessToken;
            await SignInWithUnityAsync(accessToken);

        }
        catch (global::System.Exception)
        {

            throw;
        }
    }

    public async Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }
}
