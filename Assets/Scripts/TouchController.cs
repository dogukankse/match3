using System;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                if (hit.transform.TryGetComponent(out Tile tile))
                {
                }
            }
        }
    }
}