using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SignalData
{
    public string signalID;
    public float time;
    public string functionName;
}

[System.Serializable]
public class SingleSignalData: ISerializationCallbackReceiver
{
    [NonSerialized] public Dictionary<string, SignalData> signalList;
    public SignalData[] signals;

    public static SingleSignalData CreateFromJSON(string json)
    {
        SingleSignalData instance = null;
        try
        {
            instance = JsonUtility.FromJson<SingleSignalData>(json);
        }
        catch (Exception e)
        {
            //例外を処理する場合
            Debug.Log(e);
        }
        return instance;
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        signalList = new Dictionary<string, SignalData>();
        for (int i = 0; i < signals.Length; i++)
        {
            SignalData data = signals[i];
            signalList.Add(data.signalID, data);
        }
    }
}