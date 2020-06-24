using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTimeline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(TestTimelineController.GetInstance() != null)
        {
            TestTimelineController.GetInstance().OnSignalTriggered += (TestSignal signal) =>
            {
                Debug.Log(signal.name);
            };
        }

        if (TestLuaTimelineController.GetInstance() != null)
        {
            TestLuaTimelineController.GetInstance().OnSignalTriggered += (TestSignal signal) =>
            {
                Debug.Log(signal.name);
            };
        }
    }
}
