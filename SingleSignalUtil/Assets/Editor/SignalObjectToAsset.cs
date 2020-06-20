using UnityEngine;
using UnityEditor;
using System.IO;

public class SignalObjectToAsset : EditorWindow
{
	private static string m_signalOutPath = "Assets/Resources"; // you can modefy an initial path.

	static void Create()
	{
		UnityEngine.Object selectedObj = Selection.activeObject;
		if(selectedObj != null)
        {
			string className = selectedObj.name;
			System.Type type = GetTypeByClassName(className);
			object signal = System.Activator.CreateInstance(type); ;
			System.Reflection.MethodInfo method = type.GetMethod("GetSignalEnums");

			string[] _enums = (string[])method.Invoke(signal, null);
			for (int i = 0; i < _enums.Length; i++)
			{
				Debug.Log(_enums[i]);
				string enumStr = _enums[i];
				ScriptableObject obj = ScriptableObject.CreateInstance(className);
				System.Reflection.MethodInfo mInfo = obj.GetType().GetMethod("GetSignalEnum");
				System.Reflection.MethodInfo mInfo2 = obj.GetType().GetMethod("SetSignalEnum");
				var signalEnum = mInfo.Invoke(obj, new object[] { enumStr });
				mInfo2.Invoke(obj, new object[] { signalEnum });

				AssetDatabase.CreateAsset(obj, m_signalOutPath+"/"+ enumStr + ".asset");
			}
			//Debug.Log("Out Path:" + m_signalOutPath);
		}
	}

	static string GetSavePath(Object selectedObject)
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

	[MenuItem("SingleSignal/GenerateSignals")]
    static void ShowWindow()
	{
		EditorWindow.GetWindow<SignalObjectToAsset>();
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

	void OnGUI()
	{
		GUILayout.Label("Input a signal sssets folder path");
		m_signalOutPath = EditorGUILayout.TextField("Output Folder: ", m_signalOutPath);
		if (GUILayout.Button("Select a folder as output folder"))
		{
			m_signalOutPath = GetSavePath(Selection.activeObject);
		}

		GUILayout.Label("Select a signal sssets class in the project view");
		if (GUILayout.Button("output signal assets"))
		{
			Create();
		}
		this.Repaint();
	}
}