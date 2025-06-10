using UnityEngine;

namespace fishing.Network
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "Config/ServerConfig")]
    public class ServerConfig : ScriptableObject
    {
        public string BaseUrl = "http://172.31.2.88:5926/api/";
        public float TimeoutSeconds = 10f;
        public int MaxRetries = 3;
        public float RetryDelaySeconds = 1f;
    }
} 