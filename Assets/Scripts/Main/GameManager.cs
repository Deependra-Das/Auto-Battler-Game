using AutoBattler.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattler.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {
        [SerializeField] private TileScriptableObjectScript _tile_SO;
        [SerializeField] private UnitScriptableObject _unit_SO;
        [SerializeField] private RangedAbilitiesScriptableObjectScript _rangedAbilities_SO;
        [SerializeField] private int _startingBalance = 10;

        protected override void Awake()
        {
            base.Awake();
            RegisterServices();
        }

        private void Start()
        {
            GameplayManager.Instance.Initialize(_unit_SO);
        }

        private void RegisterServices()
        {
            ServiceLocator.Register(new TileGridService(_tile_SO));
            ServiceLocator.Register(new GraphService());
            ServiceLocator.Register(new TeamService());
            ServiceLocator.Register(new RangedAbilityService(_rangedAbilities_SO));
            ServiceLocator.Register(new ShopService(_unit_SO));
            ServiceLocator.Register(new InventoryService());
            ServiceLocator.Register(new CurrencyService(_startingBalance));
        }

        private void DeregisterServices()
        {
            ServiceLocator.Unregister<TileGridService>();
            ServiceLocator.Unregister<GraphService>();
            ServiceLocator.Unregister<TeamService>();
            ServiceLocator.Unregister<RangedAbilityService>();
            ServiceLocator.Unregister<ShopService>();
            ServiceLocator.Unregister<InventoryService>();
            ServiceLocator.Unregister<CurrencyService>();
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}
