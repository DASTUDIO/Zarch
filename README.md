# Zarch
C#的辅助语言,用于管理依赖。
* 1.自动创建并注入使用Attribute标记的类的对象。
* 2.用Zarch语言字符串控制被管理的对象的行为。
* 3.网络传输的Zarch语言字符串以实现RPC功能。

### Net Framework 使用

* 导入NuGet包 `点击 Project->Add NuGetPackages->搜索Zarch->勾选->Add Package`

* 使用Z命名空间 `using Z;`

### Unity3D 使用

* 导入[Unity Package](https://github.com/DASTUDIO/Zarch/raw/master/Zarch.unitypackage)到工程

* 参考 Package 里的 Demo


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
```

* 取委托符 []

```python
# x.play()
y = [x.play]
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

C# Code

```cs
Zarch.classes["T"] = typeof(Vector3);
```

Zarch Code

```python
x = T(float(0),float(0),folat(0));
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
