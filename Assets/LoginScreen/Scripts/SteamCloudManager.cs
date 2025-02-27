using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class SteamCloudManager : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private TMP_Text userIdText;
    [SerializeField] private TMP_Text userNameText;
    // Reference to your ScriptableObject holding player data.
    public PlayerDataSO playerData;
    // Name of the file to use on Steam Cloud.
    public string fileName = "playerdata.json";

    void Start()
    {
        // Initialize Steam.
        if (!SteamAPI.Init())
        {
            Debug.LogError("SteamAPI_Init() failed. Ensure Steam is running and your AppID is correct.");
            return;
        }
        else
        {
            Debug.Log("SteamAPI successfully initialized.");
        }
    }

    /// <summary>
    /// Serializes and saves the player data to Steam Cloud.
    /// Call this method from your TMP button for saving.
    /// </summary>
    public void SavePlayerData()
    {
        // Convert the ScriptableObject data to JSON.
        string json = JsonUtility.ToJson(playerData);
        byte[] bytes = Encoding.UTF8.GetBytes(json);

        bool success = SteamRemoteStorage.FileWrite(fileName, bytes, bytes.Length);
        if (success)
        {
            Debug.Log("Player data saved to Steam Cloud.");
        }
        else
        {
            Debug.LogError("Failed to save player data to Steam Cloud.");
        }
    }

    /// <summary>
    /// Loads player data from Steam Cloud and updates the ScriptableObject.
    /// Call this method from your TMP button for loading.
    /// </summary>
    public void LoadPlayerData()
    {
        int fileSize = SteamRemoteStorage.GetFileSize(fileName);
        if (fileSize <= 0)
        {
            Debug.LogError("File not found or empty: " + fileName);
            return;
        }

        byte[] bytes = new byte[fileSize];
        int bytesRead = SteamRemoteStorage.FileRead(fileName, bytes, fileSize);
        if (bytesRead != fileSize)
        {
            Debug.LogError("Failed to read complete player data from Steam Cloud.");
            return;
        }

        string json = Encoding.UTF8.GetString(bytes);
        // Overwrite the ScriptableObject with the loaded JSON data.
        JsonUtility.FromJsonOverwrite(json, playerData);
        Debug.Log("Player data loaded from Steam Cloud.");
    }



    void Update()
    {
        // Process any pending Steam callbacks.
        SteamAPI.RunCallbacks();
    }

    void OnDestroy()
    {
        // Shutdown SteamAPI on object destruction.
        SteamAPI.Shutdown();
    }
}
