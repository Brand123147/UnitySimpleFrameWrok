
using System.Collections.Generic;
using UnityEngine;

public abstract class Base3DObject : MonoBehaviour
{
    Dictionary<string, List<Component>> controlDic = new Dictionary<string, List<Component>>();
    public virtual void OnAwake()
    {
        //需要的控件从这里添加查找，需要哪些手动添加哪些
        //在界面初始化的时候就先得到界面上所有需要用到的控件
        FindChildrenControl<BoxCollider>();
        FindChildrenControl<Rigidbody>();
        FindChildrenControl<CapsuleCollider>();
        FindChildrenControl<SphereCollider>();
    }
   
    void Start()
    {
        OnRegisterEvent();
    }
    void OnDestroy()
    {
        OnUnRegisterEvent();
    }
    /// <summary>
    /// 找到子对象的控件
    /// </summary>
    /// <typeparam name="T">输入控件类型名</typeparam>
    private void FindChildrenControl<T>() where T : Component
    {
        T[] controls = this.GetComponentsInChildren<T>();
        string objName = null;
        for (int i = 0; i < controls.Length; i++)
        {
            objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);
            }
            else
            {
                controlDic.Add(objName, new List<Component>() { controls[i] });
            }
        }
    }
    /// <summary>
    /// 外部调用获取UI控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="goName"></param>
    /// <returns></returns>
    protected T GetComponent<T>(string goName) where T : Component
    {
        if (controlDic.ContainsKey(goName))
        {
            for (int i = 0; i < controlDic[goName].Count; i++)
            {
                if (controlDic[goName][i] is T)
                {
                    return controlDic[goName][i] as T;
                }
            }
        }
        return null;
    }
    

    /// <summary>
    /// 当外界调用showPanel的时候会自动调用
    /// </summary>
    /// <param name="args">参数</param>
    public void Instantiate(params object[] args)
    {
        On3DInitialize(args);
    }
    //------------------------------------------------- 必须实现以下接口---------------------------------------------------------------
    /// <summary>
    /// 窗口初始化
    /// </summary>
    /// <param name="args">外界传进来的参数</param>
    protected abstract void On3DInitialize(params object[] args);


    //----------------------------------------------------- 虚函数接口---------------------------------------------------------------
    /// <summary>
    /// 窗口初始化时注册/监听事件
    /// </summary>
    protected virtual void OnRegisterEvent() { }
    /// <summary>
    /// 关闭窗口时移除事件
    /// </summary>
    protected virtual void OnUnRegisterEvent() { }
}
