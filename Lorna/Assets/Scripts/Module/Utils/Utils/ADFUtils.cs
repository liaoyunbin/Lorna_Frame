using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LornaGame.ModuleExtensions{
    public static class ADFUtils{
        /// <summary>
        /// C#获取一个类在其所在的程序集中的所有子类
        /// </summary>
        /// <param name="parentType">给定的类型</param>
        /// <returns>所有子类的名称</returns>
        public static List<Type> GetSubClass(Type parentType, bool containsAbstract = false){
            var subTypeList = new List<Type>();
            //获取当前父类所在的程序集``
            var assembly = parentType.Assembly;
            //获取该程序集中的所有类型
            var assemblyAllTypes = assembly.GetTypes();
            foreach (var itemType in assemblyAllTypes){
                if (itemType != parentType && itemType.IsSubclassOf(parentType)){
                    if (!containsAbstract && itemType.IsAbstract){
                        continue;
                    }

                    subTypeList.Add(itemType);
                }
            }

            return subTypeList;
        }

        /// <summary>
        ///  获取所有继承自指定接口的类型(别再高性能需求场景使用)
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="containsAbstract"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetImplementingTypes(Type interfaceType, bool containsAbstract = false){
            var subTypeList = new List<Type>();
            // 获取所有程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies){
                // 获取程序集中的所有类型
                Type[] types = assembly.GetTypes();
                foreach (Type type in types){
                    // 检查类型是否实现了目标接口
                    if (interfaceType.IsAssignableFrom(type) && !type.IsInterface){
                        if (!containsAbstract && type.IsAbstract){
                            continue;
                        }

                        subTypeList.Add(type);
                    }
                }
            }

            return subTypeList;
        }

        /// <summary>
        /// C#获取一个类在所有程序集中的所有子类
        /// </summary>
        /// <param name="parentType">给定的类型</param>
        /// <returns>所有子类的名称</returns>
        public static List<Type> GetAllSubClass(Type parentType, bool containsAbstract){
            var subTypeList = new List<Type>();

            // 获取所有加载的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies){
                var assemblyAllTypes = assembly.GetTypes();
                foreach (var itemType in assemblyAllTypes){
                    if (itemType != parentType && itemType.IsSubclassOf(parentType)){
                        if (!containsAbstract && itemType.IsAbstract){
                            continue;
                        }

                        subTypeList.Add(itemType);
                    }
                }
            }

            return subTypeList;
        }

        /// <summary>
        /// 获取所有有T特性的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Type> GetAllTypesByAttribute<T>() where T : Attribute{
            var res = new List<Type>();
            res = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttributes(typeof(T), false).Any()).ToList();
            return res;
        }

        /// <summary>
        /// 获取所有有T特性的方法
        /// 仅Static
        /// 消耗较大,建议仅Editor使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<MethodInfo> GetAllMethodByAttribute<T>() where T : Attribute{
            var res = new List<MethodInfo>();

            void OnCheck(Type t){
                var methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                var mts     = methods.Where(t => t.GetCustomAttributes(typeof(T)).Any());
                res.AddRange(mts);
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            assemblies.Ergodic(t => t.GetTypes().Ergodic(OnCheck));
            return res;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(Type model) where T : Attribute, new(){
            var res = new T();
            res = model.GetCustomAttribute<T>();
            return res;
        }
    }
}