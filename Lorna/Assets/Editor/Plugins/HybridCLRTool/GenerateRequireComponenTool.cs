using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

//转为UTF8格式：
public class GenerateRequireComponenTool
{
    //first key:程序集名 
    private static HashSet<string> dependencyMap = new HashSet<string>();

    [MenuItem("Tools/GenerateRequireComponentData")]
    /// <summary>
    /// 生成对应c#脚本
    /// </summary>
    public static void GenerateRequireComponentData()
    {
        var list = ScanAllAssemblies();
        StringBuilder sb = new StringBuilder(1024);
        sb.Append("\n");
        foreach (var i in list)
        {
            sb.Append('"').Append(i).Append('"').Append(',').Append("\n");
        }
        GenerateRequireComponenCS(sb.ToString());
    }

    private static void GenerateRequireComponenCS(string _datas)
    {
        // 1. 定义脚本内容模板
        string className = "RequireComponentDatas";
        string fieldName = "stringList"; // 目标字段名
        string path = Application.dataPath + $"/HybridCLRGenerate/{className}.cs";
        if (File.Exists(path))
        {
            File.Delete(path);
            UnityEngine.Debug.Log($"已删除: {path}");
        }

        string scriptContent = $@"
        using System.Collections.Generic;
        using UnityEngine;

        public class {className}
        {{
            // 存储字符串数据的字段
            public static readonly List<string> {fieldName} = new List<string>(){{ {_datas}  }};
        }}";

        // 2. 保存脚本到项目
        File.WriteAllText(path, scriptContent);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh(); // 刷新资源数据库
    }

    /// <summary>
    /// 查看所有程序集中有RequireComponent组件添加的组件名
    /// </summary>
    /// <returns></returns>

    private static HashSet<string> ScanAllAssemblies()
    {
        dependencyMap.Clear();
        // 获取当前加载的所有程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            // 跳过Unity引擎和系统程序集
            if (assembly.FullName.StartsWith("UnityEngine") ||
                assembly.FullName.StartsWith("System") ||
                assembly.FullName.StartsWith("mscorlib"))
                continue;

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    // 检查RequireComponent特性
                    var attrs = type.GetCustomAttributes(typeof(RequireComponent), true);
                    if (attrs.Length > 0)
                    {
                        List<string> requiredTypes = new List<string>();
                        foreach (RequireComponent attr in attrs)
                        {
                            if (attr.m_Type0 != null) AddToMap(dependencyMap, attr.m_Type0);
                            if (attr.m_Type1 != null) AddToMap(dependencyMap, attr.m_Type1);
                            if (attr.m_Type2 != null) AddToMap(dependencyMap, attr.m_Type2);
                        }
                    }
                }
            }
        }
        return dependencyMap;
    }
    private static void AddToMap(HashSet<string> _map, Type _type)
    {
        if (!_type.IsSubclassOf(typeof(Component)))
        {
            return;
        }
        if (_type.IsAbstract)
        {
            return;
        }
        var _key = _type.FullName + "," + _type.Assembly.GetName().Name;
        if (!_map.Contains(_key))
        {
            _map.Add(_key);
        }
    }

    private static void AddToMap(HashSet<string> _map, string _asmName, string _className)
    {
        var _key = _className + ","+_asmName;
        if (!_map.Contains(_key))
        {
            _map.Add(_key); 
        }
    }
}
