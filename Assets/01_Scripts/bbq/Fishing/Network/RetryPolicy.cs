using System;
using System.Threading.Tasks;

namespace fishing.Network
{
    public interface IRetryPolicy
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }

    public class ExponentialBackoffRetryPolicy : IRetryPolicy
    {
        private readonly int _maxRetries;
        private readonly float _initialDelay;
        
        public ExponentialBackoffRetryPolicy(int maxRetries, float initialDelay)
        {
            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
        }
        
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var delay = _initialDelay;
            for (int i = 0; i < _maxRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch (Exception) when (i < _maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delay));
                    delay *= 2; // 지수 백오프
                }
            }
            return await action(); // 마지막 시도
        }
    }
} 