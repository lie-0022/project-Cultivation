using UnityEngine;
using UnityEngine.InputSystem;

namespace Cultivation.Systems
{
    /// <summary>
    /// 3인칭 캐릭터 이동 컨트롤러. WASD는 카메라 yaw 기준 상대 방향으로 적용되며, Space로 점프.
    /// UI 모드(GameManager.IsUIModeActive=true)일 때 입력을 차단한다.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private GameManager _gameManager;

        private CharacterController _controller;
        private Vector3 _velocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_gameManager != null && _gameManager.IsUIModeActive) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

            Vector3 forward;
            Vector3 right;
            if (_cameraTransform != null)
            {
                forward = _cameraTransform.forward;
                right = _cameraTransform.right;
                forward.y = 0f; right.y = 0f;
                forward.Normalize(); right.Normalize();
            }
            else
            {
                forward = transform.forward;
                right = transform.right;
            }

            Vector3 horizontal = (forward * z + right * x).normalized * _moveSpeed;

            if (_controller.isGrounded)
            {
                if (_velocity.y < 0f) _velocity.y = -2f;
                if (kb.spaceKey.wasPressedThisFrame)
                    _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }
            _velocity.y += _gravity * Time.deltaTime;

            Vector3 motion = horizontal + new Vector3(0f, _velocity.y, 0f);
            _controller.Move(motion * Time.deltaTime);

            if (horizontal.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(horizontal.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, _rotationSpeed * Time.deltaTime);
            }
        }
    }
}
