using UnityEngine;
using UnityEngine.InputSystem;

namespace Cultivation.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpHeight = 1.5f;
        [SerializeField] private float _gravity = -20f;

        private CharacterController _controller;
        private Vector3 _velocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
            float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
            Vector3 horizontal = new Vector3(x, 0f, z).normalized * _moveSpeed;

            if (_controller.isGrounded)
            {
                if (_velocity.y < 0f) _velocity.y = -2f;
                if (kb.spaceKey.wasPressedThisFrame)
                    _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            _velocity.y += _gravity * Time.deltaTime;

            Vector3 motion = horizontal + new Vector3(0f, _velocity.y, 0f);
            _controller.Move(motion * Time.deltaTime);
        }
    }
}
