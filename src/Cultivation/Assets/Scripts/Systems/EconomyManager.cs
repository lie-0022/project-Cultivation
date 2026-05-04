using System;

namespace Cultivation.Systems
{
    /// <summary>
    /// 골드 자원 관리. 가챠/판매/확장 비용의 가감을 처리한다.
    /// 실제 판매 흐름(작물/크리처 차감)은 호출자에서 조합하여 사용.
    /// </summary>
    public class EconomyManager
    {
        private int _gold;

        /// <summary>골드가 변경되었을 때 발생. (newGold)</summary>
        public event Action<int> OnGoldChanged;

        public int Gold => _gold;

        public EconomyManager(int startingGold)
        {
            _gold = startingGold;
        }

        /// <summary>골드를 증가시킨다.</summary>
        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            _gold += amount;
            OnGoldChanged?.Invoke(_gold);
        }

        /// <summary>지정 금액을 차감한다. 부족하면 false 리턴, 골드 변동 없음.</summary>
        public bool TrySpendGold(int amount)
        {
            if (amount <= 0) return false;
            if (_gold < amount) return false;

            _gold -= amount;
            OnGoldChanged?.Invoke(_gold);
            return true;
        }
    }
}
