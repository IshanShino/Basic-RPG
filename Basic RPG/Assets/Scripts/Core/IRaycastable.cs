using RPG.Control;

namespace RPG.Core
{
    public interface IRaycastable
    {   
        CursorType GetCursorType(PlayerController controller);
        bool HandleRaycast(PlayerController controller);
    }
}