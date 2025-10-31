using UnityEngine;
using Movements;

namespace Mechanics
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(RbMovement))]
    public class PlayerSizeControl : MonoBehaviour
    {
        [Header("Continuous Size Control Settings")]
        [Tooltip("Minimum size multiplier relative to initial scale.")]
        public float minScaleMultiplier = 0.6f;

        [Tooltip("Maximum size multiplier relative to initial scale.")]
        public float maxScaleMultiplier = 1.8f;

        [Tooltip("How fast the player grows or shrinks while held.")]
        public float scaleChangeSpeed = 1.5f;

        [Tooltip("How fast physics stats adapt to new size.")]
        public float physicsResponseSpeed = 4f;

        private Rigidbody2D _rb;
        private RbMovement _movement;

        private Vector3 _initialScale;
        private float _initialMass;
        private float _initialJumpForce;
        private float _initialMoveSpeed;

        private float _currentScaleMultiplier = 1f;
        private float _targetScaleMultiplier = 1f;

        private bool _isGrowPressed = false;
        private bool _isShrinkPressed = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _movement = GetComponent<RbMovement>();

            // Store initial physical stats and scale
            _initialScale = transform.localScale;
            _initialMass = _rb.mass;
            _initialJumpForce = GetPrivateField<float>(_movement, "_jumpForce");
            _initialMoveSpeed = GetPrivateField<float>(_movement, "_horizontalSpeed");
        }

        private void Update()
        {
            HandleInput();
            ApplyScaling();
        }

        private void LateUpdate()
        {
            // Prevent scale reset from animation or physics updates
            transform.localScale = _initialScale * _currentScaleMultiplier;
        }

        private void HandleInput()
        {
            // ✅ PC keyboard input
            bool growHeld = Input.GetKey(KeyCode.G) || _isGrowPressed;
            bool shrinkHeld = Input.GetKey(KeyCode.H) || _isShrinkPressed;

            if (growHeld && !shrinkHeld)
                _targetScaleMultiplier += Time.deltaTime * scaleChangeSpeed;
            else if (shrinkHeld && !growHeld)
                _targetScaleMultiplier -= Time.deltaTime * scaleChangeSpeed;
            else
                _targetScaleMultiplier = Mathf.Lerp(_targetScaleMultiplier, 1f, Time.deltaTime * scaleChangeSpeed);

            _targetScaleMultiplier = Mathf.Clamp(_targetScaleMultiplier, minScaleMultiplier, maxScaleMultiplier);
        }

        private void ApplyScaling()
        {
            // Smooth transition
            _currentScaleMultiplier = Mathf.Lerp(_currentScaleMultiplier, _targetScaleMultiplier, Time.deltaTime * physicsResponseSpeed);

            // Apply visual scale
            transform.localScale = _initialScale * _currentScaleMultiplier;

            // Update mass based on area scaling
            float targetMass = _initialMass * Mathf.Pow(_currentScaleMultiplier, 2);
            _rb.mass = Mathf.Lerp(_rb.mass, targetMass, Time.deltaTime * physicsResponseSpeed);

            // Adjust speed & jump
            float targetMoveSpeed = _initialMoveSpeed / _currentScaleMultiplier;
            float targetJumpForce = _initialJumpForce / _currentScaleMultiplier;

            SetPrivateField(_movement, "_horizontalSpeed",
                Mathf.Lerp(GetPrivateField<float>(_movement, "_horizontalSpeed"), targetMoveSpeed, Time.deltaTime * physicsResponseSpeed));

            SetPrivateField(_movement, "_jumpForce",
                Mathf.Lerp(GetPrivateField<float>(_movement, "_jumpForce"), targetJumpForce, Time.deltaTime * physicsResponseSpeed));
        }

        // 🔹 PUBLIC METHODS for JoystickController buttons
        public void Grow()
        {
            _isGrowPressed = true;
            _isShrinkPressed = false;
            _targetScaleMultiplier += Time.deltaTime * scaleChangeSpeed * 5f; // instant button press has stronger effect
        }

        public void Shrink()
        {
            _isShrinkPressed = true;
            _isGrowPressed = false;
            _targetScaleMultiplier -= Time.deltaTime * scaleChangeSpeed * 5f;
        }

        // 🔹 Reset flags (Joystick button release can call this)
        public void StopSizeChange()
        {
            _isGrowPressed = false;
            _isShrinkPressed = false;
        }

        // --- Reflection Helpers ---
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T)field.GetValue(obj);
        }

        private void SetPrivateField<T>(object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(obj, value);
        }
    }
}
