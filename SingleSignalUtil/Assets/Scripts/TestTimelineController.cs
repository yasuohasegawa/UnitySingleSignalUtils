using UnityEngine;
using UnityEngine.Timeline;

public class TestTimelineController : SingleSignal
{
    public System.Action<TestSignal> OnSignalTriggered;

    static private TestTimelineController instance = null;
    static public TestTimelineController GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetPlayableDirector();
        m_playableDirector.Play();
    }

    protected override void OnSignal(SignalAsset signal)
    {
        OnSignalTriggered?.Invoke(signal as TestSignal);
    }

    protected override void OnTimelineEnd()
    {
        Debug.Log("timeline end");
    }
}
