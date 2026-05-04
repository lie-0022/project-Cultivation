using Cultivation.Data;
using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 가챠 시스템. 골드 차감 + 가중치 추첨 + 인벤토리 추가를 한 단위로 처리.
    /// </summary>
    public class GachaManager
    {
        private readonly GachaConfig _config;
        private readonly EconomyManager _economy;
        private readonly InventoryManager _inventory;

        public GachaManager(GachaConfig config, EconomyManager economy, InventoryManager inventory)
        {
            _config = config;
            _economy = economy;
            _inventory = inventory;
        }

        public int PullCost => _config != null ? _config.PullCost : 0;

        /// <summary>1회 뽑기. 성공 시 result에 추첨된 씨앗을 채우고 인벤토리 추가까지 완료.</summary>
        public bool TryPull(out SeedData result)
        {
            result = null;
            if (_config == null || _config.Entries == null || _config.Entries.Count == 0)
            {
                Debug.LogError("[GachaManager] GachaConfig가 비어있어 뽑기를 진행할 수 없습니다.");
                return false;
            }

            float totalWeight = 0f;
            for (int i = 0; i < _config.Entries.Count; i++)
                totalWeight += _config.Entries[i].Weight;

            if (totalWeight <= 0f)
            {
                Debug.LogError("[GachaManager] 가중치 합이 0 이하입니다.");
                return false;
            }

            if (!_economy.TrySpendGold(_config.PullCost))
            {
                return false;
            }

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            SeedData picked = null;
            for (int i = 0; i < _config.Entries.Count; i++)
            {
                var entry = _config.Entries[i];
                cumulative += entry.Weight;
                if (roll < cumulative)
                {
                    picked = entry.Seed;
                    break;
                }
            }

            if (picked == null)
            {
                Debug.LogError("[GachaManager] 추첨에 실패했습니다(매칭되는 엔트리 없음).");
                return false;
            }

            _inventory.AddSeed(picked.SeedId);
            result = picked;
            return true;
        }
    }
}
