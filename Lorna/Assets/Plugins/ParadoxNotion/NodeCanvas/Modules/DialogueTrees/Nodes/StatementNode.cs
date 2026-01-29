using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.DialogueTrees
{

    [Name("对话节点")]
    [Description("Make the selected Dialogue Actor talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName] or [Global/myVarName]")]
    public class StatementNode : DTNode
    {
        [SerializeField]
        public Statement statement = new ("This is a dialogue text...456"){Type = StatementType.Spine};//后加参数,原本的做法不正确,容易给冗余配置只是一个类型区分，不需要一个节点。当前已经做了很多配置，暂时不删除
        
        public override bool requireActorSelection { get { return true; } }

        protected override Status OnExecute(Component agent, IBlackboard bb) {
            var tempStatement = statement.BlackboardReplace(bb);

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
        protected override void OnNodeGUI() {
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
            // //statement.text不空则反向寻找lanKey
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