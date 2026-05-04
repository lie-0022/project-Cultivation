using UnityEngine;

namespace Cultivation.Data
{
    /// <summary>
    /// 작물 정의. 밭에서 수확되며 직접 판매하거나 크리처로 변환할 수 있다.
    /// </summary>
    [CreateAssetMenu(menuName = "Cultivation/Data/Crop", fileName = "Crop_New")]
    public class CropData : ScriptableObject
    {
        [SerializeField] private string _cropId;
        [SerializeField] private string _cropName;
        [SerializeField] private string _creatureId;
        [SerializeField] private int _sellPrice;

        public string CropId => _cropId;
        public string CropName => _cropName;
        public string CreatureId => _creatureId;
        public int SellPrice => _sellPrice;
    }
}
