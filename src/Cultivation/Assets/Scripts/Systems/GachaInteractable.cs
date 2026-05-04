using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 가챠 빌딩 시설물. OnInteract 시 GachaPanel을 엽니다.
    /// </summary>
    public class GachaInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _interactionRange = 2.5f;

        public Vector3 Position => transform.position;
        public float InteractionRange => _interactionRange;
        public string PromptText => "가챠";
        public bool CanInteract => true;

        public void OnInteract(GameManager gm)
        {
            gm?.UI?.OpenGachaPanel();
        }
    }
}
