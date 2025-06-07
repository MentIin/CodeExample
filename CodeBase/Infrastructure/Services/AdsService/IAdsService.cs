using System;

namespace Assets.CodeBase.Infrastructure.Services.AdsService
{
    public interface IAdsService : IService
    {
        public Action<int> RewardedAdEvent { get; set; }
        public bool RewardedAvailable { get; }
        public bool InitialAvailable { get; }

        public void ShowInitial();

        public void ShowRewarded(int id);

    }

    public class MockAdsService : IAdsService
    {
        public Action<int> RewardedAdEvent { get; set; }
        public bool RewardedAvailable { get; }
        public bool InitialAvailable { get; }
        public void ShowInitial()
        {
            
        }

        public void ShowRewarded(int id)
        {
            
        }
    }
}