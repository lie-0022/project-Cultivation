using Cultivation.Data;
using Cultivation.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cultivation.UI
{
    /// <summary>
    /// 씨앗 가챠 패널. DrawBtn 클릭 → TryPull → ResultCard 갱신.
    /// </summary>
    public class GachaPanelController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        private Button _drawBtn;
        private Button _closeBtn;
        private VisualElement _resultCard;
        private Label _rarityTag;
        private Label _seedNameLabel;
        private Label _seedDescLabel;
        private VisualElement _seedArt;

        private GameManager _gm;

        public void Initialize(GameManager gm)
        {
            _gm = gm;

            var root = _document.rootVisualElement;
            _drawBtn = root.Q<Button>("DrawBtn");
            _closeBtn = root.Q<Button>("CloseBtn");
            _resultCard = root.Q<VisualElement>("ResultCard");

            if (_resultCard != null)
            {
                _rarityTag = _resultCard.Q<Label>(className: "seed-rarity-tag");
                _seedNameLabel = _resultCard.Q<Label>(className: "seed-name");
                _seedDescLabel = _resultCard.Q<Label>(className: "seed-desc");
                // seed-art 컨테이너 내부의 pic-big 요소를 직접 참조해야 클래스 교체가 작동함
                _seedArt = _resultCard.Q<VisualElement>(className: "seed-art")?.Q<VisualElement>(className: "pic-big");
            }

            _drawBtn?.RegisterCallback<ClickEvent>(_ => OnDraw());
            _closeBtn?.RegisterCallback<ClickEvent>(_ => CloseFromPanel());

            // 초기 상태: 결과 카드 숨김
            if (_resultCard != null) _resultCard.style.display = DisplayStyle.None;
        }

        public void Open()
        {
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            RefreshDrawButton();
            // 결과 카드는 열릴 때 숨김
            if (_resultCard != null) _resultCard.style.display = DisplayStyle.None;
        }

        public void Close()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void CloseFromPanel()
        {
            _gm?.UI.CloseAll();
            _gm?.SetUIModeActive(false);
        }

        private void OnDraw()
        {
            if (_gm == null) return;

            bool success = _gm.Gacha.TryPull(out SeedData result);
            if (!success || result == null) return;

            ShowResult(result);
            RefreshDrawButton();
        }

        private void ShowResult(SeedData seed)
        {
            if (_resultCard == null) return;

            _resultCard.style.display = DisplayStyle.Flex;

            // 등급 클래스 교체
            _resultCard.RemoveFromClassList("seed-card--common");
            _resultCard.RemoveFromClassList("seed-card--rare");
            _resultCard.RemoveFromClassList("seed-card--epic");
            _resultCard.AddToClassList($"seed-card--{seed.Rarity.ToString().ToLower()}");

            if (_rarityTag != null)
            {
                _rarityTag.text = RarityToKorean(seed.Rarity);
                _rarityTag.RemoveFromClassList("rarity-common");
                _rarityTag.RemoveFromClassList("rarity-rare");
                _rarityTag.RemoveFromClassList("rarity-epic");
                _rarityTag.AddToClassList($"rarity-{seed.Rarity.ToString().ToLower()}");
            }

            if (_seedNameLabel != null) _seedNameLabel.text = seed.SeedName;
            if (_seedDescLabel != null) _seedDescLabel.text = $"{seed.GrowthTime}초 후 {seed.ResultCropId} 수확";

            // 씨앗 아트 클래스를 cropId 기반으로 교체
            if (_seedArt != null)
            {
                // 기존 pic-big--* 클래스 모두 제거 후 교체
                var classes = new System.Collections.Generic.List<string>(_seedArt.GetClasses());
                foreach (var cls in classes)
                    if (cls.StartsWith("pic-big--")) _seedArt.RemoveFromClassList(cls);
                string artKey = SeedIdToArtKey(seed.SeedId);
                _seedArt.AddToClassList($"pic-big--{artKey}");
            }
        }

        private void RefreshDrawButton()
        {
            if (_drawBtn == null || _gm == null) return;
            bool canAfford = _gm.Economy.Gold >= _gm.Gacha.PullCost;
            _drawBtn.SetEnabled(canAfford);
            _drawBtn.EnableInClassList("btn--disabled", !canAfford);
        }

        private static string RarityToKorean(Rarity rarity) => rarity switch
        {
            Rarity.Common => "일반",
            Rarity.Rare => "희귀",
            Rarity.Epic => "영웅",
            _ => rarity.ToString()
        };

        private static string SeedIdToArtKey(string seedId) => seedId switch
        {
            "seed_carrot" => "carrot",
            "seed_cabbage" => "cabbage",
            "seed_tomato" => "tomato",
            _ => "carrot"
        };
    }
}
