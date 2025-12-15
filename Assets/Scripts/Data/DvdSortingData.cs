using UnityEngine;
using System.Collections.Generic;

namespace Data
{
    public enum BookColor { Red, Blue, Green, Yellow }

    [CreateAssetMenu(menuName = "Game/DvdSorting Data")]
    public class DvdSortingData : ScriptableObject
    {
        [System.Serializable]
        public class BookData
        {
            public string bookId;          // "Book_Red"
            public BookColor color;        // Red
            public int slotCount = 8;      // 8 slot
        }

        [System.Serializable]
        public class PieceData
        {
            public string pieceId;         // "DVD_A_Red_01"
            public GameObject prefab;
            public Vector2 startPosition;

            public BookColor color;        // warna kaset
            public char letter;            // huruf kaset: A,B,C...
        }

        public List<BookData> books = new List<BookData>();
        public List<PieceData> pieces = new List<PieceData>();

        public PieceData GetPiece(string id) => pieces.Find(p => p.pieceId == id);
        public BookData GetBook(string id) => books.Find(b => b.bookId == id);
    }
}