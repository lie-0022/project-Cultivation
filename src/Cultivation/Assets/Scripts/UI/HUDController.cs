using Cultivation.Data;
using Cultivation.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cultivation.UI
{
    /// <summary>
    /// 항상 표시되는 HUD. Gold / SeedInventory / CropInventory / InteractionPrompt.
    /// UIDocument는 picking-mode="Ignore"이므로 플레이어 입력을 차단하지 않는다.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        // 씨앗 고정 순서 (디자인 데이터와 일치)
        private static readonly string[] SeedIds = { "seed_carrot", "seed_cabbage", "seed_tomato" };
        // 작물 고정 순서
        private static readonly string[] CropIds = { "crop_carrot", "crop_cabbage", "crop_tomato" };

        private Label _goldLabel;
        private VisualElement _seedInventoryHUD;
        private VisualElement _cropInventoryHUD;
        private VisualElement _interactionPrompt;
        private Label _promptLabel;

        private GameManager _gm;
        private GameDataRegistry _registry;

        public void Initialize(GameManager gm)
        {
            _gm = gm;
            _registry = gm.DataRegistry;

            var root = _document.rootVisualElement;
            _goldLabel = root.Q<Label>("GoldLabel");
            _seedInventoryHUD = root.Q<VisualElement>("SeedInventoryHUD");
            _cropInventoryHUD = root.Q<VisualElement>("CropInventoryHUD");
            _interactionPrompt = root.Q<VisualElement>("InteractionPrompt");
            _promptLabel = root.Q<Label>("PromptLabel");

            HidePrompt();

            // 초기 값 표시
            RefreshGold(gm.Economy.Gold);
            RefreshSeeds();
            RefreshCrops();

            // 이벤트 구독
            gm.Economy.OnGoldChanged += RefreshGold;
            gm.Inventory.OnSeedChanged += OnSeedChanged;
            gm.Inventory.OnCropChanged += OnCropChanged;
        }

        private void OnDestroy()
        {
            if (_gm == null) return;
            _gm.Economy.OnGoldChanged -= RefreshGold;
            _gm.Inventory.OnSeedChanged -= OnSeedChanged;
            _gm.Inventory.OnCropChanged -= OnCropChanged;
        }

        private void RefreshGold(int gold)
        {
            if (_goldLabel != null) _goldLabel.text = gold.ToString();
        }

        private void OnSeedChanged(string _, int __) => RefreshSeeds();
        private void OnCropChanged(string _, int __) => RefreshCrops();

        private void RefreshSeeds()
        {
            if (_seedInventoryHUD == null) return;
            // UXML hud-row 순서: carrot(0) / cabbage(1) / tomato(2) — SeedIds 배열과 일치
            var rows = _seedInventoryHUD.Query<VisualElement>(className: "hud-row").ToList();
            for (int i = 0; i < rows.Count && i < SeedIds.Length; i++)
            {
                int count = _gm.Inventory.GetSeedCount(SeedIds[i]);
                var lbl = rows[i].Q<Label>(className: "hud-item-count");
                if (lbl == null) continue;
                lbl.text = $"×{count}";
                lbl.EnableInClassList("hud-item-count--zero", count == 0);
            }
        }

        private void RefreshCrops()
        {
            if (_cropInventoryHUD == null) return;
            // UXML hud-row 순서: carrot(0) / cabbage(1) — CropIds 배열과 일치
            var rows = _cropInventoryHUD.Query<VisualElement>(className: "hud-row").ToList();
            for (int i = 0; i < rows.Count && i < CropIds.Length; i++)
            {
                int count = _gm.Inventory.GetCropCount(CropIds[i]);
                var lbl = rows[i].Q<Label>(className: "hud-item-count");
                if (lbl == null) continue;
                lbl.text = $"×{count}";
                lbl.EnableInClassList("hud-item-count--zero", count == 0);
            }
        }

        public void ShowPrompt(string text)
        {
            if (_interactionPrompt == null) return;
            if (_promptLabel != null) _promptLabel.text = text;
            _interactionPrompt.style.display = DisplayStyle.Flex;
        }

        public void HidePrompt()
        {
            if (_interactionPrompt == null) return;
            _interactionPrompt.style.display = DisplayStyle.None;
        }
    }
}
