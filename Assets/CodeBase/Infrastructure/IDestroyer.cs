using UnityEngine;

namespace CodeBase.Infrastructure
{
    public interface IDestroyer
    {
        void DestroyObject(GameObject target);
    }
}