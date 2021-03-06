# Zarch
**在编译后的环境中执行自定义C#代码。**

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
### 版本

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
