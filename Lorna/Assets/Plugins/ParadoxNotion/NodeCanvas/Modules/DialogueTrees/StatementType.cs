using UnityEngine;

namespace NodeCanvas.DialogueTrees{
    public enum StatementType{
        None = 0, //空节点,不计数

        [InspectorName("立绘对话")]
        Spine = 1,

        [InspectorName("底边对话")]
        Bottom = 2,

        [InspectorName("侧边对话-左侧")]
        LeftAside = 3,

		[InspectorName("底边CG字幕")]
        BottomCG = 10,
    }
}
