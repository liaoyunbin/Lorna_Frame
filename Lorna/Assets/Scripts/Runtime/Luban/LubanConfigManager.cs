using UnityEngine;

//转为UTF8格式：
using cfg;
using SimpleJSON;
using System.IO;
using Unity.VisualScripting;
namespace LornaGame.Runtime
{
    public class LubanConfigManager : LornaGame.ModuleExtensions.Singleton<LubanConfigManager>
    {
        private Tables m_tables;
        private System.Func<string, string, string> _currentTextMapper;
        private System.Action OnLanguageChanged;
        private Tables tables
        {
            get
            {
                if (m_tables == null)
                {
                    m_tables = new Tables(file => JSON.Parse(File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "LubanData", file + ".json"))));
                }
                return m_tables;
            }
        }

        #region public function

        public void Init()
        {
            // 传入一个用于加载单个JSON文件的方法
            //_tables = new Tables(LoadJsonFile);

            // 使用配置数据，例如根据ID获取道具
            var item = tables.Tbitem.Get(1001);
            Debug.Log($"道具名: {item.Name}, 描述: {item.Desc}");

            var icon = tables.Tbicon.Get(1001);
            Debug.Log($"道具名: {icon.Name}, 描述: {icon.Desc}");

        }
        public void LoadLanguage(string languageCode)
        {
            // 根据语言设置对应的文本映射器
            switch (languageCode)
            {
                case "zh":
                    _currentTextMapper = (key, defaultText) => tables.TbLanguage.Get(key).TextZh;
                    break;
                case "en":
                    _currentTextMapper = (key, defaultText) => tables.TbLanguage.Get(key).TextEn;
                    break;
            }
        }
        public string GetText(string key, string defaultText = "")
        {
            if (tables == null)
            {
                Debug.LogError("LubanI18nManager尚未初始化，请先调用LoadLanguage。");
                return defaultText;
            }

            var localizedText = tables.TbLanguage.Get(key);
            return _currentTextMapper?.Invoke(key, defaultText) ?? defaultText;
        }

        public void SwitchLanguage(string languageCode)
        {
            LoadLanguage(languageCode);
            OnLanguageChanged?.Invoke();
        }
        #endregion
    }
}
