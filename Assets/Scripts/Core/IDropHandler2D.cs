using UnityEngine;

namespace Core
{
    public interface IDropHandler2D
    {
        bool TryDrop(
            string pieceId,
            Collider2D pieceCollider,
            Vector3 dropWorldPos,
            out Vector3 snapPos
        );
    }
}