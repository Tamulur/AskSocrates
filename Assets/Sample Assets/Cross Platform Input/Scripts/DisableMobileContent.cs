using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DisableMobileContent : MonoBehaviour {

    public bool enableMobileControls;
    private bool mobileControlsPreviousState;

#if UNITY_EDITOR
    private BuildTarget previousPlatform;

    void OnEnable () {
       EditorUserBuildSettings.activeBuildTargetChanged += SwicthEnableControlsStatus;
        EditorApplication.update += UpdateControlStatus;
    }

    void OnDisable()
    {
        EditorUserBuildSettings.activeBuildTargetChanged -= SwicthEnableControlsStatus;
        EditorApplication.update -= UpdateControlStatus;
    }



    void SwicthEnableControlsStatus()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone 
            || EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android 
            || EditorUserBuildSettings.activeBuildTarget == BuildTarget.WP8Player) {
            enableMobileControls = true;
            Debug.LogWarning ("Enabling Mobile Controls", transform);
        } else {
            enableMobileControls = false;
            Debug.LogWarning ("Disabling Mobile Controls", transform);
        }
    }
#endif


    void Awake () {
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 )
        enableMobileControls = true;
#else
        enableMobileControls = false;
#endif
        SetMobileControlsStatus(enableMobileControls);
        mobileControlsPreviousState = enableMobileControls;
    }

    void UpdateControlStatus()
    {
        if (mobileControlsPreviousState != enableMobileControls)
        {
            SetMobileControlsStatus(enableMobileControls);
            mobileControlsPreviousState = enableMobileControls;
        }
    }

    private void SetMobileControlsStatus(bool activeStatus)
    {
        foreach (Transform child in transform)
        {
            child.transform.gameObject.SetActive(activeStatus);
        }
    }
}
