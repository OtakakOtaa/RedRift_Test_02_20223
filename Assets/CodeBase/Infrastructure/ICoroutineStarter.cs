using System.Collections;

namespace CodeBase.Infrastructure
{
    public interface ICoroutineStarter
    {
        void Start(IEnumerator coroutine);
    }
}