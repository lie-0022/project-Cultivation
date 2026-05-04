using Cultivation.Runtime;
using Cultivation.Systems;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cultivation.UI
{
    /// <summary>
    /// 밭 플롯 패널. Empty / Growing / Ready 세 모드를 런타임에 표시 전환.
    /// Open(plotIndex)으로 진입하며, 모드는 FarmManager.Plots[plotIndex].State 기반.
    /// </summary>
    public class FarmPlotPanelController : MonoBehaviour
    {
        [SerializeField] private UIDocument _document;

        // 모드 컨테이너
        private VisualElement _emptyMode;
        private VisualElement _growingMode;
        private VisualElement _readyMode;

        // Empty 모드: 씨앗 목록 행(심기 버튼 포함)
        private VisualElement _seedList;

        // Growing 모드
        private Label _growingTitle;
        private Label _growingTimeLeft;
        private VisualElement _progressFill;
        private VisualElement _growingArt;

        // Ready 모드
        private Label _readySubLabel;
        private Button _harvestBtn;

        // 공통
        private Label _titleLabel;
        private Button _closeBtn;
        private Label _expandCostLabel;
        private Button _expandBtn;

        private GameManager _gm;
        private int _plotIndex = -1;

        public void Initialize(GameManager gm)
        {
            _gm = gm;

            var root = _document.rootVisualElement;
            _titleLabel = root.Q<Label>("FarmTitle");
            _closeBtn = root.Q<Button>("CloseBtn");

            _emptyMode = root.Q<VisualElement>("EmptyMode");
            _growingMode = root.Q<VisualElement>("GrowingMode");
            _readyMode = root.Q<VisualElement>("ReadyMode");

            _seedList = _emptyMode?.Q<VisualElement>(className: "seed-list");

            _growingTitle = _growingMode?.Q<Label>(className: "mode-prompt");
            _growingTimeLeft = _growingMode?.Q<Label>(className: "mode-sub");
            _progressFill = _growingMode?.Q<VisualElement>(className: "progress-fill");
            _growingArt = _growingMode?.Q<VisualElement>(className: "growing-art")?.Q<VisualElement>(className: "pic-big");

            _readySubLabel = _readyMode?.Q<Label>(className: "mode-sub");
            _harvestBtn = _readyMode?.Q<Button>();

            // 확장 행
            var expandRow = root.Q<VisualElement>(className: "expand-row");
            if (expandRow != null)
            {
                _expandCostLabel = expandRow.Q<Label>(className: "cost-text");
                _expandBtn = expandRow.Q<Button>();
                _expandBtn?.RegisterCallback<ClickEvent>(_ => OnExpand());
            }

            _closeBtn?.RegisterCallback<ClickEvent>(_ => CloseFromPanel());
            _harvestBtn?.RegisterCallback<ClickEvent>(_ => OnHarvest());

            gm.Farm.OnPlotStateChanged += OnPlotStateChanged;
        }

        private void OnDestroy()
        {
            if (_gm != null) _gm.Farm.OnPlotStateChanged -= OnPlotStateChanged;
        }

        public void Open(int plotIndex)
        {
            _plotIndex = plotIndex;
            _document.rootVisualElement.style.display = DisplayStyle.Flex;

            if (_titleLabel != null) _titleLabel.text = $"밭 ({plotIndex + 1}번 칸)";

            Refresh();
        }

        public void Close()
        {
            var root = _document != null ? _document.rootVisualElement : null;
            if (root != null) root.style.display = DisplayStyle.None;
            _plotIndex = -1;
        }

        private void CloseFromPanel()
        {
            _gm?.UI.CloseAll();
            _gm?.SetUIModeActive(false);
        }

        private void Update()
        {
            // Growing 모드일 때 진행도 매 프레임 갱신
            if (_plotIndex < 0 || _gm == null) return;
            var plots = _gm.Farm.Plots;
            if (_plotIndex >= plots.Count) return;
            var plot = plots[_plotIndex];
            if (plot.State != PlotState.Growing) return;

            float progress = plot.Progress;
            if (_progressFill != null)
                _progressFill.style.width = Length.Percent(progress * 100f);

            if (_growingTimeLeft != null && plot.CurrentSeed != null)
            {
                float remaining = plot.CurrentSeed.GrowthTime - plot.ElapsedTime;
                _growingTimeLeft.text = $"{Mathf.Max(0f, remaining):F0}초 남음";
            }
        }

        private void Refresh()
        {
            if (_plotIndex < 0 || _gm == null) return;
            var plots = _gm.Farm.Plots;
            if (_plotIndex >= plots.Count) return;
            var plot = plots[_plotIndex];

            SetMode(plot.State);
            RefreshExpandRow();

            if (plot.State == PlotState.Empty) RebuildSeedList();
            if (plot.State == PlotState.Growing) RefreshGrowingMode(plot);
            if (plot.State == PlotState.Ready) RefreshReadyMode(plot);
        }

        private void RefreshGrowingMode(Runtime.FarmPlot plot)
        {
            if (plot.CurrentSeed == null || _gm == null) return;
            var crop = _gm.DataRegistry.FindCrop(plot.CurrentSeed.ResultCropId);
            string cropName = crop != null ? crop.CropName : plot.CurrentSeed.SeedName;
            if (_growingTitle != null) _growingTitle.text = $"{cropName} 자라는 중…";

            if (_growingArt != null)
            {
                var classes = new System.Collections.Generic.List<string>(_growingArt.GetClasses());
                foreach (var cls in classes)
                    if (cls.StartsWith("pic-big--")) _growingArt.RemoveFromClassList(cls);
                _growingArt.AddToClassList($"pic-big--{SeedIdToArtKey(plot.CurrentSeed.SeedId)}");
            }
        }

        private void SetMode(PlotState state)
        {
            Debug.Log($"[FarmPanel] SetMode={state} | empty={_emptyMode!=null} growing={_growingMode!=null} ready={_readyMode!=null}");
            SetDisplay(_emptyMode, state == PlotState.Empty);
            SetDisplay(_growingMode, state == PlotState.Growing);
            SetDisplay(_readyMode, state == PlotState.Ready);
        }

        private void RebuildSeedList()
        {
            if (_seedList == null || _gm == null) return;
            _seedList.Clear();

            foreach (var seed in _gm.DataRegistry.Seeds)
            {
                int count = _gm.Inventory.GetSeedCount(seed.SeedId);

                var row = new VisualElement();
                row.AddToClassList("seed-list-item");
                if (count == 0) row.AddToClassList("seed-list-item--disabled");

                // 아트 placeholder
                var art = new VisualElement();
                art.AddToClassList("pic-md");
                art.AddToClassList($"pic-seed--{SeedIdToArtKey(seed.SeedId)}");
                row.Add(art);

                // 메타
                var meta = new VisualElement();
                meta.AddToClassList("seed-list-meta");
                var nameLabel = new Label(seed.SeedName);
                nameLabel.AddToClassList("t-semibold");
                nameLabel.AddToClassList("t-md");
                var infoLabel = new Label($"{seed.GrowthTime}초 · {RarityToKorean(seed.Rarity)}");
                infoLabel.AddToClassList("t-sm");
                infoLabel.AddToClassList("t-muted");
                meta.Add(nameLabel);
                meta.Add(infoLabel);
                row.Add(meta);

                // 수량
                var countLabel = new Label($"×{count}");
                countLabel.AddToClassList("t-bold");
                countLabel.AddToClassList("t-md");
                countLabel.AddToClassList("seed-list-count");
                if (count == 0) countLabel.AddToClassList("seed-list-count--zero");
                row.Add(countLabel);

                // 심기 버튼
                string capturedSeedId = seed.SeedId;
                var plantBtn = new Button(() => OnPlant(capturedSeedId));
                plantBtn.text = "심기";
                plantBtn.AddToClassList("btn");
                plantBtn.AddToClassList("btn--primary");
                plantBtn.AddToClassList("btn--small");
                if (count == 0)
                {
                    plantBtn.SetEnabled(false);
                    plantBtn.AddToClassList("btn--disabled");
                }
                row.Add(plantBtn);

                _seedList.Add(row);
            }
        }

        private void RefreshReadyMode(Runtime.FarmPlot plot)
        {
            if (plot.CurrentSeed == null || _gm == null) return;
            var crop = _gm.DataRegistry.FindCrop(plot.CurrentSeed.ResultCropId);
            if (_readySubLabel != null && crop != null)
                _readySubLabel.text = $"{crop.CropName} 1개를 얻습니다";
        }

        private void RefreshExpandRow()
        {
            if (_gm == null) return;
            int cost = _gm.Farm.GetExpansionCost();
            bool canAfford = _gm.Economy.Gold >= cost;

            if (_expandCostLabel != null) _expandCostLabel.text = $"{cost} G";
            if (_expandBtn != null)
            {
                _expandBtn.SetEnabled(canAfford);
                _expandBtn.EnableInClassList("btn--disabled", !canAfford);
            }
        }

        private void OnPlant(string seedId)
        {
            if (_plotIndex < 0 || _gm == null) return;
            int before = _gm.Inventory.GetSeedCount(seedId);
            bool ok = _gm.Farm.PlantSeed(_plotIndex, seedId);
            int after = _gm.Inventory.GetSeedCount(seedId);
            var plot = _gm.Farm.Plots[_plotIndex];
            Debug.Log($"[FarmPanel] OnPlant seed={seedId} plot={_plotIndex} ok={ok} count {before}→{after} state={plot.State}");
        }

        private void OnHarvest()
        {
            if (_plotIndex < 0 || _gm == null) return;
            _gm.Farm.Harvest(_plotIndex, out _);
        }

        private void OnExpand()
        {
            if (_gm == null) return;
            _gm.Farm.TryExpand();
            RefreshExpandRow();
        }

        private void OnPlotStateChanged(int idx, PlotState state)
        {
            Debug.Log($"[FarmPanel] OnPlotStateChanged received idx={idx} state={state} (panel _plotIndex={_plotIndex})");
            if (idx != _plotIndex) return;
            Refresh();
        }

        private static void SetDisplay(VisualElement el, bool visible)
        {
            if (el == null) return;
            el.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private static string SeedIdToArtKey(string seedId) => seedId switch
        {
            "seed_carrot" => "carrot",
            "seed_cabbage" => "cabbage",
            "seed_tomato" => "tomato",
            _ => "carrot"
        };

        private static string RarityToKorean(Data.Rarity rarity) => rarity switch
        {
            Data.Rarity.Common => "일반",
            Data.Rarity.Rare => "희귀",
            Data.Rarity.Epic => "영웅",
            _ => rarity.ToString()
        };
    }
}
