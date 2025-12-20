using AutoBattler.Utilities;
using UnityEngine;

namespace AutoBattler.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {

        public const string UsernameKey = "Username";

        private void Start()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
        }

        private void DeregisterServices()
        {
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}
