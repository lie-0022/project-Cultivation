using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 밭 플롯 시설물. plotIndex로 FarmManager의 해당 플롯에 매핑된다.
    /// MVP의 OnInteract는 UI 모드 ON + 디버그 로그만 처리(Phase 5에서 씨앗 선택/수확 UI 연동).
    /// </summary>
    public class FarmPlotInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private int _plotIndex;
        [SerializeField] private float _interactionRange = 2f;

        public int PlotIndex => _plotIndex;
        public Vector3 Position => transform.position;
        public float InteractionRange => _interactionRange;
        public string PromptText => $"밭 #{_plotIndex + 1}";
        public bool CanInteract => true;

        public void OnInteract(GameManager gm)
        {
            Debug.Log($"[FarmPlot {_plotIndex}] 상호작용 (Phase 5에서 씨앗 선택/수확 UI 연결 예정).");
            if (gm != null) gm.SetUIModeActive(true);
        }
    }
}
