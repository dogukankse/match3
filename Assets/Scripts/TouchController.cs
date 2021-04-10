using System;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public class TouchController : MonoBehaviour
{
	public UnityAction<Tile> OnClick;

	private void Update()
	{
		#if UNITY_EDITOR || UNITY_EDITOR_OSX
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);
			if (hit && hit.transform.TryGetComponent(out Tile tile))
			{
				OnClick(tile);
			}
		}
		#elif UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
			if (touch.phase == TouchPhase.Began)
			{
				RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
				if (hit && hit.transform.TryGetComponent(out Tile tile))
				{
					OnClick(tile);
				}
			}
		}

		#endif
	}
}