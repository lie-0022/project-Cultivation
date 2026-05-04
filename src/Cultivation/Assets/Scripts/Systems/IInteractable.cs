using UnityEngine;

namespace Cultivation.Systems
{
    /// <summary>
    /// 상호작용 가능한 시설물/오브젝트가 구현하는 계약. InteractionController가 IInteractable을 스캔하여
    /// 가장 가까운 1개를 선택하고 E키 입력 시 OnInteract를 호출한다.
    /// </summary>
    public interface IInteractable
    {
        Vector3 Position { get; }
        float InteractionRange { get; }
        string PromptText { get; }
        bool CanInteract { get; }
        void OnInteract(GameManager gm);
    }
}
