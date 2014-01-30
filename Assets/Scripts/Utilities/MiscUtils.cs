using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;



public class MiscUtils
{
	static Regex sFilenameCleanerReg = null;
	
	
	public static float GetGlobalScaleInLocalXDirection( Transform trans )
	{
		Transform oldParent = trans.parent;
		trans.parent = null;
		float scaleX = trans.localScale.x;
		trans.parent = oldParent;

		return scaleX;
	}


	
	public static GameObject FindChildInHierarchy( GameObject root, string childName )
	{
		if ( root.name == childName )
			return root;
		
		foreach ( Transform t in root.transform )
		{
			GameObject foundGO = FindChildInHierarchy ( t.gameObject, childName );
			if ( foundGO != null )
				return foundGO;
		}
		
		return null;
	}
	
	

	public static T GetComponentInParents<T>(GameObject go) where T:Component
	{
		T target = null;
		while ( target == null && go != null )
		{
			target = go.GetComponent<T>();
			go = go.transform.parent != null ? go.transform.parent.gameObject : null;
		}

		return target;
	}



	public static T GetComponentSafely<T>(string objectName) where T: Component
	{
		GameObject go = GameObject.Find (objectName);
		return (go != null) ? go.GetComponent<T>() : null;
	}
	
	
	

    public static GameObject FindInChildren(GameObject go, string name)
    {
        if (go.name == name)
            return go;

        foreach (Transform t in go.transform)
        {
            GameObject foundGO = FindInChildren(t.gameObject, name);
            if (foundGO != null)
                return foundGO;
        }

        return null;
    }



	public static string GetCleanFilename (string filename)
	{
		if (sFilenameCleanerReg == null)
		{
			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			sFilenameCleanerReg = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
		}
		
		string cleanedFilename = sFilenameCleanerReg.Replace (filename, "").Replace(" ", "_").Replace("/", "_").Replace(":", "_").Replace(".", "_");
		return cleanedFilename;
	}
	
	
	
		// returns the angle in the range -180 to 180
	public static float NormalizedDegAngle ( float degrees )
	{
		int factor = (int) (degrees/360);
		degrees -= factor * 360;
		if ( degrees > 180 )
			return degrees - 360;
		
		if ( degrees < -180 )
			return degrees + 360;
		
		return degrees;
	}
	
	
	
	public static float NormalizedRadAngle ( float rad )
	{
		int factor = (int) (rad/(2*Mathf.PI));
		rad -= factor * 2 * Mathf.PI;
		if ( rad > Mathf.PI )
			return rad - 2 * Mathf.PI;
		
		if ( rad < -Mathf.PI )
			return rad + 2 * Mathf.PI;
		
		return rad;
	}
	
}
