using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using Z;

public class ZarchUnity3DConnector : MonoBehaviour
{
    [SerializeField]
    public Text console;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        Zarch.ReflectConfig.Assembly = ZarchReflectHelper.AssemblyType.Executing;

        load_objects();

        Zarch.classes.Register<Vector2>();
        Zarch.classes.Register<Vector3>();
        Zarch.classes.Register<Vector4>();
        Zarch.classes.Register<Quaternion>();

        Zarch.classes.Register<GameObject>();
        Zarch.classes.Register<Camera>();

        Zarch.classes.Register<Light>();

        Zarch.classes.Register<Texture>();
        Zarch.classes.Register<Texture2D>();
        Zarch.classes.Register<Material>();
        Zarch.classes.Register<CustomRenderTexture>();
        Zarch.classes.Register<Renderer>();
        Zarch.classes.Register<Sprite>();
        Zarch.classes.Register<SpriteRenderer>();
        Zarch.classes.Register<SpriteMask>();

        Zarch.classes.Register<RectTransform>();
        Zarch.classes.Register<RectTransformUtility>();
        Zarch.classes.Register<Text>();
        Zarch.classes.Register<Image>();
        Zarch.classes.Register<RawImage>();
        Zarch.classes.Register<Mask>();
        Zarch.classes.Register<Button>();
        Zarch.classes.Register<Toggle>();
        Zarch.classes.Register<Slider>();
        Zarch.classes.Register<Scrollbar>();
        Zarch.classes.Register<Dropdown>();
        Zarch.classes.Register<InputField>();
        Zarch.classes.Register<ScrollRect>();

        Zarch.classes.Register<Transform>();
        Zarch.classes.Register<Rigidbody>();
        Zarch.classes.Register<Collider>();
        Zarch.classes.Register<Collision>();
        Zarch.classes.Register<SphereCollider>();
        Zarch.classes.Register<MeshCollider>();
        Zarch.classes.Register<WheelCollider>();
        Zarch.classes.Register<Collider2D>();
        Zarch.classes.Register<Rigidbody2D>();
        Zarch.classes.Register<Collision2D>();
        Zarch.classes.Register<EdgeCollider2D>();
        Zarch.classes.Register<ColliderDistance2D>();
        
        Zarch.classes.Register<Joint>();
        Zarch.classes.Register<Joint2D>();
        Zarch.classes.Register<JointMotor>();
        Zarch.classes.Register<JointMotor2D>();
        Zarch.classes.Register<JointDrive>();
        Zarch.classes.Register<JointLimits>();
        Zarch.classes.Register<JointAngleLimits2D>();
        Zarch.classes.Register<JointSpring>();
        Zarch.classes.Register<JointSuspension2D>();
        Zarch.classes.Register<Ray>();

        Zarch.classes.Register<CharacterController>();

        Zarch.classes.Register<AudioClip>();
        Zarch.classes.Register<AudioListener>();
        Zarch.classes.Register<AudioSource>();

        Zarch.classes.Register<Animation>();
        Zarch.classes.Register<AnimationClip>();
        Zarch.classes.Register<Animator>();
        Zarch.classes.Register<Avatar>();

        Zarch.classes.Register<Navigation>();

        Zarch.classes.Register<Mesh>();
        Zarch.classes.Register<Time>();
        Zarch.classes.Register<Random>();
        Zarch.classes.Register<Mathf>();
        Zarch.classes.Register<Application>();
        Zarch.classes.Register<Input>();
        Zarch.classes.Register<GUI>();
        Zarch.classes.Register<Physics>();
        Zarch.classes.Register<Physics2D>();
        Zarch.classes.Register<Resources>();
        Zarch.classes.Register<AssetBundle>();
        Zarch.classes.Register<Debug>();

        Zarch.classes.Register<Object>();
        Zarch.classes.Register<Component>();
        Zarch.classes.Register<MonoBehaviour>();

        Zarch.classes.Register<ParticleSystem>();
        Zarch.classes.Register<Behaviour>();
        Zarch.classes.Register<Shader>();
        Zarch.classes.Register<Motion>();
        Zarch.classes.Register<Thread>();

        Zarch.methods["help"] = help;
        Zarch.methods["clear"] = clear;
        Zarch.methods["objects"] = show_objs;
        Zarch.methods["methods"] = show_methods;
        Zarch.methods["classes"] = show_classes;
        Zarch.methods["toUnityObj"] = to_unity_object;

        Zarch.classes["$"] = typeof(GoHelper);
        Zarch.objects["$"] = instance.task;

    }

    static void load_objects()
    {
        var objs = Resources.FindObjectsOfTypeAll<GameObject>();

        for (int i = 0; i < objs.Length; i++)
        {
            try
            {
                string key = objs[i].name.Replace(" ","");

                while (Zarch.objects.hasKey(key))
                {
                    key += "0";
                }

                Zarch.objects[key] = objs[i];

            }
            catch { }
        }
    }

    static object help(object[] pms)
    {
        Zarch.code = "print('$(someGameObject)调用快捷方法(对象和字符串都可以), help()与$.help()查看帮助, 后者是协程和线程功能,objects()查看可使用的游戏对象,methods()查看可使用的方法,classes()查看当前注入的类,clear()清空屏幕')";
        return null;
    }

    
    static object clear(object[] pms)
    {
        if(instance.console != null)
            instance.console.text = "";
        return null;
    }


    static object show_objs(object[] pms)
    {
        var objs = Zarch.objects.tree;
        foreach (var item in objs)
        { Zarch.code = "print(\'" + item + ", \');"; }
        return null;
    }

    static object show_classes(object[] pms)
    {
        var cls = Zarch.classes.tree;
        foreach (var item in cls)
        { Zarch.code = "print(\'" + item + ", \');"; }
        return null;
    }

    static object show_methods(object[] mds)
    {
        var ms = Zarch.methods.tree;
        foreach (var item in ms)
        { Zarch.code = "print(\'" + item + ", \');"; }
        return null;
    }

    static object to_unity_object(object[] objs) { return objs[0] as Object; }
    static object get(string key) { return Zarch.objects[key]; }

    private ZarchUnity3DConnector() { }
    static ZarchUnity3DConnector _instance;
    public static ZarchUnity3DConnector instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ZarchUnity3DConnector>();
            return _instance;
        }
    }

    private void Awake() {
        _instance = this;
        if (instance.console != null)
            Zarch.LogDelegate = (string msg) => { instance.console.text += ">> " + msg; };
    }

    // $(go).func
    public class GoHelper
    {
        GameObject go;

        public GoHelper(GameObject _go) { go = _go; }

        public GoHelper(string _obj_key) { go = Zarch.objects[_obj_key] as GameObject; }

        public Component get(System.Type _type) { return go.GetComponent(_type) as Component; }

        public Component add(System.Type _type) { var res = go.AddComponent(_type); return res; }

        public void move(double x, double y, double z) { go.transform.Translate((float)x, (float)y, (float)z); }
        public void move(Vector3 vector) { go.transform.Translate(vector); }

        public void rotate(double x, double y, double z) { go.transform.Rotate((float)x, (float)y, (float)z); }
        public void rotate(Quaternion quaternion) { go.transform.rotation = quaternion; }

        public void active(bool _state) { go.SetActive(_state); }

        public GameObject parent {
            set { go.transform.SetParent(value.transform); }
            get { return go.transform.parent.gameObject; } }

        public ArrayHelper<GameObject> children
        {
            get
            {
                List<GameObject> meta = new List<GameObject>();
                for (int i = 0; i < go.transform.childCount; i++) { meta.Add(go.transform.GetChild(i).gameObject); }
                return new ArrayHelper<GameObject>(meta);
            }
        }
    }

    public class ArrayHelper<T>
    {
        List<T> meta;
        public T get(int _index) { return meta[_index]; }
        public void set(int _index, T _value) { meta[_index] = _value; }
        public int add(T t) { meta.Add(t); return meta.IndexOf(t); }
        public void del(T t) { meta.Remove(t); }
        public int len() { return meta.Count; }
        public int index(T t) { return meta.IndexOf(t); }
        public void reset() { meta.Clear(); }
        public ArrayHelper(List<T> _meta) { meta = _meta;  }
    }

    // $.coroutine()/thread()
    public class TaskHelper
    {
        public void coroutine(System.Func<object[], object> func, object delay, object isLoop) {
            reset();
            float _delay = (float)System.Convert.ToDouble(delay);
            int _loop = System.Convert.ToInt32(isLoop);
            ZarchUnity3DConnector.instance.tmp_coroutines.Add(func);
            ZarchUnity3DConnector.instance.tmp_params.Add(new KeyValuePair<float, int>(_delay, _loop)); }

        public void thread(System.Func<object[], object> func, System.Func<object[], object> callback) {
            Thread t = new Thread(new ThreadStart(delegate { func(null);
                ThreadInvoker.InvokeInMainThread(delegate { callback(null); }); Thread.CurrentThread.Abort(); }));
            t.Start(); }

        public void reset() { instance.callStopCall = true; }

        public void help() { Zarch.code = "print('使用$.coroutine(委托,延时，循环次数)调用协程，用$.thread(委托，回调)调用线程任务,新的协程启动时，之前的会被取消')"; }

    }
    public TaskHelper task = new TaskHelper();


    List<System.Func<object[], object>> tmp_coroutines = new List<System.Func<object[], object>>();
    List<KeyValuePair<float, int>> tmp_params = new List<KeyValuePair<float, int>>();
    bool callStopCall = false;
    void Update()
    {
        if (callStopCall) {StopAllCoroutines(); callStopCall = false; }

        if (tmp_coroutines.Count == 0) { return; }
        for (int i = 0; i < tmp_coroutines.Count; i++)
        {
            var pms = tmp_params[i];
            var func = tmp_coroutines[i];

            StartCoroutine(CoroutineTask(func,pms.Key,pms.Value));

            tmp_coroutines.Remove(func);
            tmp_params.Remove(pms);
        } 
    }
    IEnumerator CoroutineTask(System.Func<object[],object> action, float delay , int loopTimes )
    {
        if (loopTimes < 0) // looper
        {
            while (true)
            {
                action(null);
                if (delay.Equals(0)) { yield return null; } else { yield return new WaitForSeconds(delay); }
            }
        }
        else if (loopTimes == 0) // delay call
        {
            if (delay.Equals(0)) { yield return null; } else { yield return new WaitForSeconds(delay); }
            action(null);
        }
        else // repeater
        {
            for (int i = 0; i < loopTimes; i++)
            {
                action(null);
                if (delay.Equals(0)) { yield return null; } else { yield return new WaitForSeconds(delay); }
            }
        }
    }
}

public static class ZarchUnityExtension {
    public static void StartCoroutine(this Zarch.Extension extension, System.Action action, float delay = 0, int loopTimes = 0) {
        ZarchUnity3DConnector.instance.task.coroutine((pms) => { action(); return null; }, delay, loopTimes); }

    public static void StartThread(this Zarch.Extension extension, System.Action action, System.Action callback) {
        ZarchUnity3DConnector.instance.task.thread(pms => { action(); return null; }, pms => { callback(); return null; }); }

}
