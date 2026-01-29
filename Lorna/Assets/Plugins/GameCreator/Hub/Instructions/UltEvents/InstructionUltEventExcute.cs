//using System;
//using System.Threading.Tasks;
//using System.Collections;
//using UnityEngine;
//using GameCreator.Runtime.Common;
//using GameCreator.Runtime.VisualScripting;
//using UltEvents;


//[Version(1, 0, 2)]

//[Title("UltEvent Excute")]
//[Description("执行UltEvent")]

//[Category("UltEvents/UltEvent Excute")]

//[Keywords("Event", "UltEvents", "Excute")]
//[Keywords("MonoBehaviour", "Behaviour", "Script")]

//[Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]

//[Serializable]
//public class InstructionUltEventExcute : Instruction
//{
//	// MEMBERS: -------------------------------------------------------------------------------
	
//	//[SerializeField] private PropertyGetString m_Tag = new PropertyGetString();
//	//[SerializeField] private PropertyGetBool m_Active = GetBoolValue.Create(true);

//	[SerializeField] public UltEvent Events;

//	// PROPERTIES: ----------------------------------------------------------------------------

//	public override string Title => this.Events.PersistentCallsList.Count>0? $"执行 {this.Events.PersistentCallsList.Count} 个 事件":"无执行事件";

//	// RUN METHOD: ----------------------------------------------------------------------------

//	protected override Task Run(Args args)
//	{

//		Events.InvokeSafe();
//		return DefaultResult;
//	}
//}