/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.3 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculus.com/licenses/LICENSE-3.3

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Ovr;

/// <summary>
/// An infrared camera that tracks the position of a head-mounted display.
/// </summary>
public class OVRTracker
{
	/// <summary>
	/// The (symmetric) visible area in front of the sensor.
	/// </summary>
	public struct Frustum
	{
		/// <summary>
		/// The sensor cannot track the HMD unless it is at least this far away.
		/// </summary>
		public float nearZ;
		/// <summary>
		/// The sensor cannot track the HMD unless it is at least this close.
		/// </summary>
		public float farZ;
		/// <summary>
		/// The sensor's horizontal and vertical fields of view in degrees.
		/// </summary>
		public Vector2 fov;
	}

	/// <summary>
	/// If true, a sensor is attached to the system.
	/// </summary>
	public bool isPresent
	{
	    get {
#if !UNITY_ANDROID || UNITY_EDITOR
			if (OVRManager.instance.isVRPresent)
			{
				return OVRPlugin.positionSupported;
			}
#endif
			return false;
		}
	}

	/// <summary>
	/// If true, the sensor can see and track the HMD. Otherwise the HMD may be occluded or the system may be malfunctioning.
	/// </summary>
	public bool isPositionTracked
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			if (OVRManager.instance.isVRPresent)
			{
				return OVRPlugin.positionTracked;
			}
#endif
			return false;
		}
	}

	/// <summary>
	/// If this is true and a sensor is available, the system will use position tracking when isPositionTracked is also true.
	/// </summary>
	public bool isEnabled
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			if (OVRManager.instance.isVRPresent)
			{
				return OVRPlugin.position;
			}
#endif
			return false;
		}

		set {
#if !UNITY_ANDROID || UNITY_EDITOR
			if (OVRManager.instance.isVRPresent)
				return;

			OVRPlugin.position = value;
#endif
		}
	}

	/// <summary>
	/// Gets the sensor's viewing frustum.
	/// </summary>
	public Frustum frustum
	{
		get {
#if !UNITY_ANDROID || UNITY_EDITOR
			if (OVRManager.instance.isVRPresent)
			{
				return OVRPlugin.GetTrackerFrustum(OVRPlugin.Tracker.Zero).ToFrustum();
			}
#endif
			return new Frustum
			{
				nearZ = 0.1f,
				farZ = 1000.0f,
				fov = new Vector2(90.0f, 90.0f)
			};
		}
	}

	/// <summary>
	/// Gets the sensor's pose, relative to the head's pose at the time of the last pose recentering.
	/// </summary>
	public OVRPose GetPose(double predictionTime)
	{
#if !UNITY_ANDROID || UNITY_EDITOR
		if (OVRManager.instance.isVRPresent)
		{
			return OVRPlugin.GetTrackerPose(OVRPlugin.Tracker.Zero).ToOVRPose();
		}
#endif
		return new OVRPose
		{
			position = Vector3.zero,
			orientation = Quaternion.identity
		};
	}
}
