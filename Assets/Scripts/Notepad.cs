using UnityEngine;


public class Notepad : MonoBehaviour
{
	#region fields
		
		public bool inputEnabled { get; set;  }
		
			string _text;
		public string text {
			get { return _text; }
			set {
				_text = value;
				textMesh.text = text;
				textSize.FitToWidth( maxWidth, kMaxLines );
			}
		}

		const int kMaxLines = 8;
		
		TextMesh textMesh;
		TextSize textSize;
		float maxWidth;
		
	#endregion
	
	
	
	void Awake()
	{
		textMesh = transform.Find("Text").GetComponent<TextMesh>();
		textSize = new TextSize( textMesh );
		maxWidth = MiscUtils.GetGlobalScaleInLocalXDirection( transform.Find("Boundary") );
	}



	public void ProcessInput ()
	{
		if ( Input.inputString.Length == 0 )
			return;
			
		if ( Input.GetKeyDown(KeyCode.Escape) )
			return;
		
		foreach ( char c in Input.inputString )
		{
				bool isBackspace = c == "\b"[0];
			if ( isBackspace )
			{
					if (text.Length != 0)
				text = text.Substring(0, text.Length - 1);
			}
			else if ( text.Length < 400 )
				text += c;
		}
	}
	
	
	
	void Update()
	{
		if ( inputEnabled && Singletons.gameManager.isGameStarted )
			ProcessInput();
	}
	
	
	
}
