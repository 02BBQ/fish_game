using UnityEngine;

namespace fishing.Network
{
    public static class ServerConfig
    {
        public static string BaseUrl { get; private set; } = "http://172.31.2.88:5926/api/";
        public static float TimeoutSeconds { get; private set; } = 10f;
        public static int MaxRetries { get; private set; } = 3;
        public static float RetryDelaySeconds { get; private set; } = 1f;

        public static void Initialize(string baseUrl = null, float? timeoutSeconds = null, int? maxRetries = null, float? retryDelaySeconds = null)
        {
            if (baseUrl != null) BaseUrl = baseUrl;
            if (timeoutSeconds.HasValue) TimeoutSeconds = timeoutSeconds.Value;
            if (maxRetries.HasValue) MaxRetries = maxRetries.Value;
            if (retryDelaySeconds.HasValue) RetryDelaySeconds = retryDelaySeconds.Value;
        }
    }
}