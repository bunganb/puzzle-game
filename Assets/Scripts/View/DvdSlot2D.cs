using UnityEngine;

namespace View
{
    public class DvdSlot2D : MonoBehaviour
    {
        public string bookId;     // "Book_Red"
        public int slotIndex;     // 0..7

        public string SlotId => $"{bookId}_Slot_{slotIndex}";
    }
}