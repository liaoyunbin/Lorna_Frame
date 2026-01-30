namespace LornaGame.UIExtensions
{
    using UnityEngine.UI;
    using System.Collections;
    //转为UTF8格式：
    ///
    public partial class BackgroundA2_Design : UI_Panel
    {
        //此脚本自动生成，禁止更改
        //==自动化变量开始
        public Button ButtonConfirm; 
        public Text TextA; 
 

#if UNITY_EDITOR
        public override void GeneratePath()
        {
            //==自动化路径开始
            ButtonConfirm = CacheTransform.Find("ButtonConfirm").GetComponent<Button>();
            TextA = CacheTransform.Find("TextA").GetComponent<Text>();
 

        }
#endif
    }
}
