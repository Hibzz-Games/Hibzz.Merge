using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hibzz.Merge
{
    internal static class EditorStyleUtility
    {
		#region Readonly Color

		private static readonly Color errorColor = new Color(0.65882f, 0, 0);

		#endregion

		private static GUIStyle _hierarchyConflictStyle;
		internal static GUIStyle HierarchyConflictStyle
		{
			get
			{
				// if null, then set value to the new one
				_hierarchyConflictStyle ??= new GUIStyle()
				{
					normal = new GUIStyleState() { textColor = Color.red },
					alignment = TextAnchor.MiddleRight
				};

				return _hierarchyConflictStyle;
			}
		}
	}
}
