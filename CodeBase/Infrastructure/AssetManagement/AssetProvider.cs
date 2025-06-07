using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.CodeBase.Infrastructure.AssetManagement
{
    public class Assets : IAssets
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completedHandles =
            new Dictionary<string, AsyncOperationHandle>();

        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles =
            new Dictionary<string, List<AsyncOperationHandle>>();

        public void Initialize()
        {
            Addressables.InitializeAsync();
        }
        
        public async Task<GameObject> Instantiate(string address)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address);
            return await handle.Task;
        }
        public async Task<GameObject> Instantiate(string address, Vector3 position)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, position, Quaternion.identity);
            return await handle.Task;
        }

        
        public async Task<T> Load<T>(AssetReference prefabReference) where T : class
        {
            if (_completedHandles.TryGetValue(prefabReference.AssetGUID, out AsyncOperationHandle cache)) return cache.Result as T;
            
            return await RunWithCacheOnCompleted<T>(prefabReference.AssetGUID, Addressables.LoadAssetAsync<T>(prefabReference));
        }

        public async Task<T> Load<T>(string address) where T : class
        {
            if (_completedHandles.TryGetValue(address, out AsyncOperationHandle cache)) return cache.Result as T;
            
            return await RunWithCacheOnCompleted<T>(address, Addressables.LoadAssetAsync<T>(address));
        }

        public void CleanUp()
        {
            foreach (List<AsyncOperationHandle> handles in _handles.Values)
            {
                foreach (var handle in handles)
                {
                    Addressables.Release(handle);
                }
            }
            
            _handles.Clear();
            _completedHandles.Clear();
        }

        private async Task<T> RunWithCacheOnCompleted<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            handle.Completed += h => { _completedHandles[key] = h; };

            AddHandle(key, handle);
            return await handle.Task;
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourseHandles))
            {
                resourseHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourseHandles;
            }

            resourseHandles.Add(handle);
        }
    }
}