using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Number")]
    [Description("Sets a value equal to another value")]

    [Category("Math/Arithmetic/Set Number")]

    [Parameter("Set", "Where the value is set")]
    [Parameter("From", "The value that is set")]

    [Keywords("Change", "Float", "Integer", "Variable")]
    [Image(typeof(IconArrowCircleDown), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticSetNumber : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetNumber m_Set = SetNumberGlobalName.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_From = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = {this.m_From}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            try
            {
				double value = this.m_From.Get(args);
#if PE_DEBUG
				//项目可能存在Integer或者GC内部名称的Number和已经GC公开参数Decimal之间的修改隐式转换的问题，所以这里主动抛出日志方便排查部分问题.
				UnityEngine.Debug.Log($"[InstructionArithmeticSetNumber] 浮点数主动日志抛出查找对应隐式转换精度问题: value = {value} ,m_Set = {m_Set}");
#endif
                this.m_Set.Set(value, args);
            }
            catch (Exception e)
			{
                // 因为所有的怪物其实都挂载了这个节点，所以这个try catch暂时还是要保留，仅剔除掉容错提示。
                // Debug.LogError("容错捕获提示！ --目前怪物身上未能正确获取AP数据，后续确定后批量刷掉报错节点！");
				Debug.LogError($"[InstructionArithmeticSetNumber] Exception 捕获:{e}");
            }

            return DefaultResult;
        }
    }
}
