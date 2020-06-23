using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class SignalObjectToAsset : EditorWindow
{
	private static string m_signalOutPath = "Assets/Resources"; // you can modifiy an initial path.
	private static string m_playableFileName = "sample.playable";
	private static string CSV_FILEPATH = "../signaldata.csv";

	private static UnityEngine.Object m_signalClass;
	private static SignalTrack m_trackAsset;
	private static TimelineAsset m_timelineAsset;
	private static TextAsset m_jsonAsset;

	private GUIStyle style = new GUIStyle();

	static void Create()
	{
		//UnityEngine.Object selectedObj = Selection.activeObject;
		if(m_signalClass != null)
        {
			string className = m_signalClass.name;
			System.Type type = EditorUtils.GetTypeByClassName(className);
			object signal = System.Activator.CreateInstance(type); ;
			System.Reflection.MethodInfo method = type.GetMethod("GetSignalEnums");
			if(method == null)
            {
				DisplaySignalClassDialog();
				return;
            }

			string path = EditorUtils.DisplayFolderPanel();
			if(path == "")
            {
				return;
            }

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

				AssetDatabase.CreateAsset(obj, path + "/"+ enumStr + ".asset");
			}
			//Debug.Log("Out Path:" + m_signalOutPath);
		} else
        {
			DisplaySignalClassDialog();
		}
	}

	static void CreateTimelineAndSignalTrack()
	{
		m_timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
		string path = EditorUtils.DisplayFolderPanel();
		if (path == "")
		{
			return;
		}
		AssetDatabase.CreateAsset(m_timelineAsset, path+"/"+ m_playableFileName);
		m_trackAsset = m_timelineAsset.CreateTrack<SignalTrack>(null, "Signal Track");
	}

	static void UpdateTrackAsset()
    {
		foreach (var t in m_timelineAsset.GetOutputTracks())
		{
			if (t.name.Contains("Signal"))
			{
				m_trackAsset = t as SignalTrack;
				break;
			}
		}
	}

	static void LoadSignalAssetsToTimeline()
    {
		if(m_timelineAsset == null)
        {
			return;
        }

		UpdateTrackAsset();

		foreach (var m in m_trackAsset.GetMarkers())
		{
			m_trackAsset.DeleteMarker(m);
		}

		List<ScriptableObject> list = new List<ScriptableObject>();
		string path = EditorUtils.DisplayFolderPanel("Load signals");
		if (path == "")
		{
			return;
		}
		EditorUtils.GetUnityObjectsOfTypeFromPath(path, list);

		Debug.Log(list.Count);

		TextAsset jsonTxt = EditorUtils.GetJson(path);
		SingleSignalData json = SingleSignalData.CreateFromJSON(jsonTxt.text);

		Debug.Log(json.signalList.Count);
		for (int i = 0; i < list.Count; i++)
		{
			SignalAsset signal = list[i] as SignalAsset;
			SignalData data = json.signalList[signal.name];
			//Debug.Log(data.time);

			SignalEmitter emitter = m_trackAsset.CreateMarker<SignalEmitter>(data.time);
			emitter.name = signal.name;
			emitter.asset = signal;
		}
	}

	static void ExportCSV()
    {
		if (m_timelineAsset == null || m_jsonAsset == null)
		{
			if (m_jsonAsset == null)
			{
				DisplayJsonFileDialog();
			} else
            {
				DisplayTimelineAsseteDialog();
			}
			return;
		}
		
		// we have to merge the lua function name.
		SingleSignalData json = SingleSignalData.CreateFromJSON(m_jsonAsset.text);

		UpdateTrackAsset();

		Dictionary<string, float> signals = new Dictionary<string, float>();
		foreach (var m in m_trackAsset.GetMarkers())
		{
			SignalEmitter se = m as SignalEmitter;

            if (string.IsNullOrEmpty(se.name) || se.name == "Signal Emitter")
            {
				EditorUtility.DisplayDialog("Name is empty", "You need to set up name as same as the SignalID.", "OK");
				return;
			}

			signals.Add(se.name, (float)se.time);
		}

		IOrderedEnumerable<KeyValuePair<string, float>> sorted = signals.OrderBy(pair => pair.Value);

		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Signal Enum,time,function names\n");
		foreach (KeyValuePair<string, float> pair in sorted)
		{
			string funcName = "";
			if(json.signalList.ContainsKey(pair.Key))
            {
				funcName = json.signalList[pair.Key].functionName;
			}
			//Debug.LogFormat("{0} {1}", pair.Key, pair.Value);
			stringBuilder.Append(pair.Key+","+ pair.Value+","+ funcName+"\n");
		}

		EditorUtils.WriteTextFile(CSV_FILEPATH, stringBuilder.ToString());
	}

	[MenuItem("SingleSignal/GenerateSignals")]
    static void ShowWindow()
	{
		EditorWindow.GetWindow<SignalObjectToAsset>();
	}

	void OnGUI()
	{
		style.fontStyle = FontStyle.Bold;

		GUILayout.Label("Select a signal assets class in the project view:", style);
		m_signalClass = EditorGUILayout.ObjectField("SignalAsset Class:", m_signalClass, typeof(UnityEngine.Object), false) as UnityEngine.Object;
		if (GUILayout.Button("Create signal assets"))
		{
			Create();
		}

		EditorUtils.Separator(1f);
		GUILayout.Space(20);

		style.fontStyle = FontStyle.Bold;
		GUILayout.Label("Input a playable file name here:", style);
		m_playableFileName = EditorGUILayout.TextField("Playable file name: ", m_playableFileName);
		if (GUILayout.Button("Select a destination folder and Create a TimelineAsset and SignalTrack"))
		{
			CreateTimelineAndSignalTrack();
		}

		EditorUtils.Separator(1f);
		GUILayout.Space(20);

		style.fontStyle = FontStyle.Bold;
		GUILayout.Label("Select a folder in the project view. The folder must contain a JSON file:",style);
		m_timelineAsset = EditorGUILayout.ObjectField("Playable:", m_timelineAsset, typeof(TimelineAsset), true) as TimelineAsset;
		if (GUILayout.Button("Load signals to the timeline"))
		{
			LoadSignalAssetsToTimeline();
		}

		EditorUtils.Separator(1f);
		GUILayout.Space(20);

		m_jsonAsset = EditorGUILayout.ObjectField("Signal Json:", m_jsonAsset, typeof(TextAsset), true) as TextAsset;
		if (GUILayout.Button("Export CSV"))
		{
			ExportCSV();
		}

		//EditorGUILayout.ObjectField()
		this.Repaint();
	}


	/* Dialogs */
	static void DisplaySignalClassDialog()
	{
		EditorUtility.DisplayDialog("Select SignalAsset Class", "You must select a SignalAsset Class first!", "OK");
	}

	static void DisplayJsonFileDialog()
	{
		EditorUtility.DisplayDialog("Select a Json file", "You must select a json file!", "OK");
	}

	static void DisplayTimelineAsseteDialog()
	{
		EditorUtility.DisplayDialog("Select Timeline Asset", "You must select a Timeline Asset!", "OK");
	}
}