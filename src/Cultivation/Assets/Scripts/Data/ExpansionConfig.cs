using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 밭/사육장 확장 비용 공식 파라미터. 공식: nextCost = RoundToInt(baseCost * Pow(currentCount, multiplier)).
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Expansion Config", fileName = "ExpansionConfig")]
    public class ExpansionConfig : ScriptableObject
    {
        [SerializeField] private int _farmBaseCost = 100;
        [SerializeField] private int _barnBaseCost = 200;
        [SerializeField] private float _costMultiplier = 1.5f;

        public int FarmBaseCost => _farmBaseCost;
        public int BarnBaseCost => _barnBaseCost;
        public float CostMultiplier => _costMultiplier;

        /// <summary>현재 칸 수 기준 다음 확장 비용 계산.</summary>
        public int CalculateCost(int baseCost, int currentCount)
        {
            return Mathf.RoundToInt(baseCost * Mathf.Pow(currentCount, _costMultiplier));
        }
    }
}
