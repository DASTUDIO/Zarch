# Zarch语言，简而不单。

### 运行zarch代码
```csharp
Zarch.code = "Debug.Log('hello world')";                 // c# code
```

把ZarchConnector.prefab拖入场景，然后 `using Z;` ，之后赋值即运行。 

## Get Started：

#### * 移动物体
```js
$(myCube).move(1,2,3)
```

#### * 旋转物体
```js
$(myCube).rotate(10,20,30)
```

#### * 操作父物体

```js
$($(child).parent).move(1,2,3)
```

#### * 操作子物体

```js
$($(parent).children.get(0)).move(1,2,3)
```

#### * 显示/隐藏 物体

```js
$(myCube).active(bool(0))
```

#### * 获取组件

```js
$(myCube).get('Transform').Translate(1,2,3)
```

#### * 添加组件

```js
$(myCube).add('Rigidbody').AddForce(Vector3.up)
```

#### * 绑定点击事件
UI
```js
$(mybtn).click({ print('123') })
```
模型
```js
$(myCube).click({ Debug.Log(123) })
```

#### * 获取材质
```js
$(myCube).mat()
```

#### * 设置材质
```js
$(myCube).mat($(mc).mat())
```

#### * 获取主贴图
```js
$(myCube).tex()
```

#### * 设置主贴图
```js
$(mc).tex($(myCube).tex())
```

#### * 获取shader参数
```js
$(myCube).attr()
```

#### * 设置shader参数
```js
$(myCube).attr('_Glossiness',100)
```

#### * 获取 UI Image 图片内容
```js
$(myImage).image()
```

#### * 设置 UI Image 图片内容
```js
$(target).image($(origin).image())
```

#### * 获取 UI Text 文本内容
```js
$(myText).text()
```

#### * 设置 UI Text 文本内容
```js
$(target).text($(origin).text())
```

#### * 获取 UI Slider 滑动位置
```js
$(mySlider).slider()
```

#### * 设置 UI Slider 滑动位置
```js
$(target).slider($(origin).slider())
```

#### * 获取 tag
```js
$(myCube).tag()
```

#### * 设置 tag
```js
$(myCube).tag('myTag')
```

#### * 获取层
```js
$(myCube).layer()
```

#### * 设置层
```js
$(myCube).layer('myLayer')
```

#### * 销毁组件
```js
$(myCube).del('MeshRenderer')
```

#### * 激活/关闭 组件
```js
$(mybtn).active('Image',b)     # b是之前定义的布尔值 b = bool(0)
```

#### * 协程任务

```js
$.coroutine({ $(myCube).move(1,2,3) }, 0.5, 5)
```

（每0.5秒执行一次移动，一共执行5次）

格式：`$.coroutine([方法委托],[触发间隔],[循环次数])`

当循环次数小于0时 无限循环

当循环次数等于0时 等待\[触发间隔\]秒后，再执行\[方法委托\]

如果你的Unity版本不支持多次触发同一个协程，还有$.coroutine1().$.cotourine2(),$.coroutine3()可以使用。

#### * 线程任务


```js
$.thread( { Thread.Sleep(3000); }, { $(myCube).move(1,2,3) })
```

(在线程任务sleep()完成后回到主线程移动游戏物体myCube)

格式：`$.thread([新线程执行的方法],[完成后回到主线程执行的回调])`


#### * 网络任务


```js
print($.get('http://baidu.com'))
```

格式：`$.get([请求地址])`
直接返回字符串

该方法为阻塞方法，可以配合线程使用
```js
url = 'http://baidu.com/';
t = { res = $.get(url) };
$.thread( t, { print(res) } )
```

#### * Unity内置功能

对象
```js
print(Time.time);
```
方法
```js
Debug.Log('hello world');
```

##### * 基本输出

```js
print('hello world')
```

##### * 帮助
```js
help()
```
##### * 任务功能帮助
```js
$.help()
```
##### * 查看全部

> 可用对象
```js
objects()
```
> 自定义方法
```js
methods()
```
> 类（功能）
```js
classes()
```

##### * 清屏
```js
clear()
```

#### * 方法委托

无参数
```python
# 定义
d = { print('hello') };
# 执行
d();
```
有参数
```python
# 定义
d2 = [Debug.Log];
# 执行
d2('Hi');

```

#### 稳定性
1.如果你用到代码块{}方法委托，建议代码块大括号里不要这样{'someStr'}定义字符串
而是**把字符串定义在外面**，这样会得到更稳定的代码和更小的资源占用。
```js
url = 'http://baidu.com/';
t = { res = $.get(url) };
$.thread( t, { print(res) } )
```

2.如果使用布尔值 不要直接使用bool() 而是**把布尔值定义在外面**保存成对象使用
```js
n = bool(0)
```
然后使用对象
```js
$(mybtn).active('Image',n)
```

#### 参数传递
在zarch中最常见的自定义委托是代码块 {...}
```js
x = { print(1); }
```
这种情况下，传递运行结果到外部用的是对象。
```js
a=1;
x = { a = int(a) + 1; };
for(0, 1, 10, x);
```


## 在C#中直接使用

或者 你也可以在C#中通过`Zarch.csharp.S`直接调用这些方法

例：

zarch code:
```js
$(myCube).move(1,2,3)
```
c# code
```csharp
Zarch.csharp.S(myCube).move(1,2,3)
```

## 进阶

#### 数据类型
内置以下类型的转换 ：int(),float(),double(),bool(),str(),toUnityObj()
```js
a = int(b);
```
字符串 一律使用单引号
```js
mystr = 'hello';
```
列表
```js
mylist = list(1,2,3,4,5)
```
布尔值
```python
# bool() 返回 false
# bool(int) 大于0 true 小于等于0 false
# bool(obj) 不为空时返回true 为空返回false
# bool(a,b) 相同 true 不同 false , a、b 可以是string、int、float、double 
```
null
```js
false = bool(null())
```

#### 流程控制

通过函数实现流程控制

if(bool, trueDelegate, falseDelegate, params1, params2...)
```js
a = {...};
b = {...};
if(bool(...), a, b);
# 如果有参数 可以加在后面(...a,b,param1,param2...)

```

for(start, step, end, delegate, param1, param2...)
```js
c = {...}
for(1,1,100,c);
# 如果有参数 可以加在后面(...100,c,param1,param2...)
```
详情见下方早期版本手册


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

从2.0.1开始，使用Unity中的自建脚本不再需要注入

~~如果是你自己写的类，你也可以通过在类声明上方加[ZarchClass]来实现注入~~
```csharp
[ZarchClass]
public class MyClass : MonoBehaviour {
        void Start(){}
        void Update(){}
}

```

#### 配置反射程序集 
```csharp
Zarch.ReflectConfig.Assembly = ZarchReflectHelper.AssemblyType.Executing;
```
详情见下方早期版本手册


#### 复杂工程
如果你的工程规模很大，很复杂，那我建议你不要把各种引用拖来拖去，也不要频繁地Find物体,

而是**使用统一的依赖管理**功能。

```csharp
// 如果你使用1.0的实体类自动依赖注入功能 你需要查阅下方早期版本手册来 了解这些功能
Zarch.object[] 
Zarch.init();
Zarch.Refresh();
// 以及自动注入Attribute
[ZarchBean] [ZarchBean(params...)]

```

#### c# 调用 zarch code 内部的成员
```csharp
// 如果你需要在脚本外调用 你需要查阅下方早期版本手册来 了解这些功能
Zarch.call();
Zarch.CreateDelegate()
```

## 附录

#### 控制台

定位输出：将用以输出的Text组件拖入场景里 ZarchConnector物体中 ZarchUniry3DConnector组件 的console字段 中

#### 为Zarch增加扩展
你可以把以下代码写在任何地方
```csharp
public static class 类名随意 {
    public static void 你的方法名(this Zarch.Extension extension, 你的参数1，你的参数2...) { 要做的事; }
}
```
写好后 可以通过这样调用
```csharp
Zarch.csharp.你的方法名(你的参数1，你的参数2);
```


#### 开启新功能
如果发现需要的Unity功能未在zarch code用开启，
请自行前往 plugins/zarch/script/ZarchUnity3DConnector.cs中，仿照其他Unity功能的注册自行注册.

例:添加Slider
```js
Zarch.classes.Register<Slider>();
```


### 版本更新

2.0.1 增加在C#中使用魔法对象的扩展，增加$().click万能事件绑定,$.get()网络加载功能，$().tag/.layer/.mat/.tex/.attr/.image/.text/.slider,增加备用协程123，增加自写脚本免注入功能。

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

demo:1.0.4更新了一个unitypackage,它展示了在Unity3D中：

如何使用UGUI的InputField组件执行C#(Zarch)代码,并控制场景中的物体。

demo界面：
![demo界面](https://raw.githubusercontent.com/DASTUDIO/Zarch/master/img/idle.png)

控制光照：
![控制光照](https://raw.githubusercontent.com/DASTUDIO/Zarch/master/img/light.png)

获取类型：
![获取类型](https://raw.githubusercontent.com/DASTUDIO/Zarch/master/img/type.png)


1.0.4 修复在调用成员方法中遇见重载出现异常的Bug，新增了对类方法的支持,zarch code新增了type方法用以获取注入的class的Type(Unity中常用),C# 新增了tree查看注入的所有对象。

1.0.3.1 修复负数bug(仅源码) 

1.0.3 修复已知Bug，增加实例化对象功能，增加支持代码块、取委托，增加包含if和for在内的十个内置方法。

1.0.2 修复已知Bug，增加可赋值类型

1.0.1 增加简单自定义赋值功能

1.0.0 依赖管理

---
以下为早期版本手册（仍然兼容）

---
# Zarch

```
C#的辅助语言,早期是用于管理依赖，远程调用，热更新。
1.自动创建并注入使用Attribute标记的类的对象。
2.用Zarch语言字符串控制被管理的对象的行为。
3.网络传输的Zarch语言字符串以实现RPC功能。
```

### 使用 
1.0.4 后请直接使用源码，在工程中导入这四个cs文件：

Zarch.cs, ZarchReflectHelper.cs, ZarchBeanAttribute.cs, ZarchFunctions.cs

```
### Net Framework 使用

* 导入NuGet包 `点击 Project->Add NuGetPackages->搜索Zarch->勾选->Add Package`

* 使用Z命名空间 `using Z;`

非Visual Studio的用户，也可在[这里](https://github.com/DASTUDIO/Zarch/raw/master/ZarchLib.dll)下载DLL，在解决方案中编辑引用导入dll

### Unity3D 使用

* 导入[Unity Package](https://github.com/DASTUDIO/Zarch/raw/master/Zarch.unitypackage)到工程

* 参考 Package 里的 Demo
```


## Get Started

### Zarch Code  

* 分句符 ;

```python
code ;
```
* 赋值 =

```python
y = x ;
```


* 调用成员 .

```python
user.name ;
```

* 调用函数 [函数名(传入参数)]

```python
say() ;
```

* 注释标记 #

```python
# 这是一条注释
```

* 字符串(String) ''

```python
x = '你好这是一条字符串';
```

* 整数(Int) 

```python
x = 100 ;
```

* 双精度小数(Double)

```python
x = 0.75 ;
```

* 浮点数

```python
x = float(0.75) ;
```

* 布尔型

```python
x = bool(1) ;
```

* 列表 

```python
x = list(1,2,3,4,5) ;
```

* 字典

```python
x.name = '小明' ; x.age = 15 ;
```

* 委托代码块 {}

```python
x = {print('hello');print(',world');} ;
x();
```

* 取委托符 []

```python
# x.play();
y = [x.play];
y();
```

* 流程控制

```python
if([条件（布尔值）],[条件为真执行的委托],[条件为假执行的委托])
```
示例

```python
yes = {print('a = b');} ; no = {print('a != b')};
if(bool('x','x'),yes,no)
```

* 循环语句

```python
for([起始],[步长],[终止],[被循环的委托])
```

示例

```python
for(0,1,100,[x.play])
```


* 面向对象


Zarch Code

```python
x = T(); x.play();
```

* 内置函数

```python
int(x);
str(x);
float(x);
double(x);
null();
bool(x);bool(a,b)
list(1,2,3,4,5);
for(start,step,end,delegate,parameters1,parameters2...)
if(condition,trueDelegate,falseDelegate,parameters1,parameters2)
print(a,b,c,d...)

```


### Zarch in C Sharp

* 在一切之前 需要使用命名空间 Z

```cs
using Z;
```

* 执行 Zarch 代码

```cs
Zarch.code = Zarch代码
```

* 自动注入标记

```cs
[ZarchBean]
```

* 带有多层依赖的自动注入标记

```cs
[ZarchBean(依赖的对象的Zarch对象名)]
```

* 注入一个类
```cs
Zarch.classes[ZarchCode类名]=typeof(C#类名)
````
在Zarch代码中实例化
```cs
Zarch.code = "x = ZarchCode类名();x.say();";
```

* 手动注入对象

```cs
Zarch.objects[Zarch对象名] = CSharpObject对象
```

* 手动获取对象

```cs
var obj = Zarch.objects[Zarch对象名];
```

* 手动注入方法

```cs
Zarch.methods[Zarch方法名] = CSharpDelegate委托
```

* 手动获取方法

```cs
Zarch.methods[Zarch方法名]
```

```cs
((Func<object[],object>)Zarch.methods[Zarch方法名])(parameters);
```

* 手动调用一个Zarch中的方法

```cs
Zarch.call(Zarch方法名,参数1，参数2)
```

* 手动唤起自动注入 

用于当未使用Zarch.code时访问自动注入的对象

```cs
Zarch.init()
```

* 手动刷新自动注入

用于当自动注入内容的多层依赖达到三层以上

```cs
Zarch.refresh()
```

* 将Zarch代码中的方法提取成为C#委托 

委托类型例如typeof(Action)

```cs
Zarch.CreateDelegate(ZarchCode方法名,C#委托类型)
```

* 配置反射的程序集 默认Entry

Uninty3D是Executing，

.NetFramework是Entry，

反射指定的程序集（比如加载的Dll）ByContainedClass+设置那个程序集中包含的类，

调取方的程序集是Calling



```cs
Zarch.ReflectConfig.Assembly = ReflectHelper.AssemblyType.Entry;
//Zarch.ReflectConfig.ContainedClass = typeof(SomeClass); //如果是ByContainedClass则需要设置
```

以下是全部类型

```cs   
public enum AssemblyType
{
        Entry ,
        Executing ,
        Calling ,
        ByContainedClass 
}
```


## 示例

* 标准实体类

```cs

[ZarchBean]  // <-
public class User { 
  public string name = "小明"; 
  public void Say()
  { Console.Write( "你好我是" + name ); } }

public class MainClass{
    public static void Main(string[] args)
    { Zarch.code = "User.Say()"; } }  // <-
        

```
输出 `你好我是小明`


* 多层依赖

```cs

[ZarchBean]  // <-
public class Engine 
{ public string name = "myEngine"; }

[ZarchBean("Engine")]  // <-
public class Car {
    Engine e;
        
    public void Info()
    { Console.WriteLine(e.name); } 
        
    public Car(Engine _e) { this.e = _e; } }
        
public class MainClass{
      public static void Main(string[] args)
      { Zarch.code = "Car.Info()"; } }  // <-
        

```

输出  `myEngine`


* 多层依赖的手动注入 

```cs

public class Engine 
{ public string name = "myEngine"; }

[ZarchBean("Engine")]  // <-
public class Car {
    Engine e;
        
    public void Info()
    { Console.WriteLine(e.name); } }
        
    public Car(Engine _e) { this.e = _e; }
        
public class MainClass{
    public static void Main(string[] args) { 
         Zarch.objects["Engine"] = new Engine();  // <-
         Zarch.code = "Car.Info()"; } }  // <-
        	

```
输出 `myEngine`


### 实用示例

```cs
using System;
using Z;
class MainClass{
    public static void Main(string[] args) {
        Zarch.methods["print"] = param => { print(param[0].ToString()); return null; };   // <-
        Zarch.methods["toStr"] = param => { return ToStringResult((bool)param[0]); };   // <-
        Zarch.code = "res = toStr(Connector.Connect());print(res);"; }  // <-

    public static string ToStringResult(bool res) {
        if (res) { return "Success"; }
        else { return "Error"; } }

    public static void print(string msg) {
        Console.WriteLine(msg);}
    }

[ZarchBean]  // <-
public class Config {
    public string host = "http://localhost:8080";
    public string name = "admin";
    public string password = "admin123"; } 

[ZarchBean("Config")]   // <-
public class Connector{
    Config config;
    public Connector(Config _config)
    { config = _config; }

    public bool Connect()
    { if (config.host != null) { return true; } return false; } }


```

