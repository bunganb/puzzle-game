using UnityEngine;
using System.Collections.Generic;
namespace Data
{
    [CreateAssetMenu(menuName = "Game/PackingPuzzleData")]
    public class PackingPuzzleData : ScriptableObject
    {
        [System.Serializable]
        public class PieceData
        {
            public string pieceId;
            public GameObject prefab;
            public Vector2 startPosition;
            public Vector2 targetPosition;   // POSISI BENAR di layout
        }

        public List<PieceData> pieces = new List<PieceData>();

        public PieceData GetPiece(string id)
        {
            return pieces.Find(p => p.pieceId == id);
        }
    }

}