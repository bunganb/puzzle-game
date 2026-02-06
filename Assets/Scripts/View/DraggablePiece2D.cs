using UnityEngine;
using Core;

namespace View
{
    [RequireComponent(typeof(Collider2D))]
    public class DraggablePiece2D : MonoBehaviour
    {
        public string pieceId;

        [Header("Visual Feedback")]
        [SerializeField] private bool enableVisualFeedback = true;
        [SerializeField] private float dragScale = 1.1f; // Scale saat di-drag
        [SerializeField] private float dragAlpha = 0.9f; // Transparency saat di-drag

        private bool dragging;
        private Vector3 offset;
        private Vector3 originalPos;
        private Vector3 originalScale;
        private IDropHandler2D _dropHandler;
        
        // Visual feedback
        private SpriteRenderer _spriteRenderer;
        private int _originalSortingOrder;
        private Color _originalColor;

        public void Setup(string id, IDropHandler2D dropHandler)
        {
            pieceId = id;
            _dropHandler = dropHandler;
            originalPos = transform.position;
            originalScale = transform.localScale;
            
            // Cache sprite renderer
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null)
            {
                _originalSortingOrder = _spriteRenderer.sortingOrder;
                _originalColor = _spriteRenderer.color;
            }
        }

        private void OnMouseDown()
        {
            dragging = true;
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;
            offset = transform.position - mouse;
            
            if (enableVisualFeedback)
            {
                ApplyDragVisuals();
            }
            
            AudioManager.Instance?.grabSound();
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

            if (enableVisualFeedback)
            {
                RestoreVisuals();
            }

            if (_dropHandler == null)
            {
                Debug.LogError("DropHandler belum diset!");
                transform.position = originalPos;
                return;
            }

            Vector3 currentPiecePos = transform.position;

            bool dropSuccess = _dropHandler.TryDrop(
                pieceId,
                GetComponent<Collider2D>(),
                currentPiecePos,
                out Vector3 snapPos);

            if (dropSuccess)
            {
                transform.position = snapPos;
                originalPos = snapPos;
            }
            else
            {
                transform.position = originalPos;
                
                // Optional: Shake effect saat rejected
                if (enableVisualFeedback)
                {
                    StartCoroutine(ShakeEffect());
                }
            }
        }

        // ========== VISUAL EFFECTS ==========

        private void ApplyDragVisuals()
        {
            if (_spriteRenderer != null)
            {
                // ✅ Bring to front (sorting order tertinggi)
                _spriteRenderer.sortingOrder = 100;
                
                // ✅ Sedikit transparan
                Color c = _originalColor;
                c.a = dragAlpha;
                _spriteRenderer.color = c;
            }
            
            // ✅ Scale up sedikit
            transform.localScale = originalScale * dragScale;
        }

        private void RestoreVisuals()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = _originalSortingOrder;
                _spriteRenderer.color = _originalColor;
            }
            
            transform.localScale = originalScale;
        }

        // Shake effect saat drop rejected
        private System.Collections.IEnumerator ShakeEffect()
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                float strength = (1f - elapsed / duration) * 0.1f;
                transform.position = startPos + (Vector3)Random.insideUnitCircle * strength;
                
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            
            transform.position = startPos;
        }

        // Optional: Pulse effect saat hover (perlu Collider2D)
        private void OnMouseEnter()
        {
            if (!dragging && enableVisualFeedback && _spriteRenderer != null)
            {
                // Sedikit highlight
                Color c = _originalColor;
                c.a = 1.1f; // Slightly brighter
                _spriteRenderer.color = c;
            }
        }

        private void OnMouseExit()
        {
            if (!dragging && _spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }
    }
}