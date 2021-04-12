using UnityEngine;
using UnityEngine.Events;

public class TouchController : MonoBehaviour
{
	public UnityAction<Tile> OnClick;

	private bool _block;

	public void AllowUserInput()
	{
		_block = false;
	}

	public void BlockUserInput()
	{
		_block = true;
	}

	private void Update()
	{
		if (_block) return;
		#if UNITY_EDITOR || UNITY_EDITOR_OSX
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);
			if (hit && hit.transform.TryGetComponent(out Tile tile))
			{
				_block = true;
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
					_block = true;
					OnClick(tile);
				}
			}
		}

		#endif
	}
}