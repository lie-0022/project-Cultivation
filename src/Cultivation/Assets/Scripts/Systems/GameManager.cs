using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 게임 전체 진입점. 모든 매니저의 인스턴스를 보유하고 다른 매니저는 여기를 통해 접근한다.
    /// 싱글톤 정적 참조는 금지(MVP 컨벤션). Scene 안에 단 하나만 존재해야 한다.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("초기 게임 상태")]
        [SerializeField] private int _startingGold = 200;

        public InventoryManager Inventory { get; private set; }
        public EconomyManager Economy { get; private set; }

        private void Awake()
        {
            Inventory = new InventoryManager();
            Economy = new EconomyManager(_startingGold);
        }
    }
}
