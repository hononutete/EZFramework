using UnityEngine.EventSystems;

namespace EZFramework
{

    public interface IEventReceiver : IEventSystemHandler
    {
        void OnRecieve(object[] parameters = null);
    }
}