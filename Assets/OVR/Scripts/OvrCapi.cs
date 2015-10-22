/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ovr
{
    /// <summary>
    /// A 2D vector with integer components.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct Vector2i
    {
        [FieldOffset(0)] public Int32 x;
        [FieldOffset(4)] public Int32 y;

        public Vector2i(Int32 _x, Int32 _y)
        {
            x = _x;
            y = _y;
        }
    };

    /// <summary>
    /// A 2D size with integer components.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	internal struct Sizei
    {
        [FieldOffset(0)] public Int32 w;
        [FieldOffset(4)] public Int32 h;

        public Sizei(Int32 _w, Int32 _h)
        {
            w = _w;
            h = _h;
        }
    };

    /// <summary>
    /// A 2D rectangle with a position and size.
    /// All components are integers.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	internal struct Recti
    {
        [FieldOffset(0)] public Vector2i Pos;
        [FieldOffset(8)] public Sizei Size;
    };

    /// <summary>
    /// A quaternion rotation.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	internal struct Quatf
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;
        [FieldOffset(8)] public float z;
        [FieldOffset(12)] public float w;

        public Quatf(float _x, float _y, float _z, float _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }
    };

    /// <summary>
    /// A 2D vector with float components.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	internal struct Vector2f
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;

        public Vector2f(float _x, float _y)
        {
            x = _x;
            y = _y;
        }
    };

    /// <summary>
    /// A 3D vector with float components.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	internal struct Vector3f
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;
        [FieldOffset(8)] public float z;

        public Vector3f(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
    };

    /// <summary>
    /// Position and orientation together.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	internal struct Posef
    {
        [FieldOffset(0)] public Quatf Orientation;
        [FieldOffset(16)] public Vector3f Position;

        public Posef(Quatf q, Vector3f p)
        {
            Orientation = q;
            Position = p;
        }
    };

    //-----------------------------------------------------------------------------------
    // ***** HMD Types

    /// <summary>
    /// Enumerates all HMD types that we support.
    /// </summary>
	internal enum HmdType
    {
        None      = 0,
        DK1       = 3,
        DKHD      = 4,
        DK2       = 6,
        BlackStar = 7,
        CB        = 8,
        Other // Some HMD other than the ones in this enumeration.
    };

    /// <summary>
    /// HMD capability bits reported by device.
    /// </summary>
	internal enum HmdCaps
    {
        /// <summary>
        /// (read only) The HMD is plugged in and detected by the system.
        /// </summary>
        Present           = 0x0001,
        /// <summary>
        /// (read only) The HMD and its sensor are available for ownership use.
        /// i.e. it is not already owned by another application.
        /// </summary>
        Available         = 0x0002,
        /// <summary>
        /// (read only) Set to 'true' if we captured ownership of this HMD.
        /// </summary>
        Captured          = 0x0004,
        /// <summary>
        /// (read only) Means the display driver is in compatibility mode.
        /// </summary>
        ExtendDesktop     = 0x0008,
        /// <summary>
        /// (read only) Means HMD device is a virtual debug device.
        /// </summary>
        DebugDevice       = 0x0010,

        // Modifiable flags (through ovrHmd_SetEnabledCaps).

        /// <summary>
        /// Disables mirroring of HMD output to the window. This may improve
        /// rendering performance slightly (only if 'ExtendDesktop' is off).
        /// </summary>
        NoMirrorToWindow  = 0x2000,
        /// <summary>
        /// Turns off HMD screen and output (only if 'ExtendDesktop' is off).
        /// </summary>
        DisplayOff        = 0x0040,
        /// <summary>
        /// HMD supports low persistence mode.
        /// </summary>
        LowPersistence    = 0x0080,
        /// <summary>
        /// Adjust prediction dynamically based on internally measured latency.
        /// </summary>
        DynamicPrediction = 0x0200,
        /// <summary>
        /// Support rendering without VSync for debugging.
        /// </summary>
        NoVSync           = 0x1000,

        /// <summary>
        /// These bits can be modified by ovrHmd_SetEnabledCaps.
        /// </summary>
        WritableMask      = NoMirrorToWindow
                            | DisplayOff
                            | LowPersistence
                            | DynamicPrediction
                            | NoVSync,

        /// <summary>
        /// These flags are currently passed into the service. May change without notice.
        /// </summary>
        ServiceMask       = NoMirrorToWindow
                            | DisplayOff
                            | LowPersistence
                            | DynamicPrediction,
    };

    /// <summary>
    /// Tracking capability bits reported by the device.
    /// Used with ovrHmd_ConfigureTracking.
    /// </summary>
	internal enum TrackingCaps
    {
        /// <summary>
        /// Supports orientation tracking (IMU).
        /// </summary>
        Orientation       = 0x0010,
        /// <summary>
        /// Supports yaw drift correction via a magnetometer or other means.
        /// </summary>
        MagYawCorrection  = 0x0020,
        /// <summary>
        /// Supports positional tracking.
        /// </summary>
        Position          = 0x0040,
        /// <summary>
        /// Overrides the other flags. Indicates that the application
        /// doesn't care about tracking settings. This is the internal
        /// default before ovrHmd_ConfigureTracking is called.
        /// </summary>
        Idle              = 0x0100,
    };

    /// <summary>
    /// Distortion capability bits reported by device.
    /// Used with ovrHmd_ConfigureRendering and ovrHmd_CreateDistortionMesh.
    /// </summary>
	internal enum DistortionCaps
    {
        /// <summary>
        /// Supports chromatic aberration correction.
        /// </summary>
        Chromatic = 0x01,
        /// <summary>
        /// Supports timewarp.
        /// </summary>
        TimeWarp = 0x02,

        // 0x04 unused

        /// <summary>
        /// Supports vignetting around the edges of the view.
        /// </summary>
        Vignette = 0x08,
        /// <summary>
        /// Do not save and restore the graphics state when rendering distortion.
        /// </summary>
        NoRestore = 0x10,
        /// <summary>
        /// Flip the vertical texture coordinate of input images.
        /// </summary>
        FlipInput = 0x20,
        /// <summary>
        /// Assume input images are in sRGB gamma-corrected color space.
        /// </summary>
        SRGB = 0x40,
        /// <summary>
        /// Overdrive brightness transitions to reduce artifacts on DK2+ displays
        /// </summary>
        Overdrive = 0x80,
        /// <summary>
        /// High-quality sampling of distortion buffer for anti-aliasing
        /// </summary>
        HqDistortion = 0x100,
        /// </summary>
        /// Indicates window is fullscreen on a device when set.
        /// The SDK will automatically apply distortion mesh rotation if needed.
        /// </summary>
        LinuxDevFullscreen = 0x200,
        /// <summary>
        /// Using compute shader (DX11+ only)
        /// </summary>
        ComputeShader = 0x400,
        /// <summary>
        /// Use when profiling with timewarp to remove false positives
        /// </summary>
        ProfileNoTimewarpSpinWaits = 0x10000,
    };

    /// <summary>
    /// Specifies which eye is being used for rendering.
    /// This type explicitly does not include a third "NoStereo" option, as such is
    /// not required for an HMD-centered API.
    /// </summary>
    internal enum Eye
    {
        Left  = 0,
        Right = 1,
        Count = 2,
    };

    /// <summary>
    /// Bit flags describing the current status of sensor tracking.
    /// The values must be the same as in enum StatusBits.
    /// </summary>
	internal enum StatusBits
    {
        /// <summary>
        /// Orientation is currently tracked (connected and in use).
        /// </summary>
        OrientationTracked    = 0x0001,
        /// <summary>
        /// Position is currently tracked (false if out of range).
        /// </summary>
        PositionTracked       = 0x0002,
        /// <summary>
        /// Camera pose is currently tracked.
        /// </summary>
        CameraPoseTracked     = 0x0004,
        /// <summary>
        /// Position tracking hardware is connected.
        /// </summary>
        PositionConnected     = 0x0020,
        /// <summary>
        /// HMD Display is available and connected.
        /// </summary>
        HmdConnected          = 0x0080,
    };

    //-----------------------------------------------------------------------------------
    // ***** Platform-independent Rendering Configuration

    /// <summary>
    /// These types are used to hide platform-specific details when passing
    /// render device, OS, and texture data to the API.
    ///
    /// The benefit of having these wrappers versus platform-specific API functions is
    /// that they allow game glue code to be portable. A typical example is an
    /// engine that has multiple back ends, say GL and D3D. Portable code that calls
    /// these back ends may also use LibOVR. To do this, back ends can be modified
    /// to return portable types such as ovrTexture and ovrRenderAPIConfig.
    /// </summary>
	internal enum RenderAPIType
    {
        None,
        OpenGL,
        Android_GLES,  // May include extra native window pointers, etc.
        D3D9,
        D3D10, // Deprecated: Not supported for SDK rendering
        D3D11,
        Count,
    };

    /// <summary>
    /// Provides an interface to a CAPI HMD object.  The ovrHmd instance is normally
    /// created by ovrHmd::Create, after which its other methods can be called.
    /// The typical process would involve calling:
    ///
    /// Setup:
    ///   - Initialize() to initialize the OVR SDK.
    ///   - Construct Hmd to create an ovrHmd.
    ///   - Use hmd members and ovrHmd_GetFovTextureSize() to determine graphics configuration.
    ///   - ConfigureTracking() to configure and initialize tracking.
    ///   - ConfigureRendering() to setup graphics for SDK rendering, which is the preferred approach.
    ///   - Please refer to "Client Distortion Rendering" below if you prefer to do that instead.
    ///   - If ovrHmdCap_ExtendDesktop is not set, use ovrHmd_AttachToWindow to associate the window with an Hmd.
    ///   - Allocate render textures as needed.
    ///
    /// Game Loop:
    ///   - Call ovrHmd_BeginFrame() to get frame timing and orientation information.
    ///   - Render each eye in between, using ovrHmd_GetEyePoses or ovrHmd_GetHmdPosePerEye to get the predicted hmd pose and each eye pose.
    ///   - Call ovrHmd_EndFrame() to render distorted textures to the back buffer
    ///     and present them on the Hmd.
    ///
    /// Shutdown:
    ///   - Dispose the Hmd to release the ovrHmd.
    ///   - ovr_Shutdown() to shutdown the OVR SDK.
    /// </summary>
	internal class Hmd
    {
        public const string OVR_VERSION_STRING                    = "0.5.0";
        public const string OVR_KEY_USER                          = "User";
        public const string OVR_KEY_NAME                          = "Name";
        public const string OVR_KEY_GENDER                        = "Gender";
        public const string OVR_KEY_PLAYER_HEIGHT                 = "PlayerHeight";
        public const string OVR_KEY_EYE_HEIGHT                    = "EyeHeight";
        public const string OVR_KEY_IPD                           = "IPD";
        public const string OVR_KEY_NECK_TO_EYE_DISTANCE          = "NeckEyeDistance";
        public const string OVR_KEY_EYE_RELIEF_DIAL               = "EyeReliefDial";
        public const string OVR_KEY_EYE_TO_NOSE_DISTANCE          = "EyeToNoseDist";
        public const string OVR_KEY_MAX_EYE_TO_PLATE_DISTANCE     = "MaxEyeToPlateDist";
        public const string OVR_KEY_EYE_CUP                       = "EyeCup";
        public const string OVR_KEY_CUSTOM_EYE_RENDER             = "CustomEyeRender";
        public const string OVR_KEY_CAMERA_POSITION               = "CenteredFromWorld";

        // Default measurements empirically determined at Oculus to make us happy
        // The neck model numbers were derived as an average of the male and female averages from ANSUR-88
        // NECK_TO_EYE_HORIZONTAL = H22 - H43 = INFRAORBITALE_BACK_OF_HEAD - TRAGION_BACK_OF_HEAD
        // NECK_TO_EYE_VERTICAL = H21 - H15 = GONION_TOP_OF_HEAD - ECTOORBITALE_TOP_OF_HEAD
        // These were determined to be the best in a small user study, clearly beating out the previous default values
        public const string OVR_DEFAULT_GENDER                = "Unknown";
        public const float OVR_DEFAULT_PLAYER_HEIGHT          = 1.778f;
        public const float OVR_DEFAULT_EYE_HEIGHT             = 1.675f;
        public const float OVR_DEFAULT_IPD                    = 0.064f;
        public const float OVR_DEFAULT_NECK_TO_EYE_HORIZONTAL = 0.0805f;
        public const float OVR_DEFAULT_NECK_TO_EYE_VERTICAL   = 0.075f;
        public const float OVR_DEFAULT_EYE_RELIEF_DIAL        = 3;
        public readonly float[] OVR_DEFAULT_CAMERA_POSITION   = {0,0,0,1,0,0,0};
    }
}
