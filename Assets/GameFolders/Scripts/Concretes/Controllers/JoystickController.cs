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

        [Header("Swipe Thresholds")]
        [SerializeField, Range(0.2f, 0.6f)] private float jumpThreshold = 0.45f;
        [SerializeField, Range(-0.6f, -0.2f)] private float downThreshold = -0.45f; // 🟩 new

        [Header("Action Buttons")]
        [SerializeField] private Button jumpButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button downButton; // optional UI button

        [Header("Hold Buttons (Grow/Shrink)")]
        [SerializeField] private EventTrigger growTrigger;
        [SerializeField] private EventTrigger shrinkTrigger;

        private Vector2 inputVector;
        private bool isJumpPressed;
        private bool isInteractPressed;
        private bool isExitPressed;
        private bool isDownPressed; // joystick OR button
        private bool hasJumpedThisSwipe;
        private bool hasDownTriggeredThisSwipe;

        private bool _isGrowHeld;
        private bool _isShrinkHeld;

        private void Start()
        {
            AddButtonListener(jumpButton, () => isJumpPressed = true);
            AddButtonListener(interactButton, () => isInteractPressed = true);
            AddButtonListener(exitButton, () => isExitPressed = true);
            AddButtonListener(downButton, () => isDownPressed = true); // optional

            // grow/shrink event setup
            AddHoldEvents(growTrigger, () => _isGrowHeld = true, () => _isGrowHeld = false);
            AddHoldEvents(shrinkTrigger, () => _isShrinkHeld = true, () => _isShrinkHeld = false);
        }

        private void AddButtonListener(Button button, System.Action action)
        {
            if (button != null)
                button.onClick.AddListener(() => action?.Invoke());
        }

        private void AddHoldEvents(EventTrigger trigger, System.Action onDown, System.Action onUp)
        {
            if (trigger == null) return;

            var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            down.callback.AddListener((_) => onDown?.Invoke());
            trigger.triggers.Add(down);

            var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            up.callback.AddListener((_) => onUp?.Invoke());
            trigger.triggers.Add(up);
        }

        private bool Consume(ref bool flag)
        {
            if (flag)
            {
                flag = false;
                return true;
            }
            return false;
        }

        public void OnGrowButtonDown() => _isGrowHeld = true;
        public void OnGrowButtonUp() => _isGrowHeld = false;
        public void OnShrinkButtonDown() => _isShrinkHeld = true;
        public void OnShrinkButtonUp() => _isShrinkHeld = false;

        #region --- IPlayerInput Interface ---
        public float HorizontalAxis => inputVector.x;
        public bool IsJumpButtonDown => Consume(ref isJumpPressed);
        public bool IsJumpButton => isJumpPressed;
        public bool IsDownButton => Consume(ref isDownPressed); // both button + joystick down
        public bool IsInteractButton => Consume(ref isInteractPressed);
        public bool IsExitButton => Consume(ref isExitPressed);

        public bool IsGrowButtonHeld => _isGrowHeld;
        public bool IsShrinkButtonHeld => _isShrinkHeld;
        public bool IsGrowButtonDown => _isGrowHeld;
        public bool IsShrinkButtonDown => _isShrinkHeld;
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

            // Jump swipe detection
            if (!hasJumpedThisSwipe && inputVector.y > jumpThreshold)
            {
                isJumpPressed = true;
                hasJumpedThisSwipe = true;
            }

            // 🟩 Down swipe detection
            if (!hasDownTriggeredThisSwipe && inputVector.y < downThreshold)
            {
                isDownPressed = true;
                hasDownTriggeredThisSwipe = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.zero;
            hasJumpedThisSwipe = false;
            hasDownTriggeredThisSwipe = false;
        }
        #endregion
    }
}
