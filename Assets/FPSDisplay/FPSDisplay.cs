using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	#region fields

		public bool showFPS {
				get { return _showFPS; }
				set { _showFPS = value;
						meshrenderer.enabled = _showFPS;
					}
				}
				bool _showFPS;

		float deltaTime = 0.0f;
		float fps ; 
		string text ; 

		TextMesh textmesh ; 
		MeshRenderer meshrenderer ;

	#endregion


	
	void Start ()
	{
		textmesh = GetComponent<TextMesh>(); 
		meshrenderer = GetComponent<MeshRenderer>(); 
	}



	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		float msec = deltaTime * 1000.0f;
		fps = 1.0f / deltaTime;
		//text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		text = string.Format("{0:0.}", fps);
		textmesh.text = text ; 

	}
	
}