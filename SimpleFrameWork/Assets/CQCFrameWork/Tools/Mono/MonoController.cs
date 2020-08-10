using UnityEngine;
using UnityEngine.Events;

public class MonoController : MonoBehaviour
{
    private event UnityAction updateEvent;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
        {
            updateEvent();
        }
    }
    public void AddUpdateListener(UnityAction func)
    {
        updateEvent += func;
    }
    public void RemoveUpdateListener(UnityAction func)
    {
        updateEvent -= func;
    }
}
