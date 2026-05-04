using System;
using Cultivation.Data;
using Cultivation.Runtime;
using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 판매 오케스트레이션. 작물/크리처를 인벤토리/사육장에서 차감하고 그 가치만큼 골드를 지급한다.
    /// EconomyManager는 골드 가감만 책임지고, 본 매니저가 데이터 lookup + 차감 + 검증을 조합한다.
    /// </summary>
    public class TradeManager
    {
        private readonly InventoryManager _inventory;
        private readonly BarnManager _barn;
        private readonly EconomyManager _economy;
        private readonly GameDataRegistry _registry;

        public event Action<string, int> OnCropSold;        // (cropId, price)
        public event Action<string, int> OnCreatureSold;    // (instanceId, price)

        public TradeManager(InventoryManager inventory, BarnManager barn, EconomyManager economy, GameDataRegistry registry)
        {
            _inventory = inventory;
            _barn = barn;
            _economy = economy;
            _registry = registry;
        }

        /// <summary>작물 1개 판매. 인벤토리에서 차감 + sellPrice 만큼 골드 획득. 실패 시 false (변동 없음).</summary>
        public bool TrySellCrop(string cropId)
        {
            if (string.IsNullOrEmpty(cropId)) return false;

            var crop = _registry != null ? _registry.FindCrop(cropId) : null;
            if (crop == null)
            {
                Debug.LogError($"[TradeManager] CropData를 찾을 수 없습니다: {cropId}");
                return false;
            }

            if (_inventory.GetCropCount(cropId) <= 0) return false;
            if (!_inventory.RemoveCrop(cropId, 1)) return false;

            _economy.AddGold(crop.SellPrice);
            OnCropSold?.Invoke(cropId, crop.SellPrice);
            return true;
        }

        /// <summary>크리처 1마리 판매. Busy 크리처는 차단. 사육장에서 제거 + baseSellPrice 만큼 골드 획득.</summary>
        public bool TrySellCreature(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId)) return false;

            var creature = _barn.FindCreature(instanceId);
            if (creature == null) return false;
            if (creature.IsBusy)
            {
                Debug.LogWarning($"[TradeManager] 점유 중인 크리처는 판매할 수 없습니다: {instanceId}");
                return false;
            }

            var data = _registry != null ? _registry.FindCreature(creature.CreatureId) : null;
            if (data == null)
            {
                Debug.LogError($"[TradeManager] CreatureData를 찾을 수 없습니다: {creature.CreatureId}");
                return false;
            }

            if (!_barn.RemoveCreature(instanceId)) return false;

            _economy.AddGold(data.BaseSellPrice);
            OnCreatureSold?.Invoke(instanceId, data.BaseSellPrice);
            return true;
        }
    }
}
