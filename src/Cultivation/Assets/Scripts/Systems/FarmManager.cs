using System;
using System.Collections.Generic;
using Cultivation.Data;
using Cultivation.Runtime;
using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 밭 시스템. 플롯 목록 관리, 시간 누적 틱, 심기/수확/확장 처리.
    /// GameManager.Update에서 Tick(deltaTime) 호출 필요.
    /// </summary>
    public class FarmManager
    {
        private readonly InventoryManager _inventory;
        private readonly EconomyManager _economy;
        private readonly ExpansionConfig _expansionConfig;
        private readonly GameDataRegistry _registry;
        private readonly List<FarmPlot> _plots = new List<FarmPlot>();

        public event Action<int, PlotState> OnPlotStateChanged;
        public event Action<int> OnFarmExpanded;

        public IReadOnlyList<FarmPlot> Plots => _plots;
        public int PlotCount => _plots.Count;

        public FarmManager(InventoryManager inventory, EconomyManager economy,
            ExpansionConfig expansionConfig, GameDataRegistry registry, int startingPlots)
        {
            _inventory = inventory;
            _economy = economy;
            _expansionConfig = expansionConfig;
            _registry = registry;
            for (int i = 0; i < startingPlots; i++)
                _plots.Add(new FarmPlot());
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < _plots.Count; i++)
            {
                var plot = _plots[i];
                if (plot.Tick(deltaTime))
                {
                    OnPlotStateChanged?.Invoke(i, plot.State);
                }
            }
        }

        /// <summary>지정 플롯에 씨앗 심기. 인벤토리에서 1개 차감.</summary>
        public bool PlantSeed(int plotIndex, string seedId)
        {
            if (plotIndex < 0 || plotIndex >= _plots.Count) return false;

            var plot = _plots[plotIndex];
            if (!plot.IsEmpty) return false;

            var seed = _registry != null ? _registry.FindSeed(seedId) : null;
            if (seed == null)
            {
                Debug.LogError($"[FarmManager] SeedData가 레지스트리에 없습니다: {seedId}");
                return false;
            }

            if (_inventory.GetSeedCount(seedId) <= 0) return false;
            if (!_inventory.RemoveSeed(seedId, 1)) return false;

            if (!plot.Plant(seed))
            {
                _inventory.AddSeed(seedId, 1);
                return false;
            }

            OnPlotStateChanged?.Invoke(plotIndex, plot.State);
            return true;
        }

        /// <summary>지정 플롯 수확. 결과 작물을 인벤토리에 추가하고 플롯을 비운다. 성공 시 true.</summary>
        public bool Harvest(int plotIndex, out CropData harvested)
        {
            harvested = null;
            if (plotIndex < 0 || plotIndex >= _plots.Count) return false;

            var plot = _plots[plotIndex];
            if (!plot.IsReady) return false;

            var seed = plot.CurrentSeed;
            if (seed == null) return false;

            var crop = _registry != null ? _registry.FindCrop(seed.ResultCropId) : null;
            if (crop == null)
            {
                Debug.LogError($"[FarmManager] CropData가 레지스트리에 없습니다: {seed.ResultCropId}");
                return false;
            }

            plot.Harvest();
            _inventory.AddCrop(crop.CropId, 1);
            harvested = crop;
            OnPlotStateChanged?.Invoke(plotIndex, plot.State);
            return true;
        }

        public int GetExpansionCost()
        {
            if (_expansionConfig == null) return 0;
            return _expansionConfig.CalculateCost(_expansionConfig.FarmBaseCost, _plots.Count);
        }

        public bool TryExpand()
        {
            int cost = GetExpansionCost();
            if (!_economy.TrySpendGold(cost)) return false;
            _plots.Add(new FarmPlot());
            OnFarmExpanded?.Invoke(_plots.Count);
            return true;
        }

        /// <summary>진행 중인(Growing) 플롯이 하나라도 있으면 true. 게임 종료 알림용.</summary>
        public bool HasGrowingPlots()
        {
            for (int i = 0; i < _plots.Count; i++)
                if (_plots[i].State == PlotState.Growing) return true;
            return false;
        }
    }
}
