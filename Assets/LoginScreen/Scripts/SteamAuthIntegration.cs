using System.Collections;
using UnityEngine;
using Steamworks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using TMPro;
using UnityEngine.UI;

public class SteamAuthIntegration : MonoBehaviour
{
    Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebApiResponseCallback;
    string m_SessionTicket;
    string identity = "unityauthenticationservice";

    IEnumerator Start()
    {
        Debug.Log("SteamAuthIntegration Start called.");
        while (!SteamManager.Initialized)
        {
            Debug.Log("Waiting for SteamManager to initialize...");
            yield return null;
        }
        Debug.Log("Steam is initialized via SteamManager.");

        // Call to sign in
        SignInWithSteam();
    }

    void Update()
    {
        // Though SteamManager already runs callbacks, ensure we’re still processing Steam callbacks here.
        if (SteamManager.Initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }

    void SignInWithSteam()
    {
        Debug.Log("Attempting to sign in with Steam...");
        m_AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);
        // Request a session ticket from Steam
        SteamUser.GetAuthTicketForWebApi(identity);
        Debug.Log("Auth ticket requested from Steam.");
    }

    void OnAuthCallback(GetTicketForWebApiResponse_t callback)
    {
        Debug.Log("Received Steam auth callback.");
        m_SessionTicket = System.BitConverter.ToString(callback.m_rgubTicket).Replace("-", string.Empty);
        //m_SessionTicket = Convert.ToBase64String(callback.m_rgubTicket).Replace("-", string.Empty);
        Debug.Log("Steam login success. Session Ticket: " + m_SessionTicket);
        m_AuthTicketForWebApiResponseCallback.Dispose();
        m_AuthTicketForWebApiResponseCallback = null;
        // Proceed to Unity Authentication
        SignInToUnityAuth(m_SessionTicket);
    }

    async void SignInToUnityAuth(string ticket)
    {
        Debug.Log("Attempting Unity Services initialization for Steam sign in...");
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("Unity Services initialized.");
            await AuthenticationService.Instance.SignInWithSteamAsync(ticket, identity);
            Debug.Log("Signed in to Unity Authentication with Steam ticket.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to sign in to Unity Authentication: " + e);
        }
    }
}
