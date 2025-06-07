using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Services.GetUserInfoService
{
    public interface IUserInfoService : IService
    {
        public DeviceType GetDeviceType();
        public string GetLanguage();
        string GetUserID();
    }

    public class MockUserInfoService : IUserInfoService
    {
        public DeviceType GetDeviceType()
        {
            if (Application.isMobilePlatform)
                return DeviceType.Handheld;
            return DeviceType.Desktop;
        }

        public string GetLanguage()
        {
            if (Application.systemLanguage.ToString() == "ru")
            {
                return "Russian";
            }
            
            return "English";
        }

        public string GetUserID()
        {
            return "111-111-111-111-111"; // Mock user ID for testing purposes
        }
    }
}