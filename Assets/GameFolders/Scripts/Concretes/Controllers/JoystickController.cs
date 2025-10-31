using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Abstracts.Input;

namespace Inputs
{
    public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPlayerInput
    {
        [Header("Joystick Settings")]
        [SerializeField] private RectTransform joystickBG;
        [SerializeField] private RectTransform joystickHandle;
        [SerializeField, Range(50f, 200f)] private float handleRange = 100f;
        [SerializeField, Range(0.2f, 0.6f)] private float jumpThreshold = 0.45f;

        [Header("Action Buttons")]
        [SerializeField] private Button jumpButton;
        [SerializeField] private Button growButton;
        [SerializeField] private Button shrinkButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private Button exitButton;

        private Vector2 inputVector;
        private bool isJumpPressed;
        private bool isGrowPressed;
        private bool isShrinkPressed;
        private bool isInteractPressed;
        private bool isExitPressed;
        private bool hasJumpedThisSwipe;

        private void Start()
        {
            // ✅ Hook all buttons safely
            AddButtonListener(jumpButton, OnJumpPressed);
            AddButtonListener(growButton, OnGrowPressed);
            AddButtonListener(shrinkButton, OnShrinkPressed);
            AddButtonListener(interactButton, OnInteractPressed);
            AddButtonListener(exitButton, OnExitPressed);
        }

        private void AddButtonListener(Button button, System.Action action)
        {
            if (button != null)
                button.onClick.AddListener(() => action?.Invoke());
        }

        #region --- Button Press Handlers ---
        private void OnJumpPressed() => isJumpPressed = true;
        private void OnGrowPressed() => isGrowPressed = true;
        private void OnShrinkPressed() => isShrinkPressed = true;
        private void OnInteractPressed() => isInteractPressed = true;
        private void OnExitPressed() => isExitPressed = true;
        #endregion

        // 🔹 Helper to consume one-time button events (like GetButtonDown)
        private bool Consume(ref bool flag)
        {
            if (flag)
            {
                flag = false;
                return true;
            }
            return false;
        }

        #region --- IPlayerInput Interface ---
        public float HorizontalAxis => inputVector.x;
        public bool IsJumpButtonDown => Consume(ref isJumpPressed);
        public bool IsJumpButton => isJumpPressed;
        public bool IsDownButton => false;
        public bool IsInteractButton => Consume(ref isInteractPressed);
        public bool IsExitButton => Consume(ref isExitPressed);
        public bool IsGrowButtonDown => Consume(ref isGrowPressed);
        public bool IsShrinkButtonDown => Consume(ref isShrinkPressed);
        #endregion

        #region --- Joystick Logic ---
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                joystickBG, eventData.position, eventData.pressEventCamera, out pos);

            pos = Vector2.ClampMagnitude(pos, handleRange);
            joystickHandle.anchoredPosition = pos;
            inputVector = pos / handleRange;

            // 🔹 Trigger jump when pushing joystick upward
            if (!hasJumpedThisSwipe && inputVector.y > jumpThreshold)
            {
                isJumpPressed = true;
                hasJumpedThisSwipe = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.zero;
            hasJumpedThisSwipe = false;
        }
        #endregion
    }
}
