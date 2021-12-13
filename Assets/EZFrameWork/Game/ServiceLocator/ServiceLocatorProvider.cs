using EZFramework.Util;

namespace EZFramework.Game
{
    public class ServiceLocatorProvider : SingletonMonobehaviour<ServiceLocatorProvider>
    {
        public ServiceLocator Current { get; private set; } = new ServiceLocator();

    }
}
