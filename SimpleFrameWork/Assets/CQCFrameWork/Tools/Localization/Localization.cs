using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 定义语言种类，这里需要自定义和Localization.csv有几种对应
/// </summary>
public enum Languages
{
    Chinese = 1,
    English = 2,
}
public static class Localization
{
    /// <summary>
    /// 每一行数据
    /// </summary>
    static string line;

    /// <summary>
    /// 是否加载过数据
    /// </summary>
    static bool isLoadFile = false;

    /// <summary>
    /// unity内置api  获取assets文件夹路径
    /// </summary>
    static string path = Application.dataPath;

    /// <summary>
    /// 缓存.csv中的数据字典   主要用这个字典操作
    /// </summary>
    public static Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

    /// <summary>
    /// 语言设置属性
    /// </summary>
    public static int Language { get; set; }

    /// <summary>
    /// 输入key，获取当前存储的语言文字，如果上一次没有存因为是int类型所以默认为0，汉字Chinese
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    static public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return null;
        }
        LoadDictionary();
        List<string> languages = dictionary[key];
        int index = PlayerPrefs.GetInt("language") - 1;
        if (index < languages.Count)
        {
            if (index < 0)
            {
                index = 0;
            }
            return languages[index];
        }
        Debug.LogError("key:" + key + "  这一行是否哪个语言没有翻译？？？");
        return null;
    }

    /// <summary>
    /// 修改语言  并存储修改后的语言类型   分发修改语言事件
    /// </summary>
    /// <param name="languages"></param>
    public static void ChangeLanguage(Languages languages)
    {
        if (Language == (int)languages)
        {
            return;   //做一个判断保护，如果要修改的语言等于当前语言则返回
        }
        Language = (int)languages;
        PlayerPrefs.SetInt("language", Language);
        LocalizationText.ChangeLanguage();
    }


    /// <summary>
    /// 加载读取Localization.csv文件
    /// </summary>
    public static void LoadDictionary()
    {
        if (isLoadFile && Application.isPlaying)
        {
            return;
        }
        else
        {
            dictionary.Clear();
            // 读取Localization.csv文件的路径     可自行修改对应路径
            using (StreamReader read = new StreamReader(path + @"\CQCFrameWork\Tools\Localization\Localization.txt", Encoding.UTF8))
            {
                while (!string.IsNullOrEmpty(line = read.ReadLine()))
                {
                    string[] content = line.Split(',');   //按逗号隔开
                    dictionary.Add(content[0], GetLanguages(content));
                }
            }
            isLoadFile = true;
        }
    }


    /// <summary>
    /// 从加载的每一行数据中选出语言
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    static List<string> GetLanguages(string[] content)
    {
        List<string> strList = new List<string>();
        for (int i = 1; i < content.Length; i++)
        {
            if (!string.IsNullOrEmpty(content[i]))
            {
                strList.Add(content[i]);
            }
        }
        return strList;
    }
}
