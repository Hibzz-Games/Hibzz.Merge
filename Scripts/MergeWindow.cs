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
				if (_sceneAsset == value) { return; }

				fileReset = true;
				_sceneAsset = value;
			}
		}

		// was the file reset?
		bool fileReset = false;

		// A count of the number of conflicts
		bool? hasConflicts = null;

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
			if (fileReset)
			{
				hasConflicts = null;
				fileReset = false;
			}

			// Do not proceed if no scene is selected
			if (sceneAsset == null) { return; }

			// Draw the buttons to display the analysis and fix conflicts button
			DrawBaseButtons();

			// When the merge manager is actively resolving, then it should draw
			// all conflicts in that gameobject
			if (MergeManager.IsResolving)
			{
				DrawConflictOnSelectedGameObject();
			}
		}

		private void DrawBaseButtons()
		{
			GUILayout.Space(10);

			if(MergeManager.IsResolving)
			{
				// when actively resolving, add the abort button
				if(GUILayout.Button("Abort Resolving", GUILayout.Height(30)))
				{
					MergeManager.Abort();
					fileReset = true;
				}
			}
			else if (hasConflicts is true)
			{
				// Conflicts are found, so add the button to handle the conflicts
				if (GUILayout.Button("Fix Conflicts", GUILayout.Height(30)))
				{
					MergeManager.FixConflicts();
				}

				// notify the user that there are conflicts found in the scene
				GUILayout.Label($"Conflicts found!");
			}
			else
			{
				// Since no conflicts are found (yet), we add a button to analyze for conflicts
				// This will alter that variable to either true or false
				if (GUILayout.Button("Analyze for Merge Conflicts", GUILayout.Height(30)))
				{
					string fp = AssetDatabase.GetAssetPath(sceneAsset.GetInstanceID());
					hasConflicts = MergeManager.HasConflicts(fp);
				}

				// we notify that there are no conflicts found only when the value is false
				// (this variable can be null)
				if (hasConflicts is false)
				{
					GUILayout.Label($"No conflicts found!");
				}
			}
		}

		Vector2 scrollposition;

		private void DrawConflictOnSelectedGameObject()
		{
			// Simple seperator line
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			// Create the scrollable zone
			scrollposition = EditorGUILayout.BeginScrollView(scrollposition);

			// TODO: Fill content here

			// end the scrollable zone
			EditorGUILayout.EndScrollView();
		}
	}
}
