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

        // Flags controlled by buttons or keyboard
        private bool _isGrowHeld = false;
        private bool _isShrinkHeld = false;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _movement = GetComponent<RbMovement>();

            // Cache initial scale and stats
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
            // Prevent animator or physics from overwriting scale
            transform.localScale = _initialScale * _currentScaleMultiplier;
        }

        private void HandleInput()
        {
            // ✅ Combine PC keyboard & mobile button flags
            bool growHeld = Input.GetKey(KeyCode.G) || _isGrowHeld;
            bool shrinkHeld = Input.GetKey(KeyCode.H) || _isShrinkHeld;

            if (growHeld && !shrinkHeld)
                _targetScaleMultiplier += Time.deltaTime * scaleChangeSpeed;
            else if (shrinkHeld && !growHeld)
                _targetScaleMultiplier -= Time.deltaTime * scaleChangeSpeed;
            else
                _targetScaleMultiplier = Mathf.Lerp(_targetScaleMultiplier, 1f, Time.deltaTime * scaleChangeSpeed);

            // Clamp within safe range
            _targetScaleMultiplier = Mathf.Clamp(_targetScaleMultiplier, minScaleMultiplier, maxScaleMultiplier);
        }

        private void ApplyScaling()
        {
            // Smoothly approach target scale
            _currentScaleMultiplier = Mathf.Lerp(_currentScaleMultiplier, _targetScaleMultiplier, Time.deltaTime * physicsResponseSpeed);

            // Apply to transform
            transform.localScale = _initialScale * _currentScaleMultiplier;

            // Adjust mass proportionally to area (scale²)
            float targetMass = _initialMass * Mathf.Pow(_currentScaleMultiplier, 2);
            _rb.mass = Mathf.Lerp(_rb.mass, targetMass, Time.deltaTime * physicsResponseSpeed);

            // Adjust movement and jump inversely to scale
            float targetMoveSpeed = _initialMoveSpeed / _currentScaleMultiplier;
            float targetJumpForce = _initialJumpForce / _currentScaleMultiplier;

            SetPrivateField(_movement, "_horizontalSpeed",
                Mathf.Lerp(GetPrivateField<float>(_movement, "_horizontalSpeed"), targetMoveSpeed, Time.deltaTime * physicsResponseSpeed));

            SetPrivateField(_movement, "_jumpForce",
                Mathf.Lerp(GetPrivateField<float>(_movement, "_jumpForce"), targetJumpForce, Time.deltaTime * physicsResponseSpeed));
        }

        // ✅ Called from UI buttons (PointerDown / PointerUp)
        public void OnGrowPressed(bool isPressed)
        {
            _isGrowHeld = isPressed;
        }

        public void OnShrinkPressed(bool isPressed)
        {
            _isShrinkHeld = isPressed;
        }

        // --- Reflection Helpers (for private RbMovement fields) ---
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
