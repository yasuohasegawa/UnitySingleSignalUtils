using UnityEngine;
using UnityEngine.Timeline;

public class TestLuaTimelineController : SingleSignalWithLua
{
    public System.Action<TestSignal> OnSignalTriggered;

    static private TestLuaTimelineController instance = null;
    static public TestLuaTimelineController GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        LuaSetup();
    }

    void Start()
    {
        SetPlayableDirector();
    }

    protected override void OnSignal(SignalAsset signal)
    {
        CallLuaFunc(signal.name);
        OnSignalTriggered?.Invoke(signal as TestSignal);
    }

    protected override void OnTimelineEnd()
    {
        Debug.Log("timeline end");
    }

    protected override void OnMessageRecieved(string mess, object[] args)
    {
        Debug.Log("OnMessageRecieved:" + mess);
        for(int i = 0; i< args.Length; i++)
        {
            Debug.Log(args[i]);
        }
    }
}
