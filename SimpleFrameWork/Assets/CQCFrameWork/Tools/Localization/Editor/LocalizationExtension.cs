using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalizationExtension : MonoBehaviour
{
    [MenuItem("GameObject/UI/Localization_Text", false)]
    static public void AddText(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Text");
        LocalizationText txt = go.AddComponent<LocalizationText>();
        PlaceUIElementRoot(go, menuCommand);
    }

    // 以下是创建Text为Canvas的子物体，添加EventSystem，设置Canvas该有的组件的等
    private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
    {
        GameObject parent = menuCommand.context as GameObject;
        if (parent == null || parent.GetComponentInParent<Canvas>() == null)
        {
            parent = GetOrCreateCanvasGameObject();
        }

        string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
        element.name = uniqueName;
        Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
        Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
        GameObjectUtility.SetParentAndAlign(element, parent);


        Selection.activeGameObject = element;
    }
    // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
    static public GameObject GetOrCreateCanvasGameObject()
    {
        GameObject selectedGo = Selection.activeGameObject;

        // Try to find a gameobject that is the selected GO or one if its parents.
        Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in selection or its parents? Then use just any canvas..
        canvas = UnityEngine.Object.FindObjectOfType(typeof(Canvas)) as Canvas;
        if (canvas != null && canvas.gameObject.activeInHierarchy)
            return canvas.gameObject;

        // No canvas in the scene at all? Then create a new one.
        return CreateNewUI();
    }

    static public GameObject CreateNewUI()
    {
        // Root for the UI
        var root = new GameObject("Canvas");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
        CreateEventSystem(false, null);
        return root;
    }
    private static void CreateEventSystem(bool select, GameObject parent)
    {
        var esys = UnityEngine.Object.FindObjectOfType<EventSystem>();
        if (esys == null)
        {
            var eventSystem = new GameObject("EventSystem");
            GameObjectUtility.SetParentAndAlign(eventSystem, parent);
            esys = eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();

            Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
        }

        if (select && esys != null)
        {
            Selection.activeGameObject = esys.gameObject;
        }
    }
}


/// <summary>
/// 这里重写添加keystring属性必须要规范到serializedproperty keystring然后重写OnEnable，再在OnInspectorGUI里显示，
/// 如果直接像我参考的那个人的写法会出现keystring的值修改失败（在预制体预览界面才会出现此bug）
/// 用DelayedTextField是要按回车或者焦点移动才会完成，防止一直持续刷新增大开销
/// 处理SerializedProperty类型的字段需要用以下两句包裹起来，否则会出问题，
/// 详情参考UGUI源码：https://bitbucket.org/Unity-Technologies/ui/downloads/?tab=downloads
///  serializedObject.Update();
///  serializedObject.ApplyModifiedProperties();
/// </summary>
[CustomEditor(typeof(LocalizationText), true)]
class LocalizationTextEditor : UnityEditor.UI.TextEditor
{
    private SerializedProperty KeyString;
    protected override void OnEnable()
    {
        base.OnEnable();
        KeyString = serializedObject.FindProperty("KeyString");
    }
    public override void OnInspectorGUI()
    {
        // 测了好多次不知道为什么 鼠标每点一次 这里面的代码就运行4次
        // Debug.Log(1);  每次都打印4个1
        LocalizationText component = (LocalizationText)target;
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.ObjectField("Font Style", component.StyleFont(), typeof(UIFont), true);
        EditorGUILayout.DelayedTextField(KeyString);
        serializedObject.ApplyModifiedProperties();
        component.font = component.StyleFont().UseFont;
        string key = component.KeyString;
        if (!string.IsNullOrEmpty(key))
        {
            // 调用获取Localization.csv数据
            // 获取有几种语言
            // 判断当前的key有没有对应的value，有则显示Preview和赋值给Text文本框，没有则不显示Preview
            // 预览Preview框
            Localization.LoadDictionary();
            if (!Localization.dictionary.ContainsKey(key))
            {
                return;
            }
            List<string> languages = Localization.dictionary["KEY"];     // 获取有几种语言
            List<string> value = Localization.dictionary[key];
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 20;
            style.alignment = TextAnchor.LowerLeft;
            GUILayout.BeginHorizontal();
            GUILayout.Toggle(true, "Preview", "dragtab", GUILayout.MinWidth(20f));
            GUILayout.EndHorizontal();
            for (int i = 0; i < value.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(languages[i]);
                GUILayout.Space(15);
                if (GUILayout.Button(value[i], style))
                {
                    component.text = value[i];
                }
                GUILayout.EndHorizontal();
            }
        }
    }

}
