namespace LornaGame.UIExtensions
{
    using UnityEngine.UI;
    using System.Collections;
    //转为UTF8格式：
    ///
    public partial class BackgroundA1_Design : UI_Panel
    {
        //本脚本为自动生成，每次生成会覆盖！请勿手动修改
        //==自动化变量开始
        public Item_Design Item; 
        public Image Bg; 
        public Image Icon; 
        public Text Name; 
        public Button ButtonConfirm; 
        public Button ButtonConfirm_3_; 
        public Button ButtonConfirm_1_; 
        public Button ButtonConfirm_2_; 
 

#if UNITY_EDITOR
        public override void GeneratePath()
        {
            //==自动化路径开始
            Item = CacheTransform.Find("Item").GetComponent<Item_Design>();
            Bg = CacheTransform.Find("Item/Bg").GetComponent<Image>();
            Icon = CacheTransform.Find("Item/Icon").GetComponent<Image>();
            Name = CacheTransform.Find("Item/Name").GetComponent<Text>();
            ButtonConfirm = CacheTransform.Find("ButtonConfirm").GetComponent<Button>();
            ButtonConfirm_3_ = CacheTransform.Find("ButtonConfirm_3_").GetComponent<Button>();
            ButtonConfirm_1_ = CacheTransform.Find("ButtonConfirm_1_").GetComponent<Button>();
            ButtonConfirm_2_ = CacheTransform.Find("ButtonConfirm_2_").GetComponent<Button>();
 

        }
#endif
    }
}
