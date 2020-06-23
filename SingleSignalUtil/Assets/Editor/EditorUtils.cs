using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorUtils
{
	public static string GetSavePath(Object selectedObject)
	{
		string objectName = selectedObject.name;
		string dirPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selectedObject));
		string path = string.Format("{0}/{1}", dirPath, objectName);

		if (File.Exists(path))
			for (int i = 1; ; i++)
			{
				path = string.Format("{0}/{1}({2})", dirPath, objectName, i);
				if (!File.Exists(path))
					break;
			}

		return path;
	}

	public static void GetUnityObjectsOfTypeFromPath<T>(string path, List<T> assetsFound)
	{
		string[] filePaths = System.IO.Directory.GetFiles(path);
		if (filePaths != null && filePaths.Length > 0)
		{
			for (int i = 0; i < filePaths.Length; i++)
			{
				UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(T));
				if (obj is T asset)
				{
					if (!assetsFound.Contains(asset))
					{
						assetsFound.Add(asset);
					}
				}
			}
		}
	}

	public static TextAsset GetJson(string path)
	{
		string[] filePaths = System.IO.Directory.GetFiles(path);
		TextAsset textAsset = null;
		if (filePaths != null && filePaths.Length > 0)
		{
			for (int i = 0; i < filePaths.Length; i++)
			{
				if (filePaths[i].Contains(".json.txt") && !filePaths[i].Contains(".meta"))
				{
					textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(TextAsset)) as TextAsset;
				}
			}
		}
		return textAsset;
	}

	public static System.Type GetTypeByClassName(string className)
	{
		foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (System.Type type in assembly.GetTypes())
			{
				if (type.Name == className)
				{
					return type;
				}
			}
		}
		return null;
	}

	public static void WriteTextFile(string path, string str, bool overwrite = false)
    {
		StreamWriter sw = new StreamWriter(path, overwrite);
		sw.WriteLine(str);
		sw.Flush();
		sw.Close();
	}

	public static string DisplayFolderPanel(string title = "Select a output folder")
	{
		string path = EditorUtility.OpenFolderPanel(title, Application.dataPath, string.Empty);
		path = EditorUtils.GetAssetPath(path);
		return path;
	}

	public static string GetAssetPath(string path)
    {
		int startIndex = path.IndexOf("Assets/");
        if (startIndex < 0)
        {
			return "";
        }
		return path.Substring(startIndex);
	}

	/* GUI utils */
	public static void Separator(float height = 1)
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(height));
		EditorGUILayout.EndHorizontal();
	}
}
