using UnityEngine;


public class Startup : MonoBehaviour
{
	
	bool isFadedIn;


	void Start()
	{
		FindObjectOfType<ScreenFader>().FadeIn(onComplete: () => { isFadedIn = true; });
	}



	void Update()
	{
		if ( false == isFadedIn )
			return;
		if ( Input.GetKeyDown(KeyCode.Escape) )
			Application.Quit();
		else if ( Input.GetKeyDown(KeyCode.F5) || OVRInput.GetDown(OVRInput.Button.One))
		{
			OVRManager.display.RecenterPose();

			FindObjectOfType<ScreenFader>().FadeOut(duration: 2, onComplete: () => Application.LoadLevel(1) );
		}
	}


}
