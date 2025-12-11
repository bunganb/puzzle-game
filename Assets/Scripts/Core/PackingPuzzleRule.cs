using System.Collections.Generic;
using Data;

namespace Core
{
    public class PackingPuzzleRule : IPuzzleRule
    {
        private PackingPuzzleData _data;
        private HashSet<string> correctPieces = new HashSet<string>();

        public bool IsCompleted { get; private set; }

        public PackingPuzzleRule(PackingPuzzleData data)
        {
            _data = data;
        }

        public bool TryPlacePiece(string pieceId, string state)
        {
            // pastikan piece valid
            bool exists = _data.pieces.Exists(p => p.pieceId == pieceId);
            if (!exists) return false;

            switch (state)
            {
                case "Correct":
                    correctPieces.Add(pieceId);
                    break;

                case "InFrameWrong":
                case "Outside":
                    // kalau dia keluar slot atau keluar frame,
                    // kita anggap tidak correct lagi
                    correctPieces.Remove(pieceId);
                    break;
            }

            IsCompleted = (correctPieces.Count == _data.pieces.Count);
            return true;
        }
    }
}