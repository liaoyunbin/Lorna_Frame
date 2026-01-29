using System;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using Component = UnityEngine.Component;

namespace NodeCanvas.DialogueTrees
{

    //神TM的ButtonStatementNode
    [Obsolete("该节点已废弃,代码调用请更换为<StatementNode>,NodeCanvas调用请替换为<对话节点>")]
    [Name("[已废弃](该节点已废弃,NodeCanvas调用请替换为<对话节点>)底边或侧边对话节点")]
    [ParadoxNotion.Design.Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
    public class ButtonStatementNode : DTNode
    {
        [SerializeField]
        public Statement statement = new ("This is a dialogue text...456"){Type = StatementType.Bottom};
        
        public override bool  requireActorSelection { get { return true; } }
        
        protected override Status OnExecute(Component agent, IBlackboard bb) {
            var tempStatement = statement.BlackboardReplace(bb);

            //设置 底边对话发言
            DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(finalActor, tempStatement, OnStatementFinish));
            return Status.Success;
        }

        void OnStatementFinish() {
            status = Status.Success;
            DLGTree.Continue();
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR
        protected override void OnNodeGUI(){
            FixedLanKey();
            if (false == string.IsNullOrEmpty(statement.lanKey)){
                GUILayout.BeginVertical(Styles.centerLabel);
                GUILayout.Label("多语言Key : <b> " + statement.lanKey + "</b>");
                GUILayout.EndVertical();    
            }
            
            GUILayout.BeginVertical(Styles.roundedBox);
            GUILayout.Label("\"<i> " + statement.text.CapLength(30) + "</i> \"");
            GUILayout.EndVertical();
        }

        /// <summary>
        /// 属于后加需求
        /// statement.text不空则反向寻找lanKey
        /// statement.lanKey不空则查找lanVal填入
        /// </summary>
        private void FixedLanKey(){
            //statement.text不空则反向寻找lanKey
            // if (string.IsNullOrEmpty(statement.lanKey) && false == string.IsNullOrEmpty(statement.text)){
            //     statement.lanKey = NodeCanvas.Editor.NodeCanvasInternalWrapTool.GetLangKey(Editor.NodeCanvasInternalWrapTool.Language,statement.text);
            // }
            //
            // //statement.lanKey不空则查找lanVal填入
            // if (false == string.IsNullOrEmpty(statement.lanKey) && string.IsNullOrEmpty(statement.text)){
            //     statement.text = NodeCanvas.Editor.NodeCanvasInternalWrapTool.GetLangText(Editor.NodeCanvasInternalWrapTool.Language,statement.lanKey);   
            // }
            
            //statement.lanKey不空则查找lanVal填入
            if (false == string.IsNullOrEmpty(statement.lanKey)){
                statement.text = NodeCanvas.Editor.NodeCanvasInternalWrapTool.GetLangText(Editor.NodeCanvasInternalWrapTool.Language,statement.lanKey);   
            }
        }
#endif

    }
}