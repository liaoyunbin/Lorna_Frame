using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using System.Collections.Generic;

namespace GameCreator.Runtime.Variables
{
    [CreateAssetMenu(
        fileName = "Global Variables",
        menuName = "Game Creator/Variables/Name Variables"
    )]

    [Icon(RuntimePaths.GIZMOS + "GizmoGlobalNameVariables.png")]
    public class GlobalNameVariables : TGlobalVariables, INameVariable
    {
        // MEMBERS: -------------------------------------------------------------------------------
    
        [SerializeField] private NameList m_NameList = new NameList(
            new NameVariable("my-variable", new ValueNumber(5f))
        );

        // PROPERTIES: ----------------------------------------------------------------------------

        internal NameList NameList => this.m_NameList;

        public string[] Names => this.m_NameList.Names;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Exists(string name)
        {
            return GlobalNameVariablesManager.Instance.Exists(this, name);
        }
        public void Remove(string name)
        {
            GlobalNameVariablesManager.Instance.Remove(this, name);
        }

        public object Get(string name)
        {
            return GlobalNameVariablesManager.Instance.Get(this, name);
        }

        public void ClearAll()
        {
            GlobalNameVariablesManager.Instance.ClearAll(this);
        }

        public Dictionary<string, NameVariable> GetAll(){
            return  GlobalNameVariablesManager.Instance.GetAll(this);
        }
        public NameVariable GetNameVariable(string name){
            return GlobalNameVariablesManager.Instance.GetNameVariable(this,name);
        }
        public void Add(string name,string typeID, object value){
               GlobalNameVariablesManager.Instance.Add(this, name,typeID, value);
        }

        public void Set(string name, object value)
        {
            GlobalNameVariablesManager.Instance.Set(this, name, value);
        }

        public void Register(Action<string> callback)
        {
            if (ApplicationManager.IsExiting) return;
            GlobalNameVariablesManager.Instance.Register(this, callback);
        }
        
        public void Unregister(Action<string> callback)
        {
            if (ApplicationManager.IsExiting) return;
            GlobalNameVariablesManager.Instance.Unregister(this, callback);
        }
        
        // EDITOR METHODS: ------------------------------------------------------------------------
        
        public string Title(string name)
        {
            return GlobalNameVariablesManager.Instance.Title(this, name);
        }

        public Texture Icon(string name)
        {
            return GlobalNameVariablesManager.Instance.Icon(this, name);
        }
    }
}