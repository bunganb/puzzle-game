using UnityEngine;
using Managers;
namespace View
{
    public class DropTarget2D : MonoBehaviour
    {
        public string targetId;  // misalnya "Frame"

        public bool TryHandleDrop(DraggablePiece2D piece)
        {
            // cuma lapor ke PuzzleManager, tidak memindahkan posisi
            bool accepted = PuzzleManager.Instance
                .TryPlacePiece(piece.pieceId, targetId);

            return accepted;
        }
    }
}