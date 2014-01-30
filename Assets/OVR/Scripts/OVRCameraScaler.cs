using UnityEngine;
using System.Collections;

[RequireComponent(typeof(OVRCamera))]

public class OVRCameraScaler : MonoBehaviour 
{
	// How much to scale the render buffer
	public float ScaleFactor = 2f;
	
	// The scaled render texture that the OVRCamera will render to
	private RenderTexture rt;
	
	// The camera that will actually draw the texture
	private GameObject drawCam;
	
	// The component that gets attached to the camera to render the texture
	private TextureRenderCamera trc;
	
	// Set up the scaled rendering
	private void StartScaledRendering () 
	{
		// Get the OVRCamera
		OVRCamera ovrCam = GetComponent<OVRCamera>();
		
		// Create the scaled render texture
		int w = (int)(ovrCam.camera.pixelRect.width  * ScaleFactor);
		int h = (int)(ovrCam.camera.pixelRect.height * ScaleFactor);
		rt = new RenderTexture(w, h, 24);
		rt.hideFlags = HideFlags.HideAndDontSave;
		
		// Create a camera that will simply render the texture
		drawCam = new GameObject("DrawCam", typeof(Camera));
		drawCam.camera.CopyFrom(ovrCam.camera);
		drawCam.hideFlags = HideFlags.HideAndDontSave;
		drawCam.camera.cullingMask = 0; // cull everything
		drawCam.camera.depth += 2; // draw after the OVRCameras
		
		// Add the component that will render the texture to the camera
		trc = drawCam.AddComponent<TextureRenderCamera>();
		trc.hideFlags = HideFlags.HideAndDontSave;
		trc.CameraRenderTexture = rt;
		
		// Have the OVRCamera render to the texture
		ovrCam.camera.targetTexture = rt;
	}
	
	// Enable scaled rendering
	void OnEnable ()
	{
		StartScaledRendering();
	}
	
	// Disable scaled rendering and clean up
	void OnDisable ()
	{
		// Disable rendering to texture for the OVRCamera
		OVRCamera ovrCam = GetComponent<OVRCamera>();
		if (ovrCam != null && ovrCam.camera != null)
		{
			ovrCam.camera.targetTexture = null;
		}
		
		// Destroy the drawing component
		if (trc != null)
		{
			Destroy(trc);
		}
		
		// Destroy the drawing camera
		if (drawCam != null)
		{
			Destroy(drawCam);
		}
		
		// Destroy the render texture
		if (rt != null)
		{
			rt.Release();
			Destroy(rt);
		}
	}
}
