using System.Threading.Tasks;

namespace fishing.Network
{
    public interface IFishingServerService
    {
        Task<Result<StartFishingResponse>> StartFishing();
        Task<Result<FishJson>> EndFishing(string guid, bool success);
        Task<Result<InitData>> GetData(string userId);
        Task<Result<InitData>> AuthenticateSteamUser(string steamId, string authTicket);
    }

    [System.Serializable]
    public class StartFishingResponse
    {
        public string guid;
        public float time;
        public float dancingStep;
    }
} 