using ParadoxNotion.Design;
using NodeCanvas.Framework;
using NodeCanvas.DialogueTrees;
using ParadoxNotion;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
namespace NodeCanvas.Tasks.Actions
{

    [Category("对话组件")]
    [Description("You can use a variable inline with the text by using brackets likeso: [myVarName] or [Global/myVarName].\nThe bracket will be replaced with the variable value ToString")]
    [ParadoxNotion.Design.Icon("Dialogue")]
    public class SayImage : ActionTask<IDialogueActor>
    {

        public Statement statement = new Statement("对话图片");

        protected override string info {
            get { return string.Format("<i>' {0} '</i> Wait {1} sec.", ( statement.text.CapLength(30) ),waitTime); }
        }

        protected override void OnExecute() {
            statement.image = _Image;
            var tempStatement = statement.BlackboardReplace(blackboard);
            DialogueTree.RequestSubtitles(new SubtitlesRequestInfo(agent, tempStatement, EndAction));
        }
        public BBParameter<float> waitTime = 1f;

        public Sprite _Image;
        public CompactStatus finishStatus = CompactStatus.Success;

        // protected override string info {
        //     get { return string.Format("Wait {0} sec.", waitTime); }
        // }

        protected override void OnUpdate() {
            if ( elapsedTime >= waitTime.value ) {
                EndAction(finishStatus == CompactStatus.Success ? true : false);
            }
        }
    }
}