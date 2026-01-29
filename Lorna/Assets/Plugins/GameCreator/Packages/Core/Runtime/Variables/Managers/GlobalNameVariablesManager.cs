using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [AddComponentMenu("")]
    public class GlobalNameVariablesManager : GameCreatorSingleton<GlobalNameVariablesManager>, IGameSave
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnSubsystemsInit()
        {
            Instance.WakeUp();
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] private Dictionary<IdString, NameVariableRuntime> Values { get; set; }

        [field: NonSerialized] private HashSet<IdString> SaveValues { get; set; }

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnCreate()
        {
            base.OnCreate();

            this.Values = new Dictionary<IdString, NameVariableRuntime>();
            this.SaveValues = new HashSet<IdString>();

            GlobalNameVariables[] nameVariables = VariablesRepository.Get.Variables.NameVariables;
            foreach (GlobalNameVariables entry in nameVariables)
            {
                if (entry == null) return;
                Instance.RequireInit(entry);
            }

            _ = SaveLoadManager.Subscribe(this);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------


        private Func<GameObject, string, NameVariable, object> OnLocalVariableGetAction;
        private Action<GameObject, string, NameVariable, object> OnLocalVariableSetAction;

        public void SetLocalVariableAction(
             Func<GameObject, string, NameVariable, object> _getAction,
             Action<GameObject, string, NameVariable, object> _setAction)
        {
            OnLocalVariableGetAction = _getAction;
            OnLocalVariableSetAction = _setAction;
        }

        // 监听对GC的LocalNameVariable进行Get的操作
        public object DoLocalVariableGetAction(GameObject _go, string _name, NameVariable _varibale)
        {
            if (OnLocalVariableGetAction == null)
            {
                return null;
            }
            return OnLocalVariableGetAction?.Invoke(_go, _name, _varibale);
        }
        
        // 监听对GC的LocalNameVariable进行Set的操作
        public void DoLocalVariableSetAction(GameObject _go, string _name, NameVariable _varibale, object _value)
        {
            OnLocalVariableSetAction?.Invoke(_go, _name, _varibale, _value);
        }

        //重载本地数据
        public void ReLoadLocalAsset()
        {
            this.Values.Clear();
            GlobalNameVariables[] nameVariables = VariablesRepository.Get.Variables.NameVariables;
            foreach (GlobalNameVariables entry in nameVariables)
            {
                if (entry == null) return;
                Instance.RequireInit(entry);
            }
        }

        public bool Exists(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(
                asset.UniqueID,
                out NameVariableRuntime runtime
            ) && runtime.Exists(name);
        }

        public void Remove(GlobalNameVariables asset, string name)
        {
            if (!this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)) return;

            runtime.Remove(name);
        }

        public GlobalNameVariables GetGlobalNameVariablesAsset(string assetName)
        {

            foreach (GlobalNameVariables entry in VariablesRepository.Get.Variables.NameVariables)
            {
                if (entry == null) return null;
                if (entry.name == assetName) return entry;
            }

            return null;
        }

        public object Get(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)
                ? runtime.Get(name)
                : null;
        }

        public void ClearAll(GlobalNameVariables asset)
        {
            if (this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime))
            {
                this.Values[asset.UniqueID].Clear();
            }
        }

        public NameVariable GetNameVariable(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)
                ? runtime.GetNameVariable(name)
                : null;
        }

        public Dictionary<string, NameVariable> GetAll(GlobalNameVariables asset)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime) ?
               runtime.Variables : null;
        }

        public string Title(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)
                ? runtime.Title(name)
                : string.Empty;
        }

        public Texture Icon(GlobalNameVariables asset, string name)
        {
            return this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)
                ? runtime.Icon(name)
                : null;
        }

        public void Set(GlobalNameVariables asset, string name, object value)
        {
            if (!this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime)) return;

            runtime.Set(name, value);
            if (asset.Save) this.SaveValues.Add(asset.UniqueID);
        }

        public void Add(GlobalNameVariables asset, string name, string typeID, object value)
        {
            if (this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime))
            {
                runtime.Add(name, typeID, value);
                if (asset.Save)
                    this.SaveValues.Add(asset.UniqueID);
            }

        }

        public void Register(GlobalNameVariables asset, Action<string> callback)
        {
            if (this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime))
            {
                runtime.EventChange += callback;
            }
        }

        public void Unregister(GlobalNameVariables asset, Action<string> callback)
        {
            if (this.Values.TryGetValue(asset.UniqueID, out NameVariableRuntime runtime))
            {
                runtime.EventChange -= callback;
            }
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RequireInit(GlobalNameVariables asset)
        {
            if (this.Values.ContainsKey(asset.UniqueID)) return;

            NameVariableRuntime runtime = new NameVariableRuntime(asset.NameList);
            runtime.OnStartup();

            this.Values[asset.UniqueID] = runtime;
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string SaveID => "global-name-variables";

        public LoadMode LoadMode => LoadMode.Greedy;
        public bool IsShared => false;

        public Type SaveType => typeof(SaveGroupNameVariables);

        public object GetSaveData(bool includeNonSavable)
        {
            Dictionary<string, NameVariableRuntime> saveValues = new Dictionary<string, NameVariableRuntime>();

            foreach (KeyValuePair<IdString, NameVariableRuntime> entry in this.Values)
            {
                if (includeNonSavable)
                {
                    saveValues[entry.Key.String] = entry.Value;
                    continue;
                }

                GlobalNameVariables asset = VariablesRepository.Get.Variables.GetNameVariablesAsset(entry.Key);
                if (asset == null || !asset.Save) continue;

                saveValues[entry.Key.String] = entry.Value;
            }

            SaveGroupNameVariables saveData = new SaveGroupNameVariables(saveValues);
            return saveData;
        }

        public Task OnLoad(object value)
        {
            if (value is not SaveGroupNameVariables saveData) return Task.FromResult(false);

            int numGroups = saveData.Count();
            for (int i = 0; i < numGroups; ++i)
            {
                IdString uniqueID = new IdString(saveData.GetID(i));
                List<NameVariable> candidates = saveData.GetData(i).Variables; //json里的值

                if (!this.Values.TryGetValue(uniqueID, out NameVariableRuntime variables))
                {
                    continue;
                }
                //原插件代码进行改动。直接将数据克隆过来
                //修改原因：原先代码如果出现json里name不存在，但是当前是存在的。会保留旧数据，会出现问题
                variables.Clear();
                foreach (NameVariable candidate in candidates)
                {
                    variables.Add(candidate.Name, candidate.TypeID.ToString(), candidate.Value);
                }
            }

            return Task.FromResult(true);
        }
    }
}