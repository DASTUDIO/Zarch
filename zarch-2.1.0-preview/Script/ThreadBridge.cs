using UnityEngine;
using System.Collections.Generic;


public class ThreadBridge : MonoBehaviour
{
    static ThreadBridge _instance;

    public static void Invoke(System.Action _delegate)
    {
        if (_instance == null)
        {
            var obj = FindObjectOfType<ThreadBridge>();

            if (obj == null)
            {
                var _obj = new GameObject("Invoker", typeof(ThreadBridge));
                _instance = _obj.GetComponent<ThreadBridge>();
            }
            else
            {
                _instance = obj;
            }
        }

        _instance.delegates.Add(_delegate);

    }

    List<System.Action> delegates = new List<System.Action>();

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        Execute();
    }

    void Execute()
    {
        if (delegates.Count == 0)
            return;

        for (int i = 0; i < delegates.Count; i++)
        {
            try
            {
                delegates[i]();
            }
            catch(System.Exception e) { Debug.LogError(e); }
        }

        delegates.Clear();

    }

}
