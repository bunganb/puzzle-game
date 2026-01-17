using UnityEngine;
using Core;

namespace View
{
    public class DraggablePiece2D : MonoBehaviour
    {
        public string pieceId;

        private bool dragging;
        private Vector3 offset;
        private Vector3 originalPos;
        private IDropHandler2D _dropHandler;

        public void Setup(string id, IDropHandler2D dropHandler)
        {
            pieceId = id;
            _dropHandler = dropHandler;
            originalPos = transform.position;
        }
        private static Bounds ShiftBounds(Bounds b, Vector3 newCenter)
        {
            Vector3 delta = newCenter - b.center;
            b.center += delta;
            return b;
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

            if (_dropHandler == null)
            {
                Debug.LogError("DropHandler belum diset!");
                transform.position = originalPos;
                return;
            }

            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;

            if (_dropHandler.TryDrop(
                    pieceId,
                    GetComponent<Collider2D>(),
                    mouse,
                    out Vector3 snapPos))
            {
                transform.position = snapPos;
            }
            else
            {
                transform.position = originalPos;
            }
        }
    }
}