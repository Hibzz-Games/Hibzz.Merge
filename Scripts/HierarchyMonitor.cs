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

			// Get the number of conflicts in the gameobject from the merge manager
			int conflictCount = MergeManager.Conflicts(obj).Count;

			// If there are conflicts, then draw that on the hierarchy window
			if(conflictCount > 0)
			{
				Rect offsetRect = new Rect(selectionRect.position, selectionRect.size);
				GUI.Label(offsetRect, $"[{conflictCount} conflicts]", EditorStyleUtility.HierarchyConflictStyle);
			}
		}
	}
}
