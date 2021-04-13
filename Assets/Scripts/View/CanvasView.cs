using UnityEngine;
using UnityEngine.UI;

namespace View
{
	public class CanvasView
	{
		public static CanvasView Instance => _instance ??= new CanvasView();

		private static CanvasView _instance;

		private static Canvas _canvas;

		private CanvasView()
		{
			_canvas = GameObject.FindWithTag("UI").GetComponent<Canvas>();
		}
		
		public void ShowAlert(string msg)
		{
			GameObject go = new GameObject("Alert");
			go.transform.SetParent(_canvas.transform);
			RectTransform rt =go.AddComponent<RectTransform>();
			rt.anchorMin = new Vector2(0,1);
			rt.anchorMax =  new Vector2(1,1);;
			rt.pivot =  new Vector2(0.5f,1);;
			rt.anchoredPosition = Vector2.zero;
			Text t =go.AddComponent<Text>();
			t.text = msg;
			t.font = Font.CreateDynamicFontFromOSFont("Ariel",300);
			t.fontSize = 300;
			t.color = Color.black;
			t.alignment = TextAnchor.MiddleCenter;
			t.resizeTextForBestFit = true;
		}

	}
}