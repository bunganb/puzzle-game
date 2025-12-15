using UnityEngine;
using System.Collections.Generic;
namespace Data
{
    [CreateAssetMenu(menuName = "Game/PackingPuzzleData")]
    public class PackingPuzzleData : ScriptableObject
    {
        [System.Serializable]
        public class DvdData
        {
            public string pieceId;
            public GameObject prefab;
            public Vector2 startPosition;
            public Vector2 targetPosition;   // POSISI BENAR di layout
        }

        public List<DvdData> pieces = new List<DvdData>();

        public DvdData GetPiece(string id)
        {
            return pieces.Find(p => p.pieceId == id);
        }
    }

}