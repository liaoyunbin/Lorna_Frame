using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [HelpURL("https://docs.gamecreator.io/gamecreator/variables/local-name-variables")]
    [Icon(RuntimePaths.GIZMOS + "GizmoLocalNameVariables.png")]
    
    [AddComponentMenu("Game Creator/Variables/Local Name Variables")]
    [DisallowMultipleComponent]
    
    [Serializable]
    public class LocalNameVariables : TLocalVariables, INameVariable
    {
        // MEMBERS: -------------------------------------------------------------------------------
    
        [SerializeField] private NameVariableRuntime m_Runtime = new NameVariableRuntime();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string[] KeyNameList => m_Runtime.KeyNameList;
        
        internal NameVariableRuntime Runtime => this.m_Runtime;
        
        // EVENTS: --------------------------------------------------------------------------------
        
        private event Action<string> EventChange;

        // INITIALIZERS: --------------------------------------------------------------------------
        
        
        protected override void Awake()
        {
            this.m_Runtime.OnStartup();
            this.m_Runtime.EventChange += this.OnRuntimeChange;
            
            base.Awake();
        }

        public static LocalNameVariables Create(GameObject target, NameVariableRuntime variables)
        {
            LocalNameVariables instance = target.Add<LocalNameVariables>();
            instance.m_Runtime = variables;
            instance.m_Runtime.OnStartup();
            
            instance.m_Runtime.EventChange += instance.OnRuntimeChange;
            return instance;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public bool Exists(string name)
        {
            return this.m_Runtime.Exists(name);
        }

        #region 编辑器扩展内容，其余人请勿使用
        public object GetInEditor(string name)
        {
            var list = this.m_Runtime.TemplateList;
            for (int i = 0; i < list.Length; ++i)
            {
                NameVariable variable = list.Get(i);
                if (variable == null) continue;
                if (!variable.Name.Equals(name)) continue;
                var val = variable.Copy as NameVariable;
                return val?.Value;
            }
            return null;
        }
        #endregion

        public object Get(string name)
        {
            //默认先从全局存储中拿取数据.拿不到的话直接走默认逻辑
            var nVar = this.m_Runtime.GetNameVariable(name);
            if (nVar != null)
            {
               var realData= GlobalNameVariablesManager.Instance.DoLocalVariableGetAction(this.gameObject, name, nVar);
                if(realData != null)
                {
                    return realData;
                }
            }
            return this.m_Runtime.Get(name);
        }


        public void Set(string name, object value)
        {
            //默认将数据存入全局容器中
            var nVar = this.m_Runtime.GetNameVariable(name);
            if (nVar != null)
            {
                GlobalNameVariablesManager.Instance.DoLocalVariableSetAction(this.gameObject, name, nVar, value);
            }
            this.m_Runtime.Set(name, value);
        }

        public void Add(string name,string typeID, object value)
        {
            this.m_Runtime.Add(name,typeID, value);
        }
        
        public void Register(Action<string> callback)
        {
            this.EventChange += callback;
        }
        
        public void Unregister(Action<string> callback)
        {
            this.EventChange -= callback;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private void OnRuntimeChange(string name)
        {
            this.EventChange?.Invoke(name);
        }

        // IGAMESAVE: -----------------------------------------------------------------------------

        public override Type SaveType => typeof(SaveSingleNameVariables);

        public override object GetSaveData(bool includeNonSavable)
        {
            return this.m_SaveUniqueID.SaveValue
                ? new SaveSingleNameVariables(this.m_Runtime)
                : null;   
        }

        public override Task OnLoad(object value)
        {
            SaveSingleNameVariables saveData = value as SaveSingleNameVariables;
            if (saveData != null && this.m_SaveUniqueID.SaveValue)
            {
                NameVariable[] candidates = saveData.Variables.ToArray();
                foreach (NameVariable candidate in candidates)
                {
                    if (!this.m_Runtime.Exists(candidate.Name)) continue;
                    this.m_Runtime.Set(candidate.Name, candidate.Value);
                }
            }
            
            return Task.FromResult(saveData != null || !this.m_SaveUniqueID.SaveValue);
        }
    }
}