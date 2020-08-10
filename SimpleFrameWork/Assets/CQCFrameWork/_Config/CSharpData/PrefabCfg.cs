/* 模版用于匹配导出的CSharp脚本工具 */
using LitJson;
using System.IO;
using System.Collections.Generic;

namespace CQCFrameWork.Config
{
    public sealed class PrefabCfgItem
    {
        /// <summary> 
 		/// id
 		/// </summary> 
 		public int id;
		/// <summary> 
 		/// Assetbundle名称
 		/// </summary> 
 		public string AssetBundleName;
		/// <summary> 
 		/// 预制体名字
 		/// </summary> 
 		public string AssetName;
    }

    public class PrefabCfgCtrl
    {
        Dictionary<int, PrefabCfgItem> mConfigDic;
        protected PrefabCfgCtrl()
        {
            mConfigDic = new Dictionary<int, PrefabCfgItem>();
			if (mConfigDic.Count == 0)
            {
                string mPathJson = @"Assets\CQCFrameWork\_Config\JsonData\";
                using (StreamReader sr = new StreamReader(mPathJson + GetAssetName()))
                {
                    LoadConfig(sr.ReadToEnd());
                }

            }
        }

        class SingletonCreator
        {
            static SingletonCreator() { }
            internal static readonly PrefabCfgCtrl instance = new PrefabCfgCtrl();
        }

        public static PrefabCfgCtrl Instance
        {
            get { return SingletonCreator.instance; }
        }
		
		public static PrefabCfgItem TryGet(int id)
        {
            return Instance.Get(id);
        }

        public string GetAssetName()
        {
        	return "Prefab.json";
        }

        public Dictionary<int, PrefabCfgItem> All
        {
            get { return mConfigDic; }
        }

        public void LoadConfig(string content)
        {
            mConfigDic.Clear();
            JsonData data = JsonMapper.ToObject(content);
            for (int i = 0; i < data.Count; ++i)
            {
                var obj = JsonMapper.ToObject<PrefabCfgItem>(data[i].ToJson());
                if (obj != null)
                    mConfigDic.Add(obj.id, obj);
            }
        }

        public PrefabCfgItem Get(int id)
        {
            if (mConfigDic.ContainsKey(id))
                return mConfigDic[id];
            return null;
        }
    }
}
