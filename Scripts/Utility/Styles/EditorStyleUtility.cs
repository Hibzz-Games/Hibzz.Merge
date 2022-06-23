using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hibzz.Merge
{
    internal static class EditorStyleUtility
    {
		#region Readonly Color

		private static readonly Color errorColor = new Color(1.0f, 0.43137f, 0.25098f);
		private static readonly Color inactiveColor = new Color(0.45098f, 0.45098f, 0.45098f);

		#endregion

		private static GUIStyle _hierarchyConflictStyle;
		internal static GUIStyle HierarchyConflictStyle
		{
			get
			{
				// if null, then set value to the new one
				_hierarchyConflictStyle ??= new GUIStyle()
				{
					normal = new GUIStyleState() { textColor = errorColor },
					alignment = TextAnchor.MiddleRight
				};

				return _hierarchyConflictStyle;
			}
		}

		private static GUIStyle _inactiveFontStyle;
		internal static GUIStyle InactiveFontStyle
		{
			get
			{
				// when null set the value to the new one
				_inactiveFontStyle ??= new GUIStyle()
				{
					normal = new GUIStyleState() { textColor = inactiveColor },
					fontStyle = FontStyle.Italic,
					alignment = TextAnchor.MiddleCenter
				};

				return _inactiveFontStyle;
			}
		}
	}
}
