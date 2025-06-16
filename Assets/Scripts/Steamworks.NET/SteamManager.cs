using System.Collections;
using Steamworks;
using UnityEngine;

public class SteamManager : MonoBehaviour {

    public static SteamManager instance;
    private uint appID = 3745050;
    [HideInInspector] public bool connectedToSteam = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        try
        {
            Steamworks.SteamClient.Init(appID);
            connectedToSteam = true;
        }
        catch(System.Exception exception)
        {
            connectedToSteam = false;
        }
    }
    private void Update()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamClient.RunCallbacks();
        }
    }
    public void DisconnectFromSteam()
    {
        if (connectedToSteam)
        {
            Steamworks.SteamClient.Shutdown();
        }
    }
    private void OnApplicationQuit()
    {
        DisconnectFromSteam();
    }
}
