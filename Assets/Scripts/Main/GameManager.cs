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
        [SerializeField] private BuffScriptableObjectScript _buff_SO;
        [SerializeField] private StageConfigScriptableObjectScript _stageConfig_SO;
        [SerializeField] private PlayerLevelConfigScriptableObjectScript _playerLevelConfig_SO;

        protected override void Awake()
        {
            base.Awake();
            RegisterServices();
        }

        private void Start()
        {
            GameplayManager.Instance.Initialize(_unit_SO);
        }

        private void OnDestroy()
        {
            DeregisterServices();
        }

        private void RegisterServices()
        {
            ServiceLocator.Register(new TileGridService(_tile_SO));
            ServiceLocator.Register(new GraphService());
            ServiceLocator.Register(new TeamService());
            ServiceLocator.Register(new RangedAbilityService(_rangedAbilities_SO));
            ServiceLocator.Register(new StageService(_stageConfig_SO));
            ServiceLocator.Register(new CurrencyService());
            ServiceLocator.Register(new PlayerLevelService(_playerLevelConfig_SO));
            ServiceLocator.Register(new InventoryService());
            ServiceLocator.Register(new ShopService(_unit_SO));
            ServiceLocator.Register(new BuffService(_buff_SO));
        }

        private void DeregisterServices()
        {
            ServiceLocator.Unregister<TileGridService>();
            ServiceLocator.Unregister<GraphService>();
            ServiceLocator.Unregister<TeamService>();
            ServiceLocator.Unregister<RangedAbilityService>();
            ServiceLocator.Unregister<ShopService>();
            ServiceLocator.Unregister<InventoryService>();
            ServiceLocator.Unregister<PlayerLevelService>();
            ServiceLocator.Unregister<CurrencyService>();
            ServiceLocator.Unregister<BuffService>();
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}
