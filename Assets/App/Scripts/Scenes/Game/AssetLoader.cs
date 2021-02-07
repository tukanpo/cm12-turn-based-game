using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.Scenes.Game
{
    public static class AssetLoader
    {
        public static IEnumerator LoadPlayerUnitPrefab(Action<Unit> callback)
        {
            yield return LoadGameObject("PlayerUnit", callback);
        }

        public static IEnumerator LoadEnemyUnitPrefab(Action<Unit> callback)
        {
            yield return LoadGameObject("EnemyUnit", callback);
        }

        public static IEnumerator LoadFloorTilePrefab(Action<Tile> callback)
        {
            yield return LoadGameObject("FloorTile", callback);
        }

        public static IEnumerator LoadWallPrefab(Action<Unit> callback)
        {
            yield return LoadGameObject("Wall", callback);
        }
        
        static IEnumerator LoadGameObject<TComponent>(string address, Action<TComponent> callback)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            yield return new WaitUntil(() => handle.IsDone);
            
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError(handle.Status);
                yield break;
            }

            callback(handle.Result.GetComponent<TComponent>());
        }

        static IEnumerator LoadAsset<T>(string address, Action<T> callback)
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            yield return new WaitUntil(() => handle.IsDone);
            
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError(handle.Status);
                yield break;
            }

            callback(handle.Result);
        }
    }
}
