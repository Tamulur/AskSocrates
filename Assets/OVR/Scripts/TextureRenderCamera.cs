using UnityEngine;
using System.Collections;

public class TextureRenderCamera : MonoBehaviour
{
	public RenderTexture CameraRenderTexture;
	
	// OnRenderImage
	void  OnRenderImage (RenderTexture source, RenderTexture destination)
	{	
		// Just draw the texture
		Graphics.Blit(CameraRenderTexture, destination);
	}
}