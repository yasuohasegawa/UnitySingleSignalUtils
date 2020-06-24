using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSignalWithLua : SingleSignal
{
    [SerializeField]
    private TextAsset m_jsonAsset;

    [SerializeField]
    private SignalLua m_signalLua;

    private SingleSignalData m_signalData;

    public void LuaSetup()
    {
        LoadJSON();
        RegisterLuaCallBack();
    }

    public void LoadJSON()
    {
        m_signalData = SingleSignalData.CreateFromJSON(m_jsonAsset.text);
    }

    public void RegisterLuaCallBack()
    {
        m_signalLua.OnMessageRecieved = _OnMessageRecieved;
    }

    public void RemoveLuaCallback()
    {
        m_signalLua.OnMessageRecieved = null;
    }

    public void CallLuaFunc(string name)
    {
        string functionName = m_signalData.signalList[name].functionName;
        if (!string.IsNullOrEmpty(functionName))
        {
            m_signalLua.CallLuaFunc(functionName);
        }
    }

    public void _OnMessageRecieved(string mess, params object[] args)
    {
        OnMessageRecieved(mess, args);
    }

    protected virtual void OnMessageRecieved(string mess, params object[] args) { }
}
