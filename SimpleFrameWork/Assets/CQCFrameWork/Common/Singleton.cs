
using UnityEngine;
/// <summary>
/// C#单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : new()
{
    private static T _instance;
    static object _lock = new object();
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new T();
                }
            }
            return _instance;
        }
    }
}
/// <summary>
/// unity单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class UnitySingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    _instance = (T)obj.AddComponent(typeof(T));
                    obj.name = typeof(T).ToString();
                }
            }
            return _instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();
    }
    public virtual void Start()
    {
   
    }
    public virtual void Update()
    {

    }
    public virtual void Init()
    {
        // TODO:
    }
}
