namespace Abstracts.Input
{
    public interface IPlayerInput
    {
        float HorizontalAxis { get; }
        bool IsJumpButtonDown { get; }
        bool IsJumpButton { get; }
        bool IsDownButton { get; }
        bool IsInteractButton { get; }
        bool IsExitButton { get; }

        // 🟩 Add these two lines ↓↓↓
        bool IsGrowButtonDown { get; }
        bool IsShrinkButtonDown { get; }
    }
}
