using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Numbers")]
    [Description("Add two values together")]
    [Category("Math/Arithmetic/Add Numbers")]

    [Keywords("Sum", "Plus", "Float", "Integer", "Variable")]
    [Image(typeof(IconPlusCircle), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionArithmeticAddNumbers : TInstructionArithmetic
    {
        protected override string Operator => "+";
        
        protected override double Operate(double value1, double value2)
		{
#if PE_DEBUG
			//项目可能存在Integer或者GC内部名称的Number和已经GC公开参数Decimal之间的修改隐式转换的问题，所以这里主动抛出日志方便排查部分问题.
			UnityEngine.Debug.Log($"[InstructionArithmeticAddNumbers] 浮点数主动日志抛出查找对应隐式转换精度问题: value1 = {value1} ,value2 = {value2}");
#endif
            return value1 + value2;
        }
    }
}
