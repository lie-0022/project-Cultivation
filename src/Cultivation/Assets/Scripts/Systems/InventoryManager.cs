using System;
using System.Collections.Generic;

namespace Cultivation.Systems
{
    /// <summary>
    /// 씨앗과 작물의 수량 기반 인벤토리. 크리처는 BarnManager가 관리하므로 여기서 다루지 않는다.
    /// MVP는 무제한 용량.
    /// </summary>
    public class InventoryManager
    {
        private readonly Dictionary<string, int> _seeds = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _crops = new Dictionary<string, int>();

        /// <summary>씨앗 수량이 변경되었을 때 발생. (seedId, newCount). newCount가 0이면 제거된 상태.</summary>
        public event Action<string, int> OnSeedChanged;

        /// <summary>작물 수량이 변경되었을 때 발생. (cropId, newCount). newCount가 0이면 제거된 상태.</summary>
        public event Action<string, int> OnCropChanged;

        public IReadOnlyDictionary<string, int> Seeds => _seeds;
        public IReadOnlyDictionary<string, int> Crops => _crops;

        public int GetSeedCount(string seedId)
        {
            return _seeds.TryGetValue(seedId, out int c) ? c : 0;
        }

        public int GetCropCount(string cropId)
        {
            return _crops.TryGetValue(cropId, out int c) ? c : 0;
        }

        public void AddSeed(string seedId, int count = 1)
        {
            AddToDict(_seeds, seedId, count, OnSeedChanged);
        }

        public bool RemoveSeed(string seedId, int count = 1)
        {
            return RemoveFromDict(_seeds, seedId, count, OnSeedChanged);
        }

        public void AddCrop(string cropId, int count = 1)
        {
            AddToDict(_crops, cropId, count, OnCropChanged);
        }

        public bool RemoveCrop(string cropId, int count = 1)
        {
            return RemoveFromDict(_crops, cropId, count, OnCropChanged);
        }

        private static void AddToDict(Dictionary<string, int> dict, string id, int count, Action<string, int> evt)
        {
            if (count <= 0) return;
            if (!dict.ContainsKey(id)) dict[id] = 0;
            dict[id] += count;
            evt?.Invoke(id, dict[id]);
        }

        private static bool RemoveFromDict(Dictionary<string, int> dict, string id, int count, Action<string, int> evt)
        {
            if (count <= 0) return false;
            if (!dict.TryGetValue(id, out int current) || current < count) return false;

            int next = current - count;
            if (next == 0)
            {
                dict.Remove(id);
                evt?.Invoke(id, 0);
            }
            else
            {
                dict[id] = next;
                evt?.Invoke(id, next);
            }
            return true;
        }
    }
}
