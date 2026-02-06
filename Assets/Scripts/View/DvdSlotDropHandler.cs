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

        if (pieceCollider == null)
        {
            Debug.LogError("Piece collider is null!");
            return false;
        }

        // ✅ Gunakan bounds.center untuk deteksi lebih akurat
        Vector3 pieceCenter = pieceCollider.bounds.center;

        // Cari slot di posisi drop
        Collider2D hit = Physics2D.OverlapPoint(pieceCenter, _slotMask);
        
        if (hit == null)
        {
            // Tidak ada slot di posisi ini
            PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
            return false;
        }

        var slot = hit.GetComponent<DvdSlot2D>();
        if (slot == null)
        {
            Debug.LogWarning($"Collider found but no DvdSlot2D component!");
            PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
            return false;
        }

        // ✅ Try place di slot
        bool success = PuzzleManager.Instance.TryPlacePiece(pieceId, slot.SlotId);
        
        if (success)
        {
            // ✅ Snap ke posisi slot
            snapPos = slot.transform.position;
            AudioManager.Instance?.placeSound();
            return true;
        }
        else
        {
            // ❌ Gagal (wrong color atau slot occupied)
            // Piece balik ke posisi awal
            return false;
        }
    }
}