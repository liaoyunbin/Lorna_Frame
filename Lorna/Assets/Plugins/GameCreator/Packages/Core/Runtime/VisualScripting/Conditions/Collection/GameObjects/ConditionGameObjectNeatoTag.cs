//using System;
//using GameCreator.Runtime.Common;
//using NeatoTags.Core;
//using UnityEngine;

//namespace GameCreator.Runtime.VisualScripting
//{
//    [Title("比较目标持有的NeatoTag")]
//    [Description("比较目标持有的NeatoTag")]

//    [Category("Custom自定义拓展/交互物/比较目标持有的NeatoTag")]
    
//    [Image(typeof(IconTag), ColorTheme.Type.Yellow)]
//    [Serializable]
//    public class ConditionGameObjectNeatoTag : Condition
//    {
//        // MEMBERS: -------------------------------------------------------------------------------
        
//        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
//        [SerializeField] private NeatoTag m_Tag;

//        // PROPERTIES: ----------------------------------------------------------------------------
        
//        protected override string Summary => $"如果目标并未持有{this.m_Tag}标签";
        
//        // RUN METHOD: ----------------------------------------------------------------------------

//        protected override bool Run(Args args)
//        {
//            GameObject gameObject = this.m_GameObject.Get(args);
            
//            if (gameObject == null)
//                return false;
            
//            // 如果小怪的受击盒上没有NeatoTag，先通过
//            if(gameObject.GetComponent<NeatoTag>() == null)
//                return true;
            
//            // 不包含Boss标签，也通过
//            return !gameObject.HasTag(gameObject.GetComponent<NeatoTag>());
//        }
//    }
//}
