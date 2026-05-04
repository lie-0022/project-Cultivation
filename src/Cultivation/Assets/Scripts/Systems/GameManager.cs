using System;
using Cultivation.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cultivation.Systems
{
    /// <summary>
    /// 게임 전체 진입점. 모든 매니저의 인스턴스를 보유하고, Tick이 필요한 매니저(Farm, Breeding)를 Update에서 호출한다.
    /// 추가로 UI 모드 전환을 관리하여 플레이어/카메라/상호작용 입력을 차단한다.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("초기 게임 상태")]
        [SerializeField] private int _startingGold = 200;
        [SerializeField] private int _startingFarmPlots = 2;
        [SerializeField] private int _startingBarnSlots = 2;

        [Header("교배 설정")]
        [SerializeField] private float _selfCloneTime = 30f;

        [Header("Configs")]
        [SerializeField] private GachaConfig _gachaConfig;
        [SerializeField] private ExpansionConfig _expansionConfig;
        [SerializeField] private GameDataRegistry _dataRegistry;

        public InventoryManager Inventory { get; private set; }
        public EconomyManager Economy { get; private set; }
        public GachaManager Gacha { get; private set; }
        public BarnManager Barn { get; private set; }
        public CreatureManager Creature { get; private set; }
        public FarmManager Farm { get; private set; }
        public BreedingManager Breeding { get; private set; }

        public GachaConfig GachaConfig => _gachaConfig;
        public ExpansionConfig ExpansionConfig => _expansionConfig;
        public GameDataRegistry DataRegistry => _dataRegistry;

        public bool IsUIModeActive { get; private set; }
        public event Action<bool> OnUIModeChanged;

        private int _uiActivatedFrame = -1;

        private void Awake()
        {
            if (_gachaConfig == null) Debug.LogError("[GameManager] GachaConfig가 할당되지 않았습니다.");
            if (_expansionConfig == null) Debug.LogError("[GameManager] ExpansionConfig가 할당되지 않았습니다.");
            if (_dataRegistry == null) Debug.LogError("[GameManager] GameDataRegistry가 할당되지 않았습니다.");

            Inventory = new InventoryManager();
            Economy = new EconomyManager(_startingGold);
            Gacha = new GachaManager(_gachaConfig, Economy, Inventory);
            Barn = new BarnManager(Economy, _expansionConfig, _startingBarnSlots);
            Creature = new CreatureManager(Inventory, Barn, _dataRegistry);
            Farm = new FarmManager(Inventory, Economy, _expansionConfig, _dataRegistry, _startingFarmPlots);
            Breeding = new BreedingManager(Barn, _dataRegistry, _selfCloneTime);
        }

        private void Start()
        {
            ApplyCursor(false);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            Farm?.Tick(dt);
            Breeding?.Tick(dt);

            var kb = Keyboard.current;
            // UI를 연 바로 그 프레임의 E키 입력은 즉시 닫힘으로 처리되지 않도록 다음 프레임부터만 토글 허용.
            if (kb != null && IsUIModeActive && Time.frameCount > _uiActivatedFrame)
            {
                // ESC 또는 E키로 UI 닫기. 닫히면 ApplyCursor(false)에서 자동 잠금.
                if (kb.escapeKey.wasPressedThisFrame || kb.eKey.wasPressedThisFrame)
                {
                    SetUIModeActive(false);
                }
            }
        }

        /// <summary>UI 모드 활성/해제. 활성 시 커서 잠금 해제 + 표시, 해제 시 커서 잠금 + 숨김.</summary>
        public void SetUIModeActive(bool active)
        {
            if (IsUIModeActive == active) return;
            IsUIModeActive = active;
            if (active) _uiActivatedFrame = Time.frameCount;
            ApplyCursor(active);
            OnUIModeChanged?.Invoke(active);
        }

        private static void ApplyCursor(bool uiActive)
        {
            Cursor.lockState = uiActive ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = uiActive;
        }
    }
}
