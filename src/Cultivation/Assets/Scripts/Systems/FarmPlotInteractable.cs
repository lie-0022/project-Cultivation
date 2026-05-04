using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 밭 플롯 시설물. OnInteract 시 FarmPlotPanel을 해당 plotIndex로 엽니다.
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
            gm?.UI?.OpenFarmPlotPanel(_plotIndex);
        }
    }
}
