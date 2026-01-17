using Core;
using UnityEngine;
using Managers;
using View;

public class DvdSlotDropHandler : IDropHandler2D
{
    private readonly LayerMask _slotMask;

    public DvdSlotDropHandler(LayerMask slotMask)
    {
        _slotMask = slotMask;
    }

    public bool TryDrop(
        string pieceId,
        Collider2D pieceCollider,
        Vector3 dropWorldPos,
        out Vector3 snapPos)
    {
        snapPos = dropWorldPos;

        Collider2D hit = Physics2D.OverlapPoint(dropWorldPos, _slotMask);
        if (hit == null)
        {
            PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
            return false;
        }

        var slot = hit.GetComponent<DvdSlot2D>();
        if (slot == null)
        {
            PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
            return false;
        }

        bool ok = PuzzleManager.Instance.TryPlacePiece(pieceId, slot.SlotId);
        if (!ok) return false;

        snapPos = slot.transform.position;
        return true;
    }
}