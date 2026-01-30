namespace FrameWork_UI
{
    using UnityEngine.UI;
    using System.Collections;
    //转为UTF8格式：
    ///
    public partial class Item_Design : UI_Base
    {
        //此脚本自动生成，禁止更改
        //==自动化变量开始
        public Image Bg; 
        public Image Icon; 
        public Text Name; 
 

#if UNITY_EDITOR
        public override void GeneratePath()
        {
            //==自动化路径开始
            Bg = CacheTransform.Find("Bg").GetComponent<Image>();
            Icon = CacheTransform.Find("Icon").GetComponent<Image>();
            Name = CacheTransform.Find("Name").GetComponent<Text>();
 

        }
#endif
    }
}
