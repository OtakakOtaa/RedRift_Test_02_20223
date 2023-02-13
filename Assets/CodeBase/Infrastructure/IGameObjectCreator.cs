using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IGameObjectCreator
    {
        GameObject CreateByPrefab(GameObject prefab);
    }
}