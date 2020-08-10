
/* 
 * 使用这个脚本需要资源都放在Audio文件夹中
 */
using CQCFrameWork.AssetBundlesMgr;
using UnityEngine;
using UnityEngine.Events;
namespace CQCFrameWork.AudioMgr
{

    public class AudioMgr : Singleton<AudioMgr>
    {
        private AudioSource BGMMusic = null;
        private AudioSource soundEff = null;
        public AudioMgr()
        {
            if (BGMMusic == null)
            {
                GameObject obj = new GameObject("BGMMusic");
                BGMMusic = obj.AddComponent<AudioSource>();
                GameObject.DontDestroyOnLoad(obj);
            }
        }
        #region ===========================================================BGM============================================================

        /// <summary>
        /// 打开BGM
        /// </summary>
        /// <param name="name"></param>
        public void PlayBGM(PrefabName name, string path = null, bool loop = true)
        {
            AssetBundleMgr.Instance.ABLoadAsync<AudioClip>(name, path, clip =>
            {
                BGMMusic.clip = clip;
                BGMMusic.volume = 1;
                BGMMusic.loop = loop;
                BGMMusic.priority = 0;
                BGMMusic.pitch = 1;
                BGMMusic.Play();
                //这里用Play()而不用PlayOneShot的原因是，用Play的话点击一次都是重头开始
                //而PlayOneShot每次点击，都要让声音播放完才停止，未播放完上一个音乐再次点击的话，第二段音乐会与第一段音乐叠加，听起来有两个声音同时存在
            });
        }
        /// <summary>
        /// 关闭BGM
        /// </summary>
        public void StopBGM()
        {
            if (BGMMusic == null)
            {
                return;
            }
            BGMMusic.Stop();
        }
        /// <summary>
        /// 暂停BGM
        /// </summary>
        public void PauseBGM()
        {
            if (BGMMusic == null)
            {
                return;
            }
            BGMMusic.Pause();
        }
        /// <summary>
        /// 改变背景音乐音量大小
        /// </summary>
        /// <param name="v"></param>
        public void ChangeBGMVolume(float v)
        {
            if (BGMMusic == null)
            {
                return;
            }
            BGMMusic.volume = v;
        }
        #endregion ===============================================BGM=========================================================================


        #region ===============================================音效=========================================================================
        /// <summary>
        /// 播放音效
        /// </summary> 
        /// <param name="name"></param>
        static AudioClip lastClip = null;
        static PrefabName lastname = PrefabName.NULL;
        public void PlaySound(PrefabName name, string path = null, bool loop = false, UnityAction<AudioSource> callback = null)
        {
            if (soundEff == null)
            {
                GameObject obj = new GameObject("SoundEff");
                obj.transform.SetParent(BGMMusic.transform);
                soundEff = obj.AddComponent<AudioSource>();
            }
            if (lastname == name)
            {
                soundEff.PlayOneShot(lastClip, 1);
                return;
            }
           
            AssetBundleMgr.Instance.ABLoadAsync<AudioClip>(name, path, clip =>
            {
                lastname = name;
                soundEff.loop = loop;
                soundEff.priority = 1;
                soundEff.pitch = 1;
                soundEff.PlayOneShot(clip, 1);
                lastClip = clip;
            });
        }
        /// <summary>
        /// 关闭音效
        /// </summary>
        public void StopSound()
        {
            if (soundEff == null)
            {
                return;
            }
            soundEff.Stop();
        }
        /// <summary>
        /// 改变音效音量
        /// </summary>
        public void ChangeSoundEffVolume(float v)
        {
            if (soundEff == null)
            {
                return;
            }
            soundEff.volume = v;
        }
        #endregion ===============================================音效=========================================================================


    }
}
