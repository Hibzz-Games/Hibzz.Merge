using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hibzz.Merge
{
	internal class MergeWindow : EditorWindow
	{
		// The selected scene asset
		SceneAsset _sceneAsset;
		SceneAsset sceneAsset 
		{ 
			get { return _sceneAsset; } 
			set
			{
				if(_sceneAsset == value) { return; }

				fileReset = true;
				_sceneAsset = value;
			}
		}

		bool fileReset = false;

		// A count of the number of conflicts
		int? conflictCount = null;

		[MenuItem("Window/Hibzz Merge")]
		private static void ShowWindow()
		{
			var window = GetWindow<MergeWindow>();
			window.titleContent = new GUIContent("Hibzz Merge");
		}

		private void OnGUI()
		{
			// Read the scene asset
			sceneAsset = EditorGUILayout.ObjectField("Scene", sceneAsset, typeof(SceneAsset), false) as SceneAsset;
			
			// if the file was reset, then handle that
			if(fileReset)
			{
				conflictCount = null;
				fileReset = false;
			}

			// Do not proceed if no scene is selected
			if(sceneAsset == null) { return; }

			// Add button to analyze the scene for merge conflicts
			GUILayout.Space(10);
			if (GUILayout.Button("Analyze for Merge Conflicts", GUILayout.Height(30)))
			{
				string fp = AssetDatabase.GetAssetPath(sceneAsset.GetInstanceID());
				conflictCount = MergeManager.CountConflicts(fp);
			}

			// if the conflict count is null, then it wasn't analyzed... So, don't report anything
			if(conflictCount == null) { return; }

			// Draw the content saying how many conflicts were found
			GUILayout.Label($"{conflictCount} conflict(s) found!");
			GUILayout.FlexibleSpace();
			
			// And when there are conflicts, add a button to handle the conflicts
			if (conflictCount > 0 && GUILayout.Button("Fix Conflicts", GUILayout.Height(35)))
			{
				MergeManager.FixConflicts();
			}
		}
	}
}
