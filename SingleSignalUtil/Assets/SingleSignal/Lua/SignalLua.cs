using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using UnityEngine.Playables;
using System.Threading.Tasks;

// you can customize more.
[MoonSharpUserData]
class SignalLuaBridge
{
    public PlayableDirector playableDirector { get; set; }
    public System.Action<string, object[]> OnMessageRecieved = null; 


    public void Play()
    {
        playableDirector.Play();
    }

    public void Pause()
    {
        playableDirector.Pause();
    }

    public void Stop()
    {
        playableDirector.Stop();
    }

    public void Rewind()
    {
        playableDirector.time = 0;
    }

    public void SetTime(float time)
    {
        playableDirector.time = time;
    }

    public float GetDuration()
    {
        return (float)playableDirector.duration;
    }

    public void Delay(DynValue callback, int millisec)
    {
        DelayCall(()=> {
            callback.Function.Call();
        }, millisec);
    }

    async void DelayCall (System.Action callback, int millisec)
    {
        await Task.Delay(millisec);
        callback?.Invoke();
    }

    public void SendMessage(string mess, params object[] args)
    {
        OnMessageRecieved?.Invoke(mess, args);
    }
}

// you can customize more.
public class SignalLua : MonoBehaviour
{
    [SerializeField]
    private string LUA_MAIN_PATH = "MoonSharp/Scripts/main.lua";

    [SerializeField]
    private PlayableDirector m_playableDirector;

    [SerializeField]
    private List<PlayableDirector> m_playableDirectors = new List<PlayableDirector>();

    private LuaScript m_mainLua;

    public System.Action<string, object[]> OnMessageRecieved = null;

    // Start is called before the first frame update
    void Awake()
    {
        UserData.RegisterAssembly(typeof(SignalLuaBridge).Assembly);
        LoadLua(m_playableDirectors);
    }

    public void LoadLua(PlayableDirector playable)
    {
        m_mainLua = new LuaScript(LUA_MAIN_PATH);
        SignalLuaBridge bridge = new SignalLuaBridge();
        bridge.playableDirector = playable;
        m_mainLua.m_script.Globals["signalLuaBridge"] = bridge;
        m_mainLua.Compile();
    }

    public void LoadLua(List<PlayableDirector> playables)
    {
        m_mainLua = new LuaScript(LUA_MAIN_PATH);
        for (int i = 0; i< playables.Count; i++)
        {
            SignalLuaBridge bridge = new SignalLuaBridge();
            bridge.OnMessageRecieved = (string mess, object[] args) => {
                OnMessageRecieved.Invoke(mess, args);
            };
            bridge.playableDirector = playables[i];
            m_mainLua.m_script.Globals["signalLuaBridge_"+i.ToString()] = bridge;
        }
        m_mainLua.Compile();
    }

    public void CallLuaFunc(string func)
    {
        m_mainLua.m_script.Call(m_mainLua.m_script.Globals[func]);
    }
}
