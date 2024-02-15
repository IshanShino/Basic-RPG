using UnityEngine;

namespace RPG.Core
{
    [CreateAssetMenu(fileName = "New CursorType", menuName = "CursorType", order = 0)]
    public class CursorType : ScriptableObject 
    {
        [SerializeField] Texture2D texture2D;
        [SerializeField] Vector2 hotSpot;

        public void SetCursor()
        {
            Cursor.SetCursor(texture2D, hotSpot, CursorMode.Auto);
        }
    }
}