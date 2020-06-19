using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class SingleSignal : MonoBehaviour
{
    [SerializeField]
    private string m_signalResourcePath;
    private Object[] m_signals;

    public PlayableDirector m_playableDirector;

    public void SetPlayableDirector()
    {
        m_playableDirector = GetComponent<PlayableDirector>();
        m_playableDirector.stopped += _OnPlayableDirectorStopped;
    }

    // this works on runtime only.
    void _OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (m_playableDirector == aDirector)
        {
            //Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
            OnTimelineEnd();
        }
    }

    protected virtual void OnTimelineEnd() { }

    public void _OnSignal(SignalAsset signal)
    {
        //Debug.Log(signal.name);
        OnSignal(signal);
    }

    protected virtual void OnSignal(SignalAsset signal) { }

    public void RemoveDelegates()
    {
        m_playableDirector.stopped -= _OnPlayableDirectorStopped;
    }

    void OnDestroy()
    {
        RemoveDelegates();
    }

    [ContextMenu("Add Signal Reciever")]
    public void AddSignalReciever()
    {
        m_signals = Resources.LoadAll(m_signalResourcePath, typeof(SignalAsset));

        SignalReceiver reciever = this.gameObject.AddComponent<SignalReceiver>();

        for (int i = 0; i < m_signals.Length; i++)
        {
            var ts = m_signals[i] as SignalAsset;
            UnityEvent evt = new UnityEvent();
            UnityAction<SignalAsset> action = new UnityAction<SignalAsset>(_OnSignal);
            //UnityEditor.Events.UnityEventTools.RemovePersistentListener<SignalAsset>(evt, action);
            UnityEditor.Events.UnityEventTools.AddObjectPersistentListener<SignalAsset>(evt, action, ts);
            evt.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.EditorAndRuntime); // This one needs to add here instead of adding this after the UnityEvent instance.
            reciever.AddReaction(ts, evt);
        }

        BindSignalReciever(reciever);
    }

    private void BindSignalReciever(SignalReceiver reciever)
    {
        SetPlayableDirector();
        var binding = m_playableDirector.playableAsset.outputs.First(c => c.streamName.Contains("Signal"));
        m_playableDirector.SetGenericBinding(binding.sourceObject, reciever);
    }
}
