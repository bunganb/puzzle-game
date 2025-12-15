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
        [SerializeField] private float correctDistance = 0.5f; // toleransi slot

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

            Collider2D hit = Physics2D.OverlapPoint(mouse, frameMask);
            if (hit == null)
            {
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                transform.position = originalPos;
                return;
            }

            var slot = hit.GetComponent<View.DvdSlot2D>();
            if (slot == null)
            {
                // kena area lain yang bukan slot
                PuzzleManager.Instance.TryPlacePiece(pieceId, "Outside");
                transform.position = originalPos;
                return;
            }

            // coba taruh ke slot
            bool ok = PuzzleManager.Instance.TryPlacePiece(pieceId, slot.SlotId);

            if (ok)
            {
                // snap ke posisi slot
                transform.position = slot.transform.position;
            }
            else
            {
                // invalid (warna salah / slot penuh / dll)
                transform.position = originalPos;
            }
        }

    }
}