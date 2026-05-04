using Cultivation.Data;
using Cultivation.Runtime;
using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 작물을 크리처로 변환하여 사육장에 추가하는 시스템. MVP는 무료/즉시 변환.
    /// </summary>
    public class CreatureManager
    {
        private readonly InventoryManager _inventory;
        private readonly BarnManager _barn;
        private readonly GameDataRegistry _registry;

        public CreatureManager(InventoryManager inventory, BarnManager barn, GameDataRegistry registry)
        {
            _inventory = inventory;
            _barn = barn;
            _registry = registry;
        }

        /// <summary>작물 1개를 크리처로 변환. 실패 시 null.</summary>
        public CreatureInstance ConvertCropToCreature(string cropId)
        {
            if (string.IsNullOrEmpty(cropId)) return null;

            var cropData = _registry != null ? _registry.FindCrop(cropId) : null;
            if (cropData == null)
            {
                Debug.LogError($"[CreatureManager] CropData가 레지스트리에 없습니다: {cropId}");
                return null;
            }

            if (string.IsNullOrEmpty(cropData.CreatureId))
            {
                Debug.LogError($"[CreatureManager] CropData.CreatureId가 비어있습니다: {cropId}");
                return null;
            }

            var creatureData = _registry.FindCreature(cropData.CreatureId);
            if (creatureData == null)
            {
                Debug.LogError($"[CreatureManager] 매칭되는 CreatureData가 없습니다: {cropData.CreatureId}");
                return null;
            }

            if (!_barn.HasFreeSlot()) return null;

            if (_inventory.GetCropCount(cropId) <= 0) return null;
            if (!_inventory.RemoveCrop(cropId, 1)) return null;

            var instance = new CreatureInstance(creatureData.CreatureId);
            if (!_barn.AddCreature(instance))
            {
                _inventory.AddCrop(cropId, 1);
                Debug.LogError("[CreatureManager] 사육장 추가 실패. 작물 환불 처리.");
                return null;
            }

            return instance;
        }
    }
}
