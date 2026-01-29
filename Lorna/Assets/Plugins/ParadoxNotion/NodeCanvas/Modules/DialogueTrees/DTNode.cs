using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    ///<summary> Base class for DialogueTree nodes that can live within a DialogueTree Graph.</summary>
    abstract public class DTNode : Node
    {

        [SerializeField] private string _actorName = DialogueTree.INSTIGATOR_NAME;
        [SerializeField] private string _actorParameterID;

        public override string name {
            get
            {
                if ( requireActorSelection ) {
                    if ( DLGTree.definedActorParameterNames.Contains(actorName) ) {
                        //NodeCanvas旧逻辑
#if UNITY_EDITOR
                        string displayMainKey = actorLanKey;
                        string displaySubKey = string.Empty;
                        if (false == string.IsNullOrEmpty(actorLanKey)){
                            displayMainKey = actorLanKey;
                            displaySubKey = NodeCanvas.Editor.NodeCanvasInternalWrapTool.GetLangText(NodeCanvas.Editor.NodeCanvasInternalWrapTool.Language, displayMainKey);
                        }
                        else{
                            displayMainKey = actorName;
                            //2025/5/23 之前有用GO做变量，后边更新为用SO,所以针对NC来讲黑板值已经有了修改，这里需要刷新一次判定(并且刷新一次缓存)
                            displaySubKey = NodeCanvas.Editor.NodeCanvasInternalWrapTool.GetLangKey(NodeCanvas.Editor.NodeCanvasInternalWrapTool.Language, displayMainKey);
                            actorLanKey = displaySubKey;
                        }
                        
                        return string.Format("{0} : {1}", displayMainKey,displaySubKey);
#else
                        return string.Format("{0}", actorName);
#endif
                        //Refactor NodeCanvas逻辑 显示多语言key与value
                        
                    }
                    return string.Format("<color=#d63e3e>* {0} *</color>", _actorName);
                }
                return base.name;
            }
        }

        virtual public bool requireActorSelection { get { return true; } }
        public override int maxInConnections { get { return -1; } }
        public override int maxOutConnections { get { return 1; } }
        sealed public override System.Type outConnectionType { get { return typeof(DTConnection); } }
        sealed public override bool allowAsPrime { get { return true; } }
        sealed public override bool canSelfConnect { get { return false; } }
        sealed public override Alignment2x2 commentsAlignment { get { return Alignment2x2.Right; } }
        sealed public override Alignment2x2 iconAlignment { get { return Alignment2x2.Bottom; } }

        protected DialogueTree DLGTree {
            get { return (DialogueTree)graph; }
        }

        private string _localActorTempKey;
        [SerializeField]
        private string _actorLanKey;
        public string actorLanKey {
            get
            {
                var result = DLGTree.GetParameterByID(_actorParameterID);
                return result != null ? result.lanKey : _actorLanKey;
            }
            private set
            {
                if ( _actorLanKey != value && !string.IsNullOrEmpty(value) ) {
                    _actorLanKey = value;
                    var param = DLGTree.GetParameterByName(value);
                    _actorParameterID = param != null ? param.ID : null;
                }
            }
        }
        ///<summary>The key name actor parameter to be used for this node</summary>
        public string actorName {
            get
            {
                var result = DLGTree.GetParameterByID(_actorParameterID);
                return result != null ? result.name : _actorName;
            }
            private set
            {
                if (!string.IsNullOrEmpty(value) ) {
                    _actorName = value;
                    var param = DLGTree.GetParameterByName(value);
                    _actorParameterID = param != null ? param.ID : null;
                }
            }
        }

        ///<summary>The DialogueActor that will execute the node</summary>
        public IDialogueActor finalActor {
            get
            {
                var result = DLGTree.GetActorReferenceByID(_actorParameterID);
                var actor = result != null ? result : DLGTree.GetActorReferenceByName(_actorName);
                actor.nameLanKey = actorLanKey;
                return actor;
            }
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeInspectorGUI() {
            if ( requireActorSelection ) {
                GUI.backgroundColor = Colors.lightBlue;
                //新接入多语言管理后做法
                List<string> actorLanKey = new List<string>();
                DLGTree.definedActorParameterNames.ForEach(t => actorLanKey.Add($"{t} : {Editor.NodeCanvasInternalWrapTool.GetLangKey(Editor.NodeCanvasInternalWrapTool.Language, t)}"));
                _localActorTempKey = EditorUtils.Popup<string>(_localActorTempKey, actorLanKey);
                int index = actorLanKey.IndexOf(_localActorTempKey);
                if (index >= 0 && index < DLGTree.definedActorParameterNames.Count){
                    actorName = DLGTree.definedActorParameterNames[index];
                }

                //旧NodeCanvas正确做法
                // actorName = EditorUtils.Popup<string>(actorName, DLGTree.definedActorParameterNames);
                GUI.backgroundColor = Color.white;
            }
            base.OnNodeInspectorGUI();
        }

        protected override UnityEditor.GenericMenu OnContextMenu(UnityEditor.GenericMenu menu) {
            menu.AddItem(new GUIContent("Breakpoint"), isBreakpoint, () => { isBreakpoint = !isBreakpoint; });
            return menu;
        }

#endif
    }
}