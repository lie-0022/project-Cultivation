using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 씨앗 정의. 가챠로 획득되어 밭에 심으면 일정 시간 후 작물로 자란다.
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Seed", fileName = "Seed_New")]
    public class SeedData : ScriptableObject
    {
        [SerializeField] private string _seedId;
        [SerializeField] private string _seedName;
        [SerializeField] private float _growthTime;
        [SerializeField] private string _resultCropId;
        [SerializeField] private Rarity _rarity;

        public string SeedId => _seedId;
        public string SeedName => _seedName;
        public float GrowthTime => _growthTime;
        public string ResultCropId => _resultCropId;
        public Rarity Rarity => _rarity;
    }
}
