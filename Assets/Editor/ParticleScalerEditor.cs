using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticleScaler))]
public class ParticleScalerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		ParticleScaler scaleBeingInspected = target as ParticleScaler;
		base.OnInspectorGUI();

		if (GUILayout.Button("Update Scale"))
		{
			scaleBeingInspected.UpdateScale();
		}
	}
}
