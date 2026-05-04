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
            if (_seedInventoryHUD == null || _registry == null) return;

            // 씨앗 행을 이름으로 찾아 카운트 레이블만 갱신
            foreach (var seedId in SeedIds)
            {
                var seed = _registry.FindSeed(seedId);
                if (seed == null) continue;
                int count = _gm.Inventory.GetSeedCount(seedId);
                // UXML 행 이름 없으므로 씨앗명 Label로 탐색
                UpdateInventoryRow(_seedInventoryHUD, seed.SeedName, count);
            }
        }

        private void RefreshCrops()
        {
            if (_cropInventoryHUD == null || _registry == null) return;

            foreach (var cropId in CropIds)
            {
                var crop = _registry.FindCrop(cropId);
                if (crop == null) continue;
                int count = _gm.Inventory.GetCropCount(cropId);
                UpdateInventoryRow(_cropInventoryHUD, crop.CropName, count);
            }
        }

        /// <summary>itemName이 포함된 행에서 "×N" 패턴 Label을 찾아 갱신.</summary>
        private static void UpdateInventoryRow(VisualElement parent, string itemName, int count)
        {
            foreach (var row in parent.Children())
            {
                Label nameLabel = null;
                Label countLabel = null;
                foreach (var child in row.Children())
                {
                    if (child is Label lbl)
                    {
                        if (lbl.text == itemName) nameLabel = lbl;
                        else if (lbl.text.StartsWith("×")) countLabel = lbl;
                    }
                }
                if (nameLabel != null && countLabel != null)
                {
                    countLabel.text = $"×{count}";
                    countLabel.EnableInClassList("hud-item-count--zero", count == 0);
                    return;
                }
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
