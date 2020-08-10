using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// �����������࣬������Ҫ�Զ����Localization.csv�м��ֶ�Ӧ
/// </summary>
public enum Languages
{
    Chinese = 1,
    English = 2,
}
public static class Localization
{
    /// <summary>
    /// ÿһ������
    /// </summary>
    static string line;

    /// <summary>
    /// �Ƿ���ع�����
    /// </summary>
    static bool isLoadFile = false;

    /// <summary>
    /// unity����api  ��ȡassets�ļ���·��
    /// </summary>
    static string path = Application.dataPath;

    /// <summary>
    /// ����.csv�е������ֵ�   ��Ҫ������ֵ����
    /// </summary>
    public static Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

    /// <summary>
    /// ������������
    /// </summary>
    public static int Language { get; set; }

    /// <summary>
    /// ����key����ȡ��ǰ�洢���������֣������һ��û�д���Ϊ��int��������Ĭ��Ϊ0������Chinese
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
        Debug.LogError("key:" + key + "  ��һ���Ƿ��ĸ�����û�з��룿����");
        return null;
    }

    /// <summary>
    /// �޸�����  ���洢�޸ĺ����������   �ַ��޸������¼�
    /// </summary>
    /// <param name="languages"></param>
    public static void ChangeLanguage(Languages languages)
    {
        if (Language == (int)languages)
        {
            return;   //��һ���жϱ��������Ҫ�޸ĵ����Ե��ڵ�ǰ�����򷵻�
        }
        Language = (int)languages;
        PlayerPrefs.SetInt("language", Language);
        LocalizationText.ChangeLanguage();
    }


    /// <summary>
    /// ���ض�ȡLocalization.csv�ļ�
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
            // ��ȡLocalization.csv�ļ���·��     �������޸Ķ�Ӧ·��
            using (StreamReader read = new StreamReader(path + @"\CQCFrameWork\Tools\Localization\Localization.txt", Encoding.UTF8))
            {
                while (!string.IsNullOrEmpty(line = read.ReadLine()))
                {
                    string[] content = line.Split(',');   //�����Ÿ���
                    dictionary.Add(content[0], GetLanguages(content));
                }
            }
            isLoadFile = true;
        }
    }


    /// <summary>
    /// �Ӽ��ص�ÿһ��������ѡ������
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
