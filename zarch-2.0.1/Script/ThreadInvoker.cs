using UnityEngine;
using System.Collections.Generic;


public class ThreadInvoker : MonoBehaviour
{
    static ThreadInvoker _instance;

    public static void InvokeInMainThread(System.Action _delegate)
    {
        if (_instance == null)
        {
            var obj = FindObjectOfType<ThreadInvoker>();

            if (obj == null)
            {
                var _obj = new GameObject("Invoker", typeof(ThreadInvoker));
                _instance = _obj.GetComponent<ThreadInvoker>();
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
