using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;
using System.IO;

public class Person{
    public string name;
    public int age;
}

[CSharpCallLua]
interface Iperson{
    string name{get;set;}
    string age{get;set;}
    void Speak();
    void Talk(int a, int b);
    void Talk2(int a, int b);
}

public class HelloWorld1 : MonoBehaviour
{
    // 创建一个Lua环境，相当于一个lua虚拟机，建议全局唯一
    LuaEnv luaEnv;
    void Start()
    {
        luaEnv = new LuaEnv();

        //Debug_1();
        //Debug_2();
        //Debug_3();
        //Debug_4();
        Debug_5(); // 添加了一个新的Loader
        //CSharpCallLua_1(); // CSharp访问Lua
        LuaCallCSharp_1(); // Lua访问C#
    }

    private void Debug_1(){
        // 执行一段Lua语句，控制台显示会多一个Lua：
        luaEnv.DoString("print('hello world!!')");
    }

    private void Debug_2(){
        // 在lua中调用C#
        luaEnv.DoString("CS.UnityEngine.Debug.Log('hello world')");
    }

    private void Debug_3(){
        // TextAsset自动找到resources文件夹下的txt文件
        TextAsset ta = Resources.Load<TextAsset>("HelloWorld.lua");
        // 执行lua文件ta中的语句
        luaEnv.DoString(ta.text);
    }

    private void Debug_5(){
        //  自定义路径运行Lua脚本，增加自定义Loader
        luaEnv.AddLoader(MyLoader);
        //luaEnv.DoString("require 'test'");
    }

    private byte[] MyLoader(ref string filePath){
        string absPath = Application.dataPath + "/Test/" + filePath + ".lua.txt";
        return System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(absPath));
    }

    private void Debug_4(){
        luaEnv.DoString("require 'Helloworld'"); // 自动找到Resource文件夹下的Helloworld.lua.txt
    }

    private void CSharpCallLua_1(){
        luaEnv.DoString("require 'CSharpCallLua'"); // 执行完后lua脚本中声明的全局变量会被保存在lua虚拟机中
        int a = luaEnv.Global.Get<int>("a"); // 获得lua中的全局变量
        string b = luaEnv.Global.Get<string>("b");
        bool c = luaEnv.Global.Get<bool>("c");
        //print(a);
        //print(b);
        //print(c);
        // 将lua中的table映射到C#的类
        // 通过C#修改类的数据不会影响到lua
        Person p = luaEnv.Global.Get<Person>("d");// Xlua会自动将变量进行赋值
        //print(p.name);
        // 将Lua中的table映射到C#的接口中
        // 在C#中修改接口相关数据，会影响到lua
        Iperson iperson = luaEnv.Global.Get<Iperson>("d");
        //print(iperson.name);
        //iperson.Speak();
        //iperson.Talk(50, 30);
        //iperson.Talk2(50, 100);
        // 将Lua映射到字典中
        // 只能映射有key有value的成员变量
        Dictionary<string, object> dic = luaEnv.Global.Get<Dictionary<string, object>>("d");
        // foreach (var item in dic.Keys)
        // {
        //     print(item + "-" + dic[item]);
        // }
        // 将Lua映射到List中
        // 只能映射没有key的成员变量
        List<object> list = luaEnv.Global.Get<List<object>>("d");
        // foreach (var o in list)
        // {
        //     print(o);
        // }

        // 访问Lua中的全局变量函数---delegate
        Action act = luaEnv.Global.Get<Action>("add");
        // act();
        act = null;// 释放act在lua脚本中的引用，让lua环境可以正常释放
        // Action<int, int> act2 = luaEnv.Global.Get<Action<int, int>>("add2");
        // act2(66, 66);// 运行失败，原因是需要添加[CSharpCallLua]
        // act2 = null;

        Add add = luaEnv.Global.Get<Add>("add2");
        // string restA;
        // string restB;
        // string result = add(66, 66, out restA, out restB);
        // print(result);
        // print(restA);
        // print(restB);
        add = null;

        // 访问Lua中的全局变量函数---LuaFunction
        LuaFunction luaFunction = luaEnv.Global.Get<LuaFunction>("add2");
        // 调用luaFunction
        object[] objects = luaFunction.Call(1, 2);
        foreach (object o in objects)
        {
            print(o);
        }
    }

    private void LuaCallCSharp_1(){
        luaEnv.DoString("require 'LuaCallCSharp'");
    }

    [CSharpCallLua]
    public delegate string Add(int a, int b, out string c, out string d);// 后两个是用来接收多余的返回值的

    private void OnDestroy() {
        // 释放lua环境
        luaEnv.Dispose();
    }
}
