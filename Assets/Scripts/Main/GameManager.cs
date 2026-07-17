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
        [SerializeField] private VfxScriptableObjectScript _vfx_SO;
        [SerializeField] private UnitColorScriptableObjectScript _unitColor_SO;
        [SerializeField] private IconScriptableObjectScript _unitIcon_SO;
        [SerializeField] private VideoInstructionScriptableObjectScript _videoInstruction_SO;
        [SerializeField] private AudioScriptableObjectScript _audio_SO;

        [SerializeField] private Transform _pooledUnitContainerTransform;
        [SerializeField] private Transform _activeUnitContainerTransform;
        [SerializeField] private Transform _pooledVfxContainerTransform;
        [SerializeField] private Transform _pooledRangedAbilityContainerTransform;

        public int currentStageIndexSelected = -1;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            RegisterServices();
            AudioManager.Instance.Initialize(_audio_SO);
            UIManager.Instance.Initialize();
        }

        private void OnDestroy()
        {
            if (!IsSingletonInstance())
                return;

            DeregisterServices();
        }

        private void RegisterServices()
        {
            ServiceLocator.Register(new VideoInstructionService(_videoInstruction_SO));
            ServiceLocator.Register(new UnitDataService(_unitData_SO));
            ServiceLocator.Register(new IconService(_unitIcon_SO));
            ServiceLocator.Register(new UnitPoolService(_unitPrefab_SO, _pooledUnitContainerTransform, _activeUnitContainerTransform));
            ServiceLocator.Register(new DragVisualPoolService());
            ServiceLocator.Register(new VfxPoolService(_vfx_SO, _pooledVfxContainerTransform));
            ServiceLocator.Register(new UnitColorService(_unitColor_SO));
            ServiceLocator.Register(new RangedAbilityPoolService(_rangedAbilities_SO, _pooledRangedAbilityContainerTransform));
            ServiceLocator.Register(new TileGridService());
            ServiceLocator.Register(new GraphService());
            ServiceLocator.Register(new HighlightTileService(_tile_SO));
            ServiceLocator.Register(new TeamService());
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
            ServiceLocator.Unregister<VideoInstructionService>();
            ServiceLocator.Unregister<UnitDataService>();
            ServiceLocator.Unregister<IconService>();
            ServiceLocator.Unregister<UnitPoolService>();
            ServiceLocator.Unregister<DragVisualPoolService>();
            ServiceLocator.Unregister<VfxPoolService>();
            ServiceLocator.Unregister<UnitColorService>();
            ServiceLocator.Unregister<RangedAbilityPoolService>();
            ServiceLocator.Unregister<TileGridService>();
            ServiceLocator.Unregister<GraphService>();
            ServiceLocator.Unregister<HighlightTileService>();
            ServiceLocator.Unregister<TeamService>();
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
