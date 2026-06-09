using AutoBattler.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattler.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {
        [SerializeField] private TileScriptableObjectScript _tile_SO;
        [SerializeField] private UnitDataScriptableObjectScript _unitData_SO;
        [SerializeField] private UnitPrefabScriptableObjectScript _unitPrefab_SO;
        [SerializeField] private RangedAbilitiesScriptableObjectScript _rangedAbilities_SO;
        [SerializeField] private BuffScriptableObjectScript _buff_SO;
        [SerializeField] private StageConfigScriptableObjectScript _stageConfig_SO;
        [SerializeField] private PlayerLevelConfigScriptableObjectScript _playerLevelConfig_SO;

        [SerializeField] private Transform _pooledUnitContainer;
        [SerializeField] private Transform _activeUnitContainer;

        public int currentStageIndexSelected = -1;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            RegisterServices();
            UIManager.Instance.Initialize();
        }

        private void OnDestroy()
        {
            DeregisterServices();
        }

        private void RegisterServices()
        {
            ServiceLocator.Register(new UnitDataService(_unitData_SO));
            ServiceLocator.Register(new UnitPoolService(_unitPrefab_SO, _pooledUnitContainer, _activeUnitContainer));
            ServiceLocator.Register(new TileGridService(_tile_SO));
            ServiceLocator.Register(new GraphService());
            ServiceLocator.Register(new TeamService());
            ServiceLocator.Register(new RangedAbilityService(_rangedAbilities_SO));
            ServiceLocator.Register(new CurrencyService());
            ServiceLocator.Register(new PlayerLevelService(_playerLevelConfig_SO));
            ServiceLocator.Register(new InventoryService());
            ServiceLocator.Register(new ShopService());
            ServiceLocator.Register(new BuffService(_buff_SO));
            ServiceLocator.Register(new RoundSnapshotService());
            ServiceLocator.Register(new StageSnapshotService());
            ServiceLocator.Register(new StageService(_stageConfig_SO));
        }

        private void DeregisterServices()
        {
            ServiceLocator.Unregister<UnitDataService>();
            ServiceLocator.Unregister<UnitPoolService>();
            ServiceLocator.Unregister<TileGridService>();
            ServiceLocator.Unregister<GraphService>();
            ServiceLocator.Unregister<TeamService>();
            ServiceLocator.Unregister<RangedAbilityService>();
            ServiceLocator.Unregister<StageService>();
            ServiceLocator.Unregister<CurrencyService>();
            ServiceLocator.Unregister<PlayerLevelService>();
            ServiceLocator.Unregister<InventoryService>();
            ServiceLocator.Unregister<ShopService>();
            ServiceLocator.Unregister<BuffService>();
            ServiceLocator.Unregister<RoundSnapshotService>();
            ServiceLocator.Unregister<StageSnapshotService>();
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}
