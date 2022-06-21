using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Hibzz.Merge
{
	public static class ObjectExtension
	{

		// For more info on how this code segment works, have a look at this thread:
		// https://forum.unity.com/threads/how-to-get-the-local-identifier-in-file-for-scene-objects.265686/
		/// <summary>
		/// Get the local identifier value of the object in a scene file
		/// </summary>
		/// <returns>The object's local identifier in file</returns>
		internal static long GetLocalIdentifierInFile(this Object obj)
		{
			SerializedObject so = new SerializedObject(obj);

			PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
			if (inspectorModeInfo is not null)
			{
				inspectorModeInfo.SetValue(so, InspectorMode.Debug, null);
			}

			SerializedProperty localIDProp = so.FindProperty("m_LocalIdentfierInFile");
			return localIDProp.longValue;
		}
	}
}
