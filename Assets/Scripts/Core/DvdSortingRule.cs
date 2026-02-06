using Core;
using Data;
using System.Collections.Generic;
using System.Linq;

public class DvdSortingRule : IPuzzleRule
{
    private readonly DvdSortingData _data;
    private readonly Dictionary<string, string> _slotToPiece = new();

    public bool IsCompleted { get; private set; }

    public DvdSortingRule(DvdSortingData data)
    {
        _data = data;
    }

    public bool TryPlacePiece(string pieceId, string targetId)
    {
        // Remove from board
        if (targetId == "Outside")
        {
            RemovePieceFromSlot(pieceId);
            RecheckCompleted();
            return true;
        }

        // Parse targetId format: "{bookId}_Slot_{index}"
        if (!TryParseSlotId(targetId, out string bookId, out int slotIndex))
        {
            UnityEngine.Debug.LogWarning($"Invalid targetId format: {targetId}");
            return false;
        }

        var piece = _data.GetPiece(pieceId);
        var book = _data.GetBook(bookId);

        // Validation
        if (piece == null)
        {
            UnityEngine.Debug.LogError($"Piece not found: {pieceId}");
            return false;
        }

        if (book == null)
        {
            UnityEngine.Debug.LogError($"Book not found: {bookId}");
            return false;
        }

        // ✅ Rule 1: Warna harus cocok
        if (piece.color != book.color)
        {
            UnityEngine.Debug.Log($"❌ Wrong color: {piece.color} != {book.color}");
            return false;
        }

        // ✅ Rule 2: Slot harus kosong
        if (_slotToPiece.ContainsKey(targetId))
        {
            UnityEngine.Debug.Log($"❌ Slot already occupied: {targetId}");
            return false;
        }

        // ✅ Rule 3: Remove piece dari slot lama (kalau ada)
        RemovePieceFromSlot(pieceId);

        // ✅ Place piece di slot baru
        _slotToPiece[targetId] = pieceId;

        RecheckCompleted();
        return true;
    }

    private void RemovePieceFromSlot(string pieceId)
    {
        // Cari slot yang berisi piece ini
        var slot = _slotToPiece.FirstOrDefault(x => x.Value == pieceId).Key;
        if (!string.IsNullOrEmpty(slot))
        {
            _slotToPiece.Remove(slot);
        }
    }

    private bool TryParseSlotId(string slotId, out string bookId, out int slotIndex)
    {
        bookId = null;
        slotIndex = -1;

        string[] parts = slotId.Split(new[] { "_Slot_" }, System.StringSplitOptions.None);
        if (parts.Length != 2)
            return false;

        bookId = parts[0];
        return int.TryParse(parts[1], out slotIndex);
    }

    private void RecheckCompleted()
    {
        // ✅ WIN Condition: Semua book terisi penuh DAN huruf terurut
        foreach (var book in _data.books)
        {
            // Get all slots untuk book ini
            var bookSlots = _slotToPiece
                .Where(x => x.Key.StartsWith(book.bookId + "_Slot_"))
                .ToList();

            // ✅ Check 1: Semua slot harus terisi
            if (bookSlots.Count != book.slotCount)
            {
                IsCompleted = false;
                return;
            }

            // ✅ Check 2: Huruf harus terurut (non-decreasing)
            var lettersInOrder = bookSlots
                .OrderBy(x => ExtractSlotIndex(x.Key, book.bookId))
                .Select(x => _data.GetPiece(x.Value).letter)
                .ToList();

            for (int i = 1; i < lettersInOrder.Count; i++)
            {
                if (lettersInOrder[i] < lettersInOrder[i - 1])
                {
                    IsCompleted = false;
                    return;
                }
            }
        }

        // ✅ Semua book valid!
        IsCompleted = true;
        UnityEngine.Debug.Log("🎉 DVD Sorting Completed!");
    }

    private int ExtractSlotIndex(string slotId, string bookId)
    {
        string prefix = bookId + "_Slot_";
        if (slotId.Length <= prefix.Length)
            return 999;

        string numStr = slotId.Substring(prefix.Length);
        return int.TryParse(numStr, out int idx) ? idx : 999;
    }
}