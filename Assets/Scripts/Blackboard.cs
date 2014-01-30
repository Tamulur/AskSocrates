using UnityEngine;

public class Blackboard : MonoBehaviour
{

	#region fields
		const int kMaxLines = 5;

		TextMesh wallTextMesh;
		TextSize wallTextSize;
		float maxWidth;

	#endregion


	void Awake()
	{
		wallTextMesh = GetComponentInChildren<TextMesh>();
		maxWidth = MiscUtils.GetGlobalScaleInLocalXDirection( transform.Find("Boundary") );
		wallTextSize = new TextSize( wallTextMesh );
	}



	public void ShowText(string text)
	{
		wallTextMesh.text = text;
		wallTextSize.FitToWidth( maxWidth, kMaxLines );
	}

}
