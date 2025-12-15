using Core;
using Data;
using System.Collections.Generic;
using System.Linq;

public class DvdSortingRule : IPuzzleRule
{
    private readonly DvdSortingData _data;

    // slotId -> pieceId
    private readonly Dictionary<string, string> _slotToPiece = new();

    public bool IsCompleted { get; private set; }

    public DvdSortingRule(DvdSortingData data)
    {
        _data = data;
    }

    public bool TryPlacePiece(string pieceId, string targetId)
    {
        // Outside means remove from board (optional)
        if (targetId == "Outside")
        {
            // kalau piece sedang ada di slot, keluarkan
            var kv = _slotToPiece.FirstOrDefault(x => x.Value == pieceId);
            if (!string.IsNullOrEmpty(kv.Key))
                _slotToPiece.Remove(kv.Key);

            RecheckCompleted();
            return true;
        }

        // targetId format: "{bookId}_Slot_{index}"
        string[] parts = targetId.Split("_Slot_");
        if (parts.Length != 2) return false;

        string bookId = parts[0];

        var piece = _data.GetPiece(pieceId);
        var book = _data.GetBook(bookId);

        if (piece == null || book == null) return false;

        // 1) cek warna cocok
        if (piece.color != book.color) return false;

        // 2) cek slot kosong
        if (_slotToPiece.ContainsKey(targetId)) return false;

        // 3) kalau piece sebelumnya sudah ada di slot lain, pindahkan (opsional)
        var prev = _slotToPiece.FirstOrDefault(x => x.Value == pieceId);
        if (!string.IsNullOrEmpty(prev.Key))
            _slotToPiece.Remove(prev.Key);

        // place
        _slotToPiece[targetId] = pieceId;

        RecheckCompleted();
        return true;
    }

    private void RecheckCompleted()
    {
        // WIN: semua book terisi 8 slot dan huruf dalam tiap book urut
        foreach (var book in _data.books)
        {
            // ambil semua slot untuk book ini
            var bookSlots = _slotToPiece
                .Where(x => x.Key.StartsWith(book.bookId + "_Slot_"))
                .ToList();

            if (bookSlots.Count != book.slotCount) { IsCompleted = false; return; }

            // urutkan berdasarkan slot index supaya urutan visual dinilai
            var lettersInOrder = bookSlots
                .OrderBy(x => ExtractSlotIndex(x.Key, book.bookId))
                .Select(x => _data.GetPiece(x.Value).letter)
                .ToList();

            // cek apakah sudah non-decreasing (A..Z)
            for (int i = 1; i < lettersInOrder.Count; i++)
            {
                if (lettersInOrder[i] < lettersInOrder[i - 1])
                {
                    IsCompleted = false;
                    return;
                }
            }
        }

        IsCompleted = true;
    }

    private int ExtractSlotIndex(string slotId, string bookId)
    {
        // slotId: "Book_Red_Slot_3"
        string prefix = bookId + "_Slot_";
        string num = slotId.Substring(prefix.Length);
        return int.TryParse(num, out int idx) ? idx : 999;
    }
}
