using UnityEngine;
using Managers;
namespace View
{
    public class DraggablePiece2D : MonoBehaviour
    {
        public string pieceId;

        private bool dragging;
        private Vector3 offset;
        private Vector3 originalPos;
        private Vector3 targetPos;

        [SerializeField] private LayerMask frameMask;   // HANYA layer FrameArea
        [SerializeField] private float correctDistance = 0.15f; // toleransi slot

        public void Setup(string id, Vector2 targetPosition)
        {
            pieceId = id;
            targetPos = targetPosition;
            originalPos = transform.position;
        }

        private void OnMouseDown()
        {
            dragging = true;
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            offset = transform.position - mouse;
        }

        private void OnMouseDrag()
        {
            if (!dragging) return;

            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            transform.position = mouse + offset;
        }

        private void OnMouseUp()
        {
            dragging = false;

            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;

            // 1️⃣ Apakah di dalam frame?
            Collider2D hit = Physics2D.OverlapPoint(mouse, frameMask);
            bool insideFrame = hit != null;

            // 2️⃣ Hitung jarak ke posisi target
            float distToTarget = Vector3.Distance(transform.position, targetPos);
            bool isCorrectSlot = insideFrame && distToTarget <= correctDistance;

            if (!insideFrame)
            {
                // Di luar kotak
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                // optional: balik ke posisi awal
                transform.position = originalPos;
            }
            else if (isCorrectSlot)
            {
                // Di dalam kotak & dekat target → snap ke posisi benar
                transform.position = targetPos;
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Correct");
            }
            else
            {
                // Di dalam frame tapi BELUM di slot tepat
                PuzzleManager.Instance.TryPlacePiece(pieceId, "InFrameWrong");
                // Di sini kamu bisa:
                // - biarkan di posisi sekarang, atau
                // - snap ke grid, terserah desain
            }
        }
    }
}