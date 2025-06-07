using System.Threading.Tasks;
using Assets.CodeBase.Infrastructure.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.CodeBase.Infrastructure.AssetManagement
{
    public interface IAssets : IService
    {
        Task<T> Load<T>(AssetReference prefabReference) where T : class;
        void CleanUp();
        Task<T> Load<T>(string address) where T : class;
        void Initialize();
        Task<GameObject> Instantiate(string address, Vector3 position);
        Task<GameObject> Instantiate(string address);
    }
}