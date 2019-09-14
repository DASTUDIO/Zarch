---

一块钱也能做网站？[微服务器](https://src.pub) 用简单的操作搭建网站，设计web接口，不是模版，代码自由，买页面送域名。

---
 
# Zarch语言 2.0.2

## 运行zarch代码
```csharp
Zarch.code = "Debug.Log('hello world')";                 // c# code
```

## 准备

>把ZarchConnector.prefab拖入场景，使用命名空间`using Z;` ，之后 **赋值即运行**。 

>**直接用[场景中物体名]就可以对物体操作**，例如`Debug.Log(myCube.name)`.



## Get Started：

### 物体操作

移动物体

```js
$(myCube).move(1,2,3)
```

旋转物体

```js
$(myCube).rotate(10,20,30)
```
获取物体位置

```js
$(myCube).pos()
```

设定物体位置

```js
$(myCube).pos(0,0,0)
```

获取物体旋转角度

```js
$(myCube).rot()
```

设定物体旋转角度

```js
$(myCube).rot(0,0,0)
```

父物体

```js
$($(child).parent).move(1,2,3)
```

子物体

```js
$($(parent).children.get(0)).move(1,2,3)
```

显示/隐藏 物体

```js
$(myCube).active(bool(0))
```

获取 tag

```js
$(myCube).tag()
```

设置 tag

```js
$(myCube).tag('myTag')
```

获取层

```js
$(myCube).layer()
```

设置层

```js
$(myCube).layer('myLayer')
```


销毁物体

```js
del(myCube)   
```


创建物体 

```js
new(ball)             //ball是提前拖入ZarchConnector中的prefab

```
>**将prefab预制体拖入ZarchConnector中**
> 
>ZarchUnity3DConnector脚本的prefabs列表里，就可以中用new(名称)实例化它到场景。




### 组件

全局查找&获取组件

```js
x = get('Transform')
```

获取组件


```js
$(myCube).get('Transform').Translate(1,2,3)
```

添加组件

```js
$(myCube).add('Rigidbody').AddForce(Vector3.up)
```


销毁组件

```js
$(myCube).del('MeshRenderer')
```

激活/关闭 组件

```js
$(mybtn).active('Image',b)     # b是之前定义的布尔值 b = bool(0)
```

### 图形

获取材质

```js
$(myCube).mat()
```

设置材质

```js
$(myCube).mat($(mc).mat())
```

获取主贴图

```js
$(myCube).tex()
```

设置主贴图

```js
$(mc).tex($(myCube).tex())
```

获取shader参数

```js
$(myCube).attr('_Glossiness')
```

设置shader参数

```js
$(myCube).attr('_Glossiness',100)
```

### UI

获取 Image Sprite内容

```js
$(myImage).image()
```

设定 Image Sprite内容

```js
$(target).image($(origin).image())
```

获取 Text 文本内容

```js
$(myText).text()
```

设定 Text 文本内容

```js
$(target).text($(origin).text())
```

获取 Slider 滑动位置

```js
$(mySlider).slider()
```

设置 Slider 滑动位置

```js
$(target).slider($(origin).slider())
```



### 事件 与 任务
绑定点击事件

```js
$(mybtn).click({ print('123') })
```

协程任务

```js
$.coroutine({ $(myCube).move(1,2,3) }, 0.5, 5)
```

>（每0.5秒执行一次移动，一共执行5次）

>格式：`$.coroutine([方法委托],[触发间隔],[循环次数])`

>当循环次数小于0时 无限循环

>当循环次数等于0时 等待\[触发间隔\]秒后，再执行\[方法委托\]

>如果你的Unity版本不支持多次触发同一个协程，还有$.coroutine1().$.cotourine2(),$.coroutine3()可以使用。

线程任务


```js
$.thread( { Thread.Sleep(3000); }, { $(myCube).move(1,2,3) })
```

>(在线程任务sleep()完成后回到主线程移动游戏物体myCube)

>格式：`$.thread([新线程执行的方法],[完成后回到主线程执行的回调])`


网络任务


```js
$.get('http://baidu.com')
```

>格式：`$.get([请求地址])`
>直接返回字符串

>该方法为阻塞方法，可以配合线程使用

>```js
>url = 'http://baidu.com/';
>t = { res = $.get(url) };
>$.thread( t, { print(res) } )
>```


### 控制台功能

基本输出

```js
print('hello world')
```

帮助

```js
help()
```
任务功能帮助

```js
$.help()
```


查看全部可用对象

```js
objects()
```
查看全部自定义方法

```js
methods()
```

查看全部注册进来的类（功能）

```js
classes()
```

清空屏幕

```js
clear()
```


### 稳定性
把字符串定义在外面
>如果你用到代码块{}方法委托，建议代码块大括号里不要这样{'someStr'}定义字符串
>而是**把字符串定义在外面**，这样会得到更稳定的代码和更小的资源占用。

>```js
>url = 'http://baidu.com/';
>t = { res = $.get(url) };
>$.thread( t, { print(res) } )
>```

把布尔值定义在外面
>如果使用布尔值 不要直接使用bool() 而是**把布尔值定义在外面**保存成对象使用

>```js
>n = bool(0)
>```
>然后使用对象
>
>```js
>$(mybtn).active('Image',n)
>```

用外部对象传递参数

>在zarch中最常见的自定义委托是代码块 {...}

>```js
>x = { print(1); }
>```

>这种情况下，传递运行结果到外部用的是对象。

>```js
>a=1;
>x = { a = int(a) + 1; };
>for(0, 1, 10, x);
>```


## 在C#中使用魔法对象 $

>你可以在C#中通过`Zarch.csharp.S`直接调用魔法对象


zarch code:

```js
$(myCube).move(1,2,3)
```
c# code

```csharp
Zarch.csharp.S(myCube).move(1,2,3)
```

## 其他功能

方法委托

>无参数

>```python
>d = { print('hello') };       #定义
>d(); 						   #执行
>```
>有参数

>```python
>d2 = [Debug.Log];     #定义
>d2('Hi');            #执行

>```


数据类型
>内置以下类型的转换 ：>int(),float(),double(),bool(),str(),toUnityObj()
>```js
>a = int(b);
>```
>字符串 一律使用单引号

>```js
>mystr = 'hello';
>```
>列表

>```js
>mylist = list(1,2,3,4,5)
>```
>布尔值

>```python
># bool() 返回 false
># bool(int) 大于0 true 小于等于0 false
># bool(obj) 不为空时返回true 为空返回false
># bool(a,b) 相同 true 不同 false , a、b 可以是string、int、float、double 
>```
>null

>```js
>false = bool(null())
>```

流程控制

>通过函数实现流程控制

>if(bool, trueDelegate, falseDelegate, params1, >params2...)
>
>```js
>a = {...};
>b = {...};
>if(bool(...), a, b);
># 如果有参数 可以加在后面(...a,b,param1,param2...)

>```

>for(start, step, end, delegate, param1, param2...)
```js
c = {...}
for(1,1,100,c);
# 如果有参数 可以加在后面(...100,c,param1,param2...)
```
>详情见下方1.0.4版本手册

## 进阶

### 手动注入
* 1.外部的对象，方法，和类在注入后才可以在脚本代码中使用。
* 2.注入对象或类后，其成员方法和字段属性也可以访问。
#### 注入对象
```csharp
Zarch.objects["test"] = new Test();
```
对应的在zarch中的使用为
```js
test.someMethod();
```
#### 注入方法
```csharp
Zarch.methods["myfun"] = dlt;              // dlt类型为 System.Func<object[],object>
```
对应的在zarch中的使用为

```js
myfun(p1,p,...);
```

#### 注入类

```csharp
Zarch.classes.Register<Thread>();
```
对应的在zarch中使用为

```python
# new
t = Thread(...);

# static method or field or property
Thread.CurrentThread.Abort();
```

>从2.0.1开始，使用Unity中的自建脚本不再需要注入

>~~如果是你自己写的类，你也可以通过在类声明上方加[ZarchClass]来实现注入~~
>```csharp
>[ZarchClass]
>public class MyClass : MonoBehaviour {
>        void Start(){}
>        void Update(){}
>}

>```

#### 配置反射程序集 

```csharp
Zarch.ReflectConfig.Assembly = ZarchReflectHelper.AssemblyType.Executing;
```
>详情见1.0.4早期版本手册


#### 复杂工程
>如果你的工程规模很大，很复杂，那我建议你不要把各种引用拖来拖去，也不要频繁地Find物体,而是**使用统一的依赖管理**功能。

>```csharp
>// 如果你使用1.0的实体类自动依赖注入功能 你需要查阅1.0.4版本手册来 了解这些功能
>Zarch.object[] 
>Zarch.init();
>Zarch.Refresh();
>// 以及自动注入Attribute
>[ZarchBean] [ZarchBean(params...)]

>```

#### c# 调用 zarch code 内部的成员
```csharp
// 如果你需要在脚本外调用 你需要查阅1.0.4版本手册来 了解这些功能
Zarch.call();
Zarch.CreateDelegate()
```

## 附录

#### 控制台

>定位输出：将用以输出的Text组件拖入场景里 ZarchConnector物体中 ZarchUnity3DConnector组件 的console字段 中

#### 为Zarch增加扩展
>你可以把以下代码写在任何地方

```csharp
public static class 类名随意 {
    public static void 你的方法名(this Zarch.Extension extension, 你的参数1，你的参数2...) { 要做的事; }
}
```
>写好后 可以通过这样调用

```csharp
Zarch.csharp.你的方法名(你的参数1，你的参数2);
```


#### 开启新功能
>如果发现需要的Unity功能未在zarch code用开启，
>请自行前往 plugins/zarch/script/ZarchUnity3DConnector.cs中，仿照其他Unity功能的注册自行注册.

>例:添加Slider

>```js
>Zarch.classes.Register<Slider>();
>```


## 版本更新

2.1.0-preview 增加 Unity Editor界面，由传统的拖放connector prefab改为Editor panel中点击按钮初始化,优化 $().text()/image()/slider()体验，优化协程流畅度， 内部集成了线程通信对用户透明的网络功能，byte[]级别，支持ipv6. 

2.0.2 修复方法名解析bug， 内核升级 ：允许代码块委托嵌套。

```js
{ ...;$.corotine({...},0,0);...}
```

2.0.1 增加在C#中使用魔法对象的扩展，增加$().click万能事件绑定,$.get()网络加载功能，
>新增$().tag/.layer/.mat/.tex/.attr/.image/.text/.slider,备用协程123，内核升级：自写脚本免注入。

2.0.0 全新接口，面向Unity3D强化，修正不安全字符，优化对象回收，向后兼容。
```
新增内容：

Extension功能

优化注入
Zarch.classes.Register<T>()来替代Zarch.classes['someClass'] = type(someClass)
[ZarchClass]来替代Zarch.classes['someClass'] = type(someClass)，放在类声明的上方。

静态字段
对类的静态字段和静态属性的支持

Unity组件
Unity连接器prefab，自动注入场景里全部GameObject，自动注入常用的Unity内置功能。外部用拖入Text组件定向输出。

魔法对象
魔法对象$，大幅简化脚本代码。

```

>1.0.4 demo 更新了一个unitypackage,它展示了在Unity3D中：

>如何使用UGUI的InputField组件执行C#(Zarch)代码,并控制场景中的物体。

>demo界面：
>![demo界面](https://raw.githubusercontent.com/DASTUDIO/Zarch/master/img/idle.png)

>控制光照：
>![控制光照](https://raw.githubusercontent.com/DASTUDIO/Zarch/master/img/light.png)

>获取类型：
>![获取类型](https://raw.githubusercontent.com/DASTUDIO/Zarch/master/img/type.png)


1.0.4 修复在调用成员方法中遇见重载出现异常的Bug，新增了对类方法的支持,zarch code新增了type方法用以获取注入的class的Type(Unity中常用),C# 新增了tree查看注入的所有对象。

1.0.3.1 修复负数bug(仅源码) 

1.0.3 修复已知Bug，增加实例化对象功能，增加支持代码块、取委托，增加包含if和for在内的十个内置方法。

1.0.2 修复已知Bug，增加可赋值类型

1.0.1 增加简单自定义赋值功能

1.0.0 依赖管理

---

[查看早期版本手册](https://github.com/DASTUDIO/Zarch/blob/master/Legacy/1.0.4/README.md)

---


