using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Hibzz.Merge
{
	[InitializeOnLoad]
	public static class HierarchyMonitor
	{
		/// <summary>
		/// Offset of the icon in heirarchy window
		/// </summary>
		static Vector2 offset = new Vector2(0, 0);

		/// <summary>
		/// Size of the icon
		/// </summary>
		static Vector2 size = new Vector2(50, 50);

		static HierarchyMonitor()
		{
			EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyItemOnGUI;
		}

		static void HandleHierarchyItemOnGUI(int instanceID, Rect selectionRect)
		{
			// if not resolving at the moment, don't proceed forward
			if(!MergeManager.IsResolving) { return; }

			// Get the gameobject with the instance id and make sure that it's a gameobject
			GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if(obj is null) { return; }

			// increase the count conflict
			int conflictCount = 0;

			// get all the components of the gameobject and check if it matches
			var components = obj.GetComponents<Component>();
			foreach(var component in components)
			{
				var id = component.GetLocalIdentifierInFile().ToString();
				var hasConflict = MergeManager.Conflicts.Exists((conflict) => conflict.ObjectId == id);

				// increase the conflict counter
				if(hasConflict) { conflictCount++; }
			}

			// If there are conflicts, then draw that on the hierarchy window
			if(conflictCount > 0)
			{
				Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
				GUI.Label(offsetRect, $"[{conflictCount} conflicts]", new GUIStyle() 
				{
					normal = new GUIStyleState() { textColor = Color.red },
					alignment = TextAnchor.MiddleRight
				});
			}
		}
	}
}
