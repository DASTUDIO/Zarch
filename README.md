# Zarch
C#上用于管理依赖的辅助语言

### Net Framework 使用

* 导入NuGet包 `点击 Project->Add NuGetPackages->搜索Zarch->勾选->Add Package`

* 使用Z命名空间 `using Z;`

### Unity3D 使用

* 拖拽Zarch.dll文件到Unity3D的Project视图里(文件窗口) [下载Zarch.dll](https://github.com/DASTUDIO/Zarch/raw/master/Zarch.dll)

* 使用Z命名空间 `using Z;`


## 概览

* 示例：标准实体类

```

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


* 示例：多层依赖

```

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


* 示例：多层依赖的手动注入 ( 延迟构造 )

```

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


## Get Started


### Zarch in C Sharp

* 在一切之前 需要使用命名空间 Z

```
using Z;
```

* 执行 Zarch 代码

```
Zarch.code = Zarch代码
```

* 自动注入标记

```
[ZarchBean]
```

* 带有多层依赖的自动注入标记

```
[ZarchBean(依赖的对象的Zarch对象名)]
```

* 手动注入对象

```
Zarch.objects[Zarch对象名] = CSharpObject对象
```
* 手动注入方法

```
Zarch.methods[Zarch方法名] = CSharpDelegate委托
```
* 手动调用一个Zarch中的方法

```
Zarch.call(Zarch方法名,参数1，参数2)
```

* 手动唤起自动注入 

用于当未使用Zarch.code时访问自动注入的对象

```
Zarch.init()
```

* 手动刷新自动注入

用于当自动注入内容的多层依赖达到三层以上

```
Zarch.refresh()
```

* 将Zarch代码中的方法提取成为C#委托 

委托类型例如typeof(Action)

```
Zarch.CreateDelegate(ZarchCode方法名,C#委托类型)
```
### Zarch Code  

* 分句符 ;

```
code;
```
* 赋值 =

```
y = x ;
```


* 调用成员 .

```
user.name ;
```

* 调用函数 [函数名(传入参数)]

```
say() ;
```

* 注释标记 #

```
# 这是一条注释
```

### 实用示例

```
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
