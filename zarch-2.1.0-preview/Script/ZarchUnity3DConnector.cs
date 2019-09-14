using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Threading;


namespace Z
{
    [ExecuteInEditMode]
    public class ZarchUnity3DConnector : MonoBehaviour
    {
        [HideInInspector]
        public Text console;

        [HideInInspector]
        public List<GameObject> prefabs = new List<GameObject>();

        #region loading

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnRuntimeMethodLoad()
        {
            Zarch.ReflectConfig.Assembly = ZarchReflectHelper.AssemblyType.Executing;

            instance.load_objects(); 

            #region register classes

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
            Zarch.classes.Register<PlayerPrefs>();
            Zarch.classes.Register<Behaviour>();
            Zarch.classes.Register<Shader>();
            Zarch.classes.Register<Motion>();
            Zarch.classes.Register<Thread>();

            Zarch.classes.Register<KeyCode>();

            #endregion

            Zarch.methods["help"] = instance.help;
            Zarch.methods["clear"] = instance.clear;
            Zarch.methods["objects"] = instance.show_objs;
            Zarch.methods["methods"] = instance.show_methods;
            Zarch.methods["classes"] = instance.show_classes;
            Zarch.methods["toUnityObj"] = instance.to_unity_object;
            Zarch.methods["new"] = obj => { return Instantiate(obj[0] as GameObject); };
            Zarch.methods["del"] = obj => { Destroy(obj[0] as GameObject); return null; };
            Zarch.methods["get"] = obj =>
            {
                string name = System.Convert.ToString(obj[0]);
                System.Type type = _get_type(name);
                return FindObjectOfType(type);
            };

            Zarch.classes["$"] = typeof(GoHelper);
            Zarch.objects["$"] = instance.task;
            
            
        }

        void load_objects()
        {
            var objs = Resources.FindObjectsOfTypeAll<GameObject>();

            for (int i = 0; i < objs.Length; i++)
            {
                try
                {
                    string key = objs[i].name.Replace(" ", "");

                    while (Zarch.objects.hasKey(key))
                    {
                        key += "0";
                    }

                    Zarch.objects[key] = objs[i];

                }
                catch { }
            }

            for (int i = 0; i < prefabs.Count; i++)
            {
                string key = prefabs[i].name.Replace(" ", "");

                while (Zarch.objects.hasKey(key))
                {
                    key += "0";
                }
                try
                {
                    Zarch.objects[key] = prefabs[i];
                }
                catch { }
            }
        }

        // type 
        object to_unity_object(object[] objs) { return objs[0] as Object; }

        #endregion

        #region console

        object help(object[] pms)
        {
            Zarch.code = "print('$(go)/$(nameStr)获得魔法对象, help()总体功能概览$.help()任务功能概览,objects()遍历框架内所有游戏对象,methods()所有方法,classes()遍历所有手动注入的类,clear()清空屏幕，" +
                "魔法对象的方法：$(go).move()移动, .rotate旋转，.click设置点击事件的回调， .get获得组件，.add增加组件，.del销毁组件，.active切换激活状态（组件和物体皆可），" +
                ".tag获得或设置标签，.layer获得/设置层，.mat获得/设置材质，.tex获得/设置主贴图，.attr获得/设置当前材质中shader的参数，.image设置/获取图片组件的内容，.text设置/获取文本组件的内容，.slider设置/获取滑动条的值，" +
                "魔法对象的对象：.parent父物体GameObject，.childred.get(index)子物体GameObject ')";
            return null;
        }


        object clear(object[] pms)
        {
            if (instance.console != null)
                instance.console.text = "";
            return null;
        }


        object show_objs(object[] pms)
        {
            var objs = Zarch.objects.tree;
            foreach (var item in objs)
            { Zarch.code = "print(\'" + item + ", \');"; }
            return null;
        }

        object show_classes(object[] pms)
        {
            var cls = Zarch.classes.tree;
            foreach (var item in cls)
            { Zarch.code = "print(\'" + item + ", \');"; }
            return null;
        }

        object show_methods(object[] mds)
        {
            var ms = Zarch.methods.tree;
            foreach (var item in ms)
            { Zarch.code = "print(\'" + item + ", \');"; }
            return null;
        }

        #endregion

        #region init

        static bool isInit = false;
        static ZarchUnity3DConnector _instance;
        public static ZarchUnity3DConnector instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<ZarchUnity3DConnector>();

                if (!isInit)
                    if (_instance.console != null) { Zarch.LogDelegate = (string msg) => { _instance.console.text += ">> " + msg; }; isInit = true; }

                return _instance;
            }
        }
        private ZarchUnity3DConnector() { }

        #endregion

        #region magic object $

        // $(go).func()
        public class GoHelper
        {
            GameObject go;


            // constructor
            public GoHelper(GameObject _go) { go = _go;}

            public GoHelper(string _obj_key) { go = Zarch.objects[_obj_key] as GameObject; }

            // component
            public Component get(System.Type _type) { return go.GetComponent(_type); }

            public Component get(string _type) { return go.GetComponent(_type); }


            public Component add(System.Type _type) { return go.AddComponent(_type); }

            public Component add(string _type) { return go.AddComponent(_get_type(_type)); }
            

            public void del(System.Type _type) { if (go.GetComponent(_type)) { Destroy(go.GetComponent(_type)); } }

            public void del(string _type) { if (go.GetComponent(_type)) { Destroy(go.GetComponent(_type)); } }


            public void active(System.Type _type, bool state) { var r = go.GetComponent(_type) as MonoBehaviour; if (r) { r.enabled = state; } }

            public void active(string _type, bool state) { var r = go.GetComponent(_type) as MonoBehaviour; if (r) { r.enabled = state; } }


            public void active(bool _state) { go.SetActive(_state); }       // go 

            // transform
            public void move(double x, double y, double z) { go.transform.Translate((float)x, (float)y, (float)z); }

            public void move(Vector3 vector) { go.transform.Translate(vector); }

            public void rotate(double x, double y, double z) { go.transform.Rotate((float)x, (float)y, (float)z); }

            


            public void pos(Vector3 pos) { go.transform.position = pos; }

            public void pos(double x, double y, double z) { go.transform.position = new Vector3((float)x, (float)y, (float)z); }

            public Vector3 pos() { return go.transform.position; }


            public void rot(Vector3 rot) { go.transform.eulerAngles = rot; }

            public void rot(double x, double y, double z) { go.transform.eulerAngles = new Vector3((float)x, (float)y, (float)z); }

            public void rot(Quaternion quaternion) { go.transform.rotation = quaternion; }

            public Vector3 rot() { return go.transform.eulerAngles; }
            

            public GameObject parent
            {
                set { go.transform.SetParent(value.transform); }
                get { return go.transform.parent.gameObject; }
            }

            public ArrayHelper<GameObject> children
            {
                get
                {
                    List<GameObject> meta = new List<GameObject>();
                    for (int i = 0; i < go.transform.childCount; i++) { meta.Add(go.transform.GetChild(i).gameObject); }
                    return new ArrayHelper<GameObject>(meta);
                }
            }


            // tag and layer
            public void tag(string tag) { go.tag = tag; }

            public string tag() { return go.tag; }

            public void layer(string layerName) { go.layer = LayerMask.NameToLayer(layerName); }

            public string layer() { return LayerMask.LayerToName(go.layer); }

            // mat and tex
            public void mat(Material mt) { go.GetComponent<Renderer>().material = mt; }

            public Material mat() { return go.GetComponent<Renderer>().material; }

            public void tex(Texture t) { go.GetComponent<Renderer>().material.mainTexture = t; }

            public Texture tex() { return go.GetComponent<Renderer>().material.mainTexture; }

            // shader
            public void attr(string name, float value) { go.GetComponent<Renderer>().material.SetFloat(name, value); }

            public float attr(string name) { return go.GetComponent<Renderer>().material.GetFloat(name); }

            // ui
            public void image(Sprite s) { var r = go.GetComponent<Image>(); if (r == null) { r = go.GetComponentsInChildren<Image>()[0]; } if (r) { r.sprite = s; } else { go.GetComponentInChildren<Image>().sprite = s; } }

            public Sprite image() { var r = go.GetComponent<Image>(); if (r == null) { r = go.GetComponentsInChildren<Image>()[0]; } if (r) { return r.sprite; } return null; }

            public void text(string txt) { var r = go.GetComponent<Text>(); if (r == null) { r = go.GetComponentsInChildren<Text>()[0]; } if (r) { r.text = txt; } else { go.GetComponentInChildren<Text>().text = txt; } }

            public string text() { var r = go.GetComponent<Text>(); if (r == null) { r = go.GetComponentsInChildren<Text>()[0]; } if (r) { return r.text; }  return ""; }

            public void slider(float progress) { var r = go.GetComponent<Slider>(); if (r == null) { r = go.GetComponentsInChildren<Slider>()[0]; } if (r) { r.value = progress; } else { go.GetComponentInChildren<Slider>().value = progress; } }

            public float slider() { var r = go.GetComponent<Slider>(); if (r == null) { r = go.GetComponentsInChildren<Slider>()[0]; } if (r) { return r.value; }  return 0f; }

            // event trigger
            public void click(System.Action onclick)
            {
                if (go.GetComponent<RectTransform>() == null) // 不是UI 是模型
                {
                    // 物体先加碰撞
                    var bxcollider = go.GetComponent<BoxCollider>();

                    if (bxcollider == null)
                        bxcollider = go.AddComponent<BoxCollider>();

                    // 摄像头加射线发射
                    var cms = FindObjectsOfType<Camera>();

                    foreach (var item in cms)
                    {
                        if (item.gameObject.GetComponent<PhysicsRaycaster>())
                            continue;

                        item.gameObject.AddComponent<PhysicsRaycaster>();

                    }
                 }

                // eventtrigger里回调方法的容器
                var zevent = go.GetComponent<ZarchEvent>();

                if (zevent)
                    zevent.zclick = onclick;
                else
                {
                    zevent = go.AddComponent<ZarchEvent>();
                    zevent.zclick = onclick;
                }

                // 加入eventtrigger
                var et = go.GetComponent<EventTrigger>();

                if (et == null)
                    et = go.AddComponent<EventTrigger>();

                
                EventTrigger.Entry entry = new EventTrigger.Entry();            // trigger 的 单元粒
                
                entry.eventID = EventTriggerType.PointerClick;                  // 事件类型
                
                UnityEngine.Events.UnityAction<BaseEventData> click_event =
                    new UnityEngine.Events.UnityAction<BaseEventData>(zevent.Z_Click);
                
                entry.callback.AddListener(click_event);                              // 事件回调
                
                et.triggers.Clear();

                et.triggers.Add(entry);                                         // 加到trigger里

            }

            public void click(System.Func<object[], object> func) { click(delegate { func(null); }); }

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
            public ArrayHelper(List<T> _meta) { meta = _meta; }
        }


        static System.Type _get_type(string name) { try { return Zarch.classes[name]; } catch { return Zarch.objects.reflectHelper.GetType(Zarch.ReflectConfig.Assembly, name); } }


        #region coroutine and thread

        public IEnumerator _coroutine(System.Action action, float delay = 0, int isLoop = 0)
        {
            if (isLoop == 0)
            {
                yield return new WaitForSeconds(delay);
                action();
            }
            else if (isLoop > 0)
            {
                for (int i = 0; i < isLoop; i++)
                {
                    action();

                    if (!delay.Equals(0))
                        yield return new WaitForSeconds(delay);
                }
            }
            else
            {
                while (true)
                {
                    action();

                    if (!delay.Equals(0))
                        yield return new WaitForSeconds(delay);
                }
            }
            
            yield return null;
        }

        public IEnumerator _DelayCall(System.Action action, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            action();
        }

        // $.coroutine()/thread()
        public class TaskHelper
        {
            public void coroutine(System.Func<object[], object> func, object delay, object isLoop)
            {
                instance.StartCoroutine(instance._coroutine(() => { func(null); }, (float)System.Convert.ToDouble(delay), System.Convert.ToInt32(isLoop)));
            }

            #region 备用协程

            public void coroutine1(System.Func<object[], object> func, object delay, object isLoop)
            {
                instance.callStop1 = true;
                float _delay = (float)System.Convert.ToDouble(delay);
                int _loop = System.Convert.ToInt32(isLoop);
                instance.tmp_coroutines1.Add(func);
                instance.tmp_params1.Add(new KeyValuePair<float, int>(_delay, _loop));
            }

            #endregion

            public void thread(System.Func<object[], object> func, System.Func<object[], object> callback)
            {
                Thread t = new Thread(new ThreadStart(delegate { func(null);
                    ThreadBridge.Invoke((delegate { callback(null); })); Thread.CurrentThread.Abort(); }));

                t.Start();
            }

            public void reset() { instance.callStopAll = true; }

            public void help() { Zarch.code = "" +
                    "print('$.get(url)加载http字符串,是阻塞方法，你可以把它放在线程里。$.thread(委托，回调)在新线程执行任务，然后回到主线程中执行回调。" +
                    "$.coroutine(委托,发起间隔或延迟，循环次数)调用协程，循环次数小于0为while(true)，0次时为延迟调用，$.coroutine1，$.coroutine2，$.coroutine3备用，" +
                    "在一些版本的Unity中，多次触发同一个协程可能会报错。')"; }

            public string get(string url) { return ZarchHttpClient.RequestGetData(url); }
        }
        public TaskHelper task = new TaskHelper();


        #region 备用协程

        List<System.Func<object[], object>> tmp_coroutines1 = new List<System.Func<object[], object>>();
        List<KeyValuePair<float, int>> tmp_params1 = new List<KeyValuePair<float, int>>();

        #endregion

        bool callStop1;
     
        bool callStopAll;

        void Update()
        {
            if (callStop1) { StopCoroutine("CoroutineTask1"); callStop1 = false; }
            
            if (callStopAll) { StopAllCoroutines(); callStopAll = false; }

            #region 备用协程

            if (tmp_coroutines1.Count != 0)
            {
                for (int i = 0; i < tmp_coroutines1.Count; i++)
                {
                    var pms = tmp_params1[i];
                    var func = tmp_coroutines1[i];

                    StartCoroutine(CoroutineTask1(func, pms.Key, pms.Value));

                    tmp_coroutines1.Remove(func);
                    tmp_params1.Remove(pms);
                }
            }

            #endregion
        }

        #region 备用协程

        IEnumerator CoroutineTask1(System.Func<object[], object> action, float delay, int loopTimes)
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

        #endregion


        #endregion

        #endregion

    }

    #region extensions

    public static class ZarchUnityExtension
    {
        public static void S_coroutine(this Zarch.Extension ze, System.Action action, float delay = 0, int loopTimes = 0)
        {
            ZarchUnity3DConnector.instance.task.coroutine((pms) => { action(); return null; }, delay, loopTimes);
        }

        public static void S_coroutine1(this Zarch.Extension ze, System.Action action, float delay = 0, int loopTimes = 0)
        {
            ZarchUnity3DConnector.instance.task.coroutine1((pms) => { action(); return null; }, delay, loopTimes);
        }

        public static void S_thread(this Zarch.Extension ze, System.Action action, System.Action callback)
        {
            ZarchUnity3DConnector.instance.task.thread(pms => { action(); return null; }, pms => { callback(); return null; });
        }

        public static ZarchUnity3DConnector.GoHelper S(this Zarch.Extension ze, GameObject go)
        {
            return new ZarchUnity3DConnector.GoHelper(go);
        }

        public static ZarchUnity3DConnector.GoHelper S(this Zarch.Extension ze, string goName)
        {
            return new ZarchUnity3DConnector.GoHelper(GameObject.Find(goName));
        }

        public static void S_StartCoroutine(this Zarch.Extension ze, IEnumerator enumerator)
        {
            ZarchUnity3DConnector.instance.StartCoroutine(enumerator);
        }

        public static void S_DelayCall(this Zarch.Extension ze, System.Action action, float delay_seconds)
        {
            ZarchUnity3DConnector.instance.StartCoroutine(ZarchUnity3DConnector.instance._DelayCall(action, delay_seconds));
        }

        #endregion

    }
}
