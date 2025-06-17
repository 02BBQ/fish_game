using UnityEngine;

namespace fishing.Network
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "Config/ServerConfig")]
    public class ServerConfig : ScriptableObject
    {
        [Header("Server Settings")]
        [SerializeField] private string baseUrl = "http://172.31.2.88:5926/api/";
        [SerializeField] private float timeoutSeconds = 10f;
        [SerializeField] private int maxRetries = 3;
        [SerializeField] private float retryDelaySeconds = 1f;

        [Header("Game Settings")]
        [SerializeField] private string defaultUserId = "test";

        public string BaseUrl => baseUrl;
        public float TimeoutSeconds => timeoutSeconds;
        public int MaxRetries => maxRetries;
        public float RetryDelaySeconds => retryDelaySeconds;
        public string DefaultUserId => defaultUserId;
    }
}