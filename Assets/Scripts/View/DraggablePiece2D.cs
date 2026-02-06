using UnityEngine;
using Core;
using Data;

namespace View
{
    public class DraggablePiece2D : MonoBehaviour
    {
        public string pieceId;

        [Header("Snap Settings")]
        [SerializeField] private float snapRadius = 0.8f; // Jarak untuk auto-snap
        [SerializeField] private bool showSnapGizmo = true;

        private bool dragging;
        private Vector3 offset;
        private Vector3 originalPos;
        private IDropHandler2D _dropHandler;
        private PackingPuzzleData _packingData; // Untuk cek target position saat drag

        public void Setup(string id, IDropHandler2D dropHandler)
        {
            pieceId = id;
            _dropHandler = dropHandler;
            originalPos = transform.position;

            // Ambil data untuk snap magnetism (hanya untuk PackingPuzzle)
            if (dropHandler is PackingDropHandler)
            {
                var config = GameManager.Instance?.CurrentLevel;
                if (config != null)
                {
                    _packingData = config.packingData;
                }
            }
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
            Vector3 targetPos = mouse + offset;

            // ✅ SNAP MAGNETISM: Cek apakah dekat dengan target position
            if (_packingData != null)
            {
                var pieceData = _packingData.GetPiece(pieceId);
                if (pieceData != null)
                {
                    // Hitung jarak dari center piece ke target
                    Collider2D col = GetComponent<Collider2D>();
                    if (col != null)
                    {
                        // Prediksi center piece di posisi baru
                        Vector3 currentCenter = col.bounds.center;
                        Vector3 offset2 = targetPos - transform.position;
                        Vector3 predictedCenter = currentCenter + offset2;

                        Vector3 targetPos3D = new Vector3(
                            pieceData.targetPosition.x,
                            pieceData.targetPosition.y,
                            0
                        );

                        float dist = Vector3.Distance(predictedCenter, targetPos3D);

                        // ✅ AUTO SNAP saat mendekati!
                        if (dist <= snapRadius)
                        {
                            // Hitung offset pivot untuk snap yang benar
                            Vector3 pivotOffset = transform.position - currentCenter;
                            targetPos = targetPos3D + pivotOffset;

                            // Visual feedback (optional)
                            // AudioManager.Instance?.grabSound();
                        }
                    }
                }
            }

            transform.position = targetPos;
        }

        private void OnMouseUp()
        {
            dragging = false;

            if (_dropHandler == null)
            {
                Debug.LogError("DropHandler belum diset!");
                transform.position = originalPos;
                return;
            }

            // ✅ PERBAIKAN: Pass posisi PIECE saat ini, bukan mouse!
            // Karena piece sudah di posisi yang kita mau check
            Vector3 currentPiecePos = transform.position;

            if (_dropHandler.TryDrop(
                    pieceId,
                    GetComponent<Collider2D>(),
                    currentPiecePos,  // ← Pass piece position, bukan mouse
                    out Vector3 snapPos))
            {
                transform.position = snapPos;
            }
            else
            {
                transform.position = originalPos;
            }
        }

        // Gizmo untuk visualisasi snap radius (hanya di Scene view)
        private void OnDrawGizmosSelected()
        {
            if (!showSnapGizmo || _packingData == null) return;

            var pieceData = _packingData?.GetPiece(pieceId);
            if (pieceData != null)
            {
                Vector3 targetPos = new Vector3(
                    pieceData.targetPosition.x,
                    pieceData.targetPosition.y,
                    0
                );

                // Draw snap radius
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawWireSphere(targetPos, snapRadius);

                // Draw target point
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(targetPos, 0.1f);

                // Draw line to target saat drag
                if (dragging)
                {
                    Collider2D col = GetComponent<Collider2D>();
                    if (col != null)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(col.bounds.center, targetPos);
                    }
                }
            }
        }
    }
}