using System.Collections.Generic;
using Cultivation.Runtime;
using Cultivation.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cultivation.UI
{
    /// <summary>
    /// 사육장 패널. 탭(크리처화/교배/판매)을 관리하고, 슬롯 그리드 + 활성 교배 진행 표시를 동적으로 갱신한다.
    /// </summary>
    public class BarnPanelController : MonoBehaviour
    {
        private enum BarnTab { Convert, Breed, Sell }

        [SerializeField] private UIDocument _document;

        private Button _closeBtn;
        private VisualElement _barnGrid;

        // 탭 버튼 3개
        private Button _tabConvert;
        private Button _tabBreed;
        private Button _tabSell;

        // 크리처화 영역
        private VisualElement _convertRow;
        private Button _convertBtn;
        private string _selectedCropId;

        // 교배 선택 상태
        private string _breedSelectA;
        private string _breedSelectB;
        private Button _breedStartBtn;

        // 활성 교배 진행 표시
        private VisualElement _activeBreedSection;
        private VisualElement _activeBreedProgressFill;
        private Label _activeBreedTimeLeft;
        private Button _cancelBreedBtn;

        // 확장
        private Label _expandCostLabel;
        private Button _expandBtn;

        private BarnTab _currentTab = BarnTab.Convert;
        private GameManager _gm;

        public void Initialize(GameManager gm)
        {
            _gm = gm;

            var root = _document.rootVisualElement;
            _closeBtn = root.Q<Button>("CloseBtn");
            _barnGrid = root.Q<VisualElement>(className: "barn-grid");

            // 탭 버튼은 tab-bar 하위 3번째 Button들
            var tabBar = root.Q<VisualElement>(className: "tab-bar");
            if (tabBar != null)
            {
                var tabs = tabBar.Query<Button>().ToList();
                if (tabs.Count >= 3)
                {
                    _tabConvert = tabs[0];
                    _tabBreed = tabs[1];
                    _tabSell = tabs[2];
                }
            }

            // 크리처화 버튼 (convert-row 내 btn--barn)
            _convertRow = root.Q<VisualElement>(className: "convert-row");
            _convertBtn = _convertRow?.Q<Button>();

            // 교배 시작 버튼 — 초기엔 없으므로 동적으로 생성
            _breedStartBtn = new Button(OnBreedStart);
            _breedStartBtn.text = "교배 시작";
            _breedStartBtn.AddToClassList("btn");
            _breedStartBtn.AddToClassList("btn--barn");
            _breedStartBtn.AddToClassList("btn--small");
            _breedStartBtn.SetEnabled(false);

            // 활성 교배 영역
            _activeBreedSection = root.Q<VisualElement>(className: "active-breed");
            if (_activeBreedSection != null)
            {
                _activeBreedProgressFill = _activeBreedSection.Q<VisualElement>(className: "progress-fill");
                _activeBreedTimeLeft = _activeBreedSection.Q<Label>(className: "time-left");
                _cancelBreedBtn = _activeBreedSection.Q<Button>();
                _cancelBreedBtn?.RegisterCallback<ClickEvent>(_ => OnCancelBreed());
            }

            // 확장 행
            var expandRow = root.Q<VisualElement>(className: "expand-row");
            if (expandRow != null)
            {
                _expandCostLabel = expandRow.Q<Label>(className: "cost-text");
                _expandBtn = expandRow.Q<Button>();
                _expandBtn?.RegisterCallback<ClickEvent>(_ => OnExpand());
            }

            _closeBtn?.RegisterCallback<ClickEvent>(_ => CloseFromPanel());
            _convertBtn?.RegisterCallback<ClickEvent>(_ => OnConvert());
            _tabConvert?.RegisterCallback<ClickEvent>(_ => SetTab(BarnTab.Convert));
            _tabBreed?.RegisterCallback<ClickEvent>(_ => SetTab(BarnTab.Breed));
            _tabSell?.RegisterCallback<ClickEvent>(_ => SetTab(BarnTab.Sell));

            gm.Barn.OnCreatureAdded += _ => Refresh();
            gm.Barn.OnCreatureRemoved += _ => Refresh();
            gm.Barn.OnBarnExpanded += _ => Refresh();
        }

        private void OnDestroy()
        {
            if (_gm == null) return;
            _gm.Barn.OnCreatureAdded -= _ => Refresh();
            _gm.Barn.OnCreatureRemoved -= _ => Refresh();
            _gm.Barn.OnBarnExpanded -= _ => Refresh();
        }

        public void Open()
        {
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            _selectedCropId = null;
            _breedSelectA = null;
            _breedSelectB = null;
            SetTab(BarnTab.Convert);
            Refresh();
        }

        public void Close()
        {
            _document.rootVisualElement.style.display = DisplayStyle.None;
        }

        private void Update()
        {
            if (_gm == null) return;
            RefreshActiveBreeding();
        }

        private void CloseFromPanel()
        {
            _gm?.UI.CloseAll();
            _gm?.SetUIModeActive(false);
        }

        private void SetTab(BarnTab tab)
        {
            _currentTab = tab;

            SetTabActive(_tabConvert, tab == BarnTab.Convert);
            SetTabActive(_tabBreed, tab == BarnTab.Breed);
            SetTabActive(_tabSell, tab == BarnTab.Sell);

            // 탭에 따라 콘텐츠 영역 가시성 조정
            SetDisplay(_convertRow, tab == BarnTab.Convert);
            if (_breedStartBtn.parent != null && tab != BarnTab.Breed)
                _breedStartBtn.RemoveFromHierarchy();

            Refresh();
        }

        private void Refresh()
        {
            RebuildSlotGrid();
            RefreshExpandRow();

            if (_currentTab == BarnTab.Convert) RebuildConvertRow();
            else if (_currentTab == BarnTab.Breed) RebuildBreedTab();
            else if (_currentTab == BarnTab.Sell) RebuildSellTab();
        }

        private void RebuildSlotGrid()
        {
            if (_barnGrid == null || _gm == null) return;
            _barnGrid.Clear();

            foreach (var slot in _gm.Barn.Slots)
            {
                var slotEl = new VisualElement();
                slotEl.AddToClassList("creature-slot");

                if (slot.IsEmpty)
                {
                    var card = new VisualElement();
                    card.AddToClassList("creature-card");
                    card.AddToClassList("creature-card--empty");
                    var emptyLabel = new Label("비어 있음");
                    emptyLabel.AddToClassList("t-medium");
                    emptyLabel.AddToClassList("t-sm");
                    emptyLabel.AddToClassList("empty-text");
                    card.Add(emptyLabel);
                    slotEl.Add(card);
                }
                else
                {
                    var creature = slot.Creature;
                    var data = _gm.DataRegistry.FindCreature(creature.CreatureId);
                    string artKey = CreatureIdToArtKey(creature.CreatureId);
                    bool busy = creature.IsBusy;

                    // 선택 상태 (교배 탭)
                    bool isSelectedForBreed = _currentTab == BarnTab.Breed &&
                        (creature.InstanceId == _breedSelectA || creature.InstanceId == _breedSelectB);

                    var card = new VisualElement();
                    card.AddToClassList("creature-card");
                    card.AddToClassList($"creature-card--{artKey}");
                    if (busy) card.AddToClassList("creature-card--busy");
                    if (isSelectedForBreed) card.AddToClassList("creature-card--selected");

                    var art = new VisualElement();
                    art.AddToClassList("creature-art");
                    art.AddToClassList($"creature-art--{artKey}");
                    card.Add(art);

                    var nameLabel = new Label(data != null ? data.CreatureName : creature.CreatureId);
                    nameLabel.AddToClassList("t-bold");
                    nameLabel.AddToClassList("t-md");
                    nameLabel.AddToClassList("creature-name");
                    card.Add(nameLabel);

                    if (busy)
                    {
                        var busyTag = new VisualElement();
                        busyTag.AddToClassList("busy-tag");
                        var busyLabel = new Label("교배 중");
                        busyLabel.AddToClassList("t-bold");
                        busyLabel.AddToClassList("t-xs");
                        busyLabel.AddToClassList("busy-tag-label");
                        busyTag.Add(busyLabel);
                        card.Add(busyTag);
                    }
                    else if (_currentTab == BarnTab.Breed)
                    {
                        string capturedId = creature.InstanceId;
                        card.RegisterCallback<ClickEvent>(_ => OnBreedSelect(capturedId));
                    }
                    else if (_currentTab == BarnTab.Sell && data != null)
                    {
                        var subLabel = new Label($"기본 {data.BaseSellPrice}G");
                        subLabel.AddToClassList("t-xs");
                        subLabel.AddToClassList("t-muted");
                        subLabel.AddToClassList("creature-sub");
                        card.Add(subLabel);

                        string capturedInstanceId = creature.InstanceId;
                        var sellBtn = new Button(() => OnSellCreature(capturedInstanceId));
                        sellBtn.text = "판매";
                        sellBtn.AddToClassList("btn");
                        sellBtn.AddToClassList("btn--shop");
                        sellBtn.AddToClassList("btn--small");
                        card.Add(sellBtn);
                    }

                    slotEl.Add(card);
                }

                _barnGrid.Add(slotEl);
            }
        }

        private void RebuildConvertRow()
        {
            if (_convertRow == null || _gm == null) return;
            _convertRow.Clear();

            bool anySelected = false;
            foreach (var crop in _gm.DataRegistry.Crops)
            {
                int count = _gm.Inventory.GetCropCount(crop.CropId);
                string artKey = CropIdToArtKey(crop.CropId);
                bool selected = crop.CropId == _selectedCropId;

                var pick = new VisualElement();
                pick.AddToClassList("convert-pick");
                if (selected) pick.AddToClassList("convert-pick--active");
                if (count == 0) pick.AddToClassList("convert-pick--disabled");

                var art = new VisualElement();
                art.AddToClassList("pic-md");
                art.AddToClassList($"pic-crop--{artKey}");
                pick.Add(art);

                var label = new Label($"{crop.CropName} ×{count}");
                label.AddToClassList("t-medium");
                label.AddToClassList("t-sm");
                if (count == 0) label.AddToClassList("t-muted");
                pick.Add(label);

                if (count > 0)
                {
                    string capturedId = crop.CropId;
                    pick.RegisterCallback<ClickEvent>(_ =>
                    {
                        _selectedCropId = capturedId;
                        RebuildConvertRow();
                    });
                    if (selected) anySelected = true;
                }

                _convertRow.Add(pick);
            }

            // 패딩 + 크리처화 버튼
            var spacer = new VisualElement();
            spacer.AddToClassList("convert-spacer");
            _convertRow.Add(spacer);

            bool barnFull = !_gm.Barn.HasFreeSlot();
            var btn = new Button(OnConvert);
            btn.text = "크리처화";
            btn.AddToClassList("btn");
            btn.AddToClassList("btn--barn");
            btn.AddToClassList("btn--small");
            bool canConvert = anySelected && !barnFull;
            btn.SetEnabled(canConvert);
            if (!canConvert) btn.AddToClassList("btn--disabled");
            _convertRow.Add(btn);
        }

        private void RebuildBreedTab()
        {
            // 슬롯 그리드 클릭으로 교배 부모 선택 → 버튼 삽입
            // 브리드 시작 버튼을 convertRow 자리에 놓기
            SetDisplay(_convertRow, false);

            bool canBreed = !string.IsNullOrEmpty(_breedSelectA) && !string.IsNullOrEmpty(_breedSelectB);
            _breedStartBtn.SetEnabled(canBreed);
            _breedStartBtn.EnableInClassList("btn--disabled", !canBreed);

            if (_breedStartBtn.parent == null && _barnGrid?.parent != null)
            {
                // 그리드 다음에 버튼 삽입
                _barnGrid.parent.Add(_breedStartBtn);
            }
        }

        private void RebuildSellTab()
        {
            SetDisplay(_convertRow, false);
        }

        private void RefreshActiveBreeding()
        {
            if (_activeBreedSection == null || _gm == null) return;
            bool hasBreeding = _gm.Breeding.HasActiveBreeding();
            _activeBreedSection.style.display = hasBreeding ? DisplayStyle.Flex : DisplayStyle.None;

            if (!hasBreeding) return;

            var tasks = _gm.Breeding.ActiveTasks;
            if (tasks.Count == 0) return;
            var task = tasks[0];

            float progress = task.Progress;
            if (_activeBreedProgressFill != null)
                _activeBreedProgressFill.style.width = Length.Percent(progress * 100f);

            if (_activeBreedTimeLeft != null)
            {
                float remaining = task.RequiredTime - task.ElapsedTime;
                _activeBreedTimeLeft.text = $"{Mathf.Max(0f, remaining):F0}초 남음";
            }
        }

        private void OnBreedSelect(string instanceId)
        {
            if (_breedSelectA == null)
            {
                _breedSelectA = instanceId;
            }
            else if (_breedSelectB == null && instanceId != _breedSelectA)
            {
                _breedSelectB = instanceId;
            }
            else
            {
                // 토글: 이미 선택된 것이면 해제
                if (instanceId == _breedSelectA) _breedSelectA = _breedSelectB;
                else if (instanceId == _breedSelectB) _breedSelectB = null;
                else _breedSelectA = instanceId; // 새 선택 → A 교체
            }

            RebuildSlotGrid();
            RebuildBreedTab();
        }

        private void OnBreedStart()
        {
            if (_gm == null || string.IsNullOrEmpty(_breedSelectA) || string.IsNullOrEmpty(_breedSelectB)) return;
            _gm.Breeding.StartBreeding(_breedSelectA, _breedSelectB);
            _breedSelectA = null;
            _breedSelectB = null;
            Refresh();
        }

        private void OnCancelBreed()
        {
            if (_gm == null) return;
            var tasks = _gm.Breeding.ActiveTasks;
            if (tasks.Count > 0) _gm.Breeding.CancelBreeding(tasks[0].TaskId);
        }

        private void OnConvert()
        {
            if (_gm == null || string.IsNullOrEmpty(_selectedCropId)) return;
            _gm.Creature.ConvertCropToCreature(_selectedCropId);
            _selectedCropId = null;
            RebuildConvertRow();
        }

        private void OnSellCreature(string instanceId)
        {
            if (_gm == null) return;
            _gm.Trade.TrySellCreature(instanceId);
        }

        private void OnExpand()
        {
            if (_gm == null) return;
            _gm.Barn.TryExpand();
        }

        private void RefreshExpandRow()
        {
            if (_gm == null) return;
            int cost = _gm.Barn.GetExpansionCost();
            bool canAfford = _gm.Economy.Gold >= cost;
            if (_expandCostLabel != null) _expandCostLabel.text = $"{cost} G";
            if (_expandBtn != null)
            {
                _expandBtn.SetEnabled(canAfford);
                _expandBtn.EnableInClassList("btn--disabled", !canAfford);
            }
        }

        private static void SetDisplay(VisualElement el, bool visible)
        {
            if (el == null) return;
            el.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static void SetTabActive(Button btn, bool active)
        {
            if (btn == null) return;
            btn.EnableInClassList("tab--active", active);
        }

        private static string CreatureIdToArtKey(string creatureId) => creatureId switch
        {
            "creature_carrot_horse" => "carrot-horse",
            "creature_cabbage_cow" => "cabbage-cow",
            "creature_tomato_chicken" => "tomato-chicken",
            "creature_salad_horse" => "salad-horse",
            "creature_ketchup_chicken" => "ketchup-chicken",
            _ => "carrot-horse"
        };

        private static string CropIdToArtKey(string cropId) => cropId switch
        {
            "crop_carrot" => "carrot",
            "crop_cabbage" => "cabbage",
            "crop_tomato" => "tomato",
            _ => "carrot"
        };
    }
}
