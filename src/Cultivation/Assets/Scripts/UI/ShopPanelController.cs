using Cultivation.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cultivation.UI
{
    /// <summary>
    /// 상점 패널. 작물 판매 / 크리처 판매 두 탭을 제공한다.
    /// 판매 후 즉시 목록 갱신(확인 다이얼로그 없음, MVP).
    /// </summary>
    public class ShopPanelController : MonoBehaviour
    {
        private enum ShopTab { Crops, Creatures }

        [SerializeField] private UIDocument _document;

        private Button _closeBtn;
        private Button _tabCrops;
        private Button _tabCreatures;
        private VisualElement _saleList;
        private Label _goldTotalLabel;

        private ShopTab _currentTab = ShopTab.Crops;
        private GameManager _gm;

        public void Initialize(GameManager gm)
        {
            _gm = gm;

            var root = _document.rootVisualElement;
            _closeBtn = root.Q<Button>("CloseBtn");

            var tabBar = root.Q<VisualElement>(className: "tab-bar");
            if (tabBar != null)
            {
                var tabs = tabBar.Query<Button>().ToList();
                if (tabs.Count >= 2)
                {
                    _tabCrops = tabs[0];
                    _tabCreatures = tabs[1];
                }
            }

            _saleList = root.Q<VisualElement>(className: "sale-list");
            _goldTotalLabel = root.Q<Label>(className: "gold-total");

            _closeBtn?.RegisterCallback<ClickEvent>(_ => CloseFromPanel());
            _tabCrops?.RegisterCallback<ClickEvent>(_ => SetTab(ShopTab.Crops));
            _tabCreatures?.RegisterCallback<ClickEvent>(_ => SetTab(ShopTab.Creatures));

            gm.Economy.OnGoldChanged += RefreshGoldTotal;
            gm.Inventory.OnCropChanged += (_, __) => { if (_currentTab == ShopTab.Crops) RebuildSaleList(); };
            gm.Barn.OnCreatureAdded += _ => { if (_currentTab == ShopTab.Creatures) RebuildSaleList(); };
            gm.Barn.OnCreatureRemoved += _ => { if (_currentTab == ShopTab.Creatures) RebuildSaleList(); };
        }

        private void OnDestroy()
        {
            if (_gm == null) return;
            _gm.Economy.OnGoldChanged -= RefreshGoldTotal;
        }

        public void Open()
        {
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            SetTab(ShopTab.Crops);
            RefreshGoldTotal(_gm.Economy.Gold);
        }

        public void Close()
        {
            var root = _document != null ? _document.rootVisualElement : null;
            if (root != null) root.style.display = DisplayStyle.None;
        }

        private void CloseFromPanel()
        {
            _gm?.UI.CloseAll();
            _gm?.SetUIModeActive(false);
        }

        private void SetTab(ShopTab tab)
        {
            _currentTab = tab;
            _tabCrops?.EnableInClassList("tab--active-shop", tab == ShopTab.Crops);
            _tabCreatures?.EnableInClassList("tab--active-shop", tab == ShopTab.Creatures);
            RebuildSaleList();
        }

        private void RebuildSaleList()
        {
            if (_saleList == null || _gm == null) return;
            _saleList.Clear();

            if (_currentTab == ShopTab.Crops)
            {
                foreach (var crop in _gm.DataRegistry.Crops)
                {
                    int qty = _gm.Inventory.GetCropCount(crop.CropId);
                    string artKey = CropIdToArtKey(crop.CropId);

                    var row = new VisualElement();
                    row.AddToClassList("sale-row");
                    if (qty == 0) row.AddToClassList("sale-row--disabled");

                    var art = new VisualElement();
                    art.AddToClassList("pic-md");
                    art.AddToClassList($"pic-crop--{artKey}");
                    row.Add(art);

                    var nameLabel = new Label(crop.CropName);
                    nameLabel.AddToClassList("t-semibold");
                    nameLabel.AddToClassList("t-md");
                    nameLabel.AddToClassList("sale-name");
                    if (qty == 0) nameLabel.AddToClassList("t-muted");
                    row.Add(nameLabel);

                    var qtyLabel = new Label($"×{qty}");
                    qtyLabel.AddToClassList("t-bold");
                    qtyLabel.AddToClassList("t-md");
                    qtyLabel.AddToClassList("sale-qty");
                    if (qty == 0) qtyLabel.AddToClassList("t-muted");
                    row.Add(qtyLabel);

                    var costChip = new VisualElement();
                    costChip.AddToClassList("cost-chip");
                    costChip.AddToClassList("cost-chip--small");
                    costChip.AddToClassList("cost-chip--earn");
                    var costIcon = new VisualElement();
                    costIcon.AddToClassList("cost-icon");
                    costChip.Add(costIcon);
                    var priceLabel = new Label($"{crop.SellPrice} G");
                    priceLabel.AddToClassList("t-bold");
                    priceLabel.AddToClassList("t-sm");
                    priceLabel.AddToClassList("cost-text");
                    costChip.Add(priceLabel);
                    row.Add(costChip);

                    string capturedCropId = crop.CropId;
                    var sellBtn = new Button(() => OnSellCrop(capturedCropId));
                    sellBtn.text = "판매";
                    sellBtn.AddToClassList("btn");
                    sellBtn.AddToClassList("btn--shop");
                    sellBtn.AddToClassList("btn--small");
                    sellBtn.SetEnabled(qty > 0);
                    if (qty == 0) sellBtn.AddToClassList("btn--disabled");
                    row.Add(sellBtn);

                    _saleList.Add(row);
                }
            }
            else // Creatures
            {
                foreach (var slot in _gm.Barn.Slots)
                {
                    if (slot.IsEmpty) continue;
                    var creature = slot.Creature;
                    var data = _gm.DataRegistry.FindCreature(creature.CreatureId);
                    if (data == null) continue;
                    bool busy = creature.IsBusy;
                    string artKey = CreatureIdToArtKey(creature.CreatureId);

                    var row = new VisualElement();
                    row.AddToClassList("sale-row");
                    if (busy) row.AddToClassList("sale-row--disabled");

                    var art = new VisualElement();
                    art.AddToClassList("pic-md");
                    art.AddToClassList($"creature-art--{artKey}");
                    row.Add(art);

                    var nameLabel = new Label(data.CreatureName);
                    nameLabel.AddToClassList("t-semibold");
                    nameLabel.AddToClassList("t-md");
                    nameLabel.AddToClassList("sale-name");
                    if (busy) nameLabel.AddToClassList("t-muted");
                    row.Add(nameLabel);

                    var costChip = new VisualElement();
                    costChip.AddToClassList("cost-chip");
                    costChip.AddToClassList("cost-chip--small");
                    costChip.AddToClassList("cost-chip--earn");
                    var costIcon = new VisualElement();
                    costIcon.AddToClassList("cost-icon");
                    costChip.Add(costIcon);
                    var priceLabel = new Label($"{data.BaseSellPrice} G");
                    priceLabel.AddToClassList("t-bold");
                    priceLabel.AddToClassList("t-sm");
                    priceLabel.AddToClassList("cost-text");
                    costChip.Add(priceLabel);
                    row.Add(costChip);

                    string capturedInstanceId = creature.InstanceId;
                    var sellBtn = new Button(() => OnSellCreature(capturedInstanceId));
                    sellBtn.text = "판매";
                    sellBtn.AddToClassList("btn");
                    sellBtn.AddToClassList("btn--shop");
                    sellBtn.AddToClassList("btn--small");
                    sellBtn.SetEnabled(!busy);
                    if (busy) sellBtn.AddToClassList("btn--disabled");
                    row.Add(sellBtn);

                    _saleList.Add(row);
                }
            }
        }

        private void OnSellCrop(string cropId)
        {
            if (_gm == null) return;
            _gm.Trade.TrySellCrop(cropId);
            RebuildSaleList();
            RefreshGoldTotal(_gm.Economy.Gold);
        }

        private void OnSellCreature(string instanceId)
        {
            if (_gm == null) return;
            _gm.Trade.TrySellCreature(instanceId);
            RebuildSaleList();
            RefreshGoldTotal(_gm.Economy.Gold);
        }

        private void RefreshGoldTotal(int gold)
        {
            if (_goldTotalLabel != null) _goldTotalLabel.text = $"{gold} G";
        }

        private static string CropIdToArtKey(string cropId) => cropId switch
        {
            "crop_carrot" => "carrot",
            "crop_cabbage" => "cabbage",
            "crop_tomato" => "tomato",
            _ => "carrot"
        };

        private static string CreatureIdToArtKey(string creatureId) => creatureId switch
        {
            "creature_carrot_horse" => "carrot-horse",
            "creature_cabbage_cow" => "cabbage-cow",
            "creature_tomato_chicken" => "tomato-chicken",
            "creature_salad_horse" => "salad-horse",
            "creature_ketchup_chicken" => "ketchup-chicken",
            _ => "carrot-horse"
        };
    }
}
