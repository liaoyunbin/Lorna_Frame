//using System;
//using System.Threading.Tasks;
//using System.Collections;
//using UnityEngine;
//using GameCreator.Runtime.Common;
//using GameCreator.Runtime.VisualScripting;
//using UltEvents;


//[Version(1, 0, 2)]

//[Title("Set Active with Tag")]
//[Description("Changes the state of all game objects with the specified tag to active or inactive")]

//[Category("Game Objects/Set Active with Tag")]

//[Keywords("Activate", "Deactivate", "Enable", "Disable", "Tag")]
//[Keywords("MonoBehaviour", "Behaviour", "Script")]

//[Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]

//[Serializable]
//public class InstructionGameObjectSetActiveWithTag : Instruction
//{
//	// MEMBERS: -------------------------------------------------------------------------------
	
//	[SerializeField] private PropertyGetString m_Tag = new PropertyGetString();
//	[SerializeField] private PropertyGetBool m_Active = GetBoolValue.Create(true);

//	//[SerializeField] public UltEvent Events;

//	// PROPERTIES: ----------------------------------------------------------------------------

//	public override string Title => $"Set Active with Tag {this.m_Tag} to {this.m_Active}";

//	// RUN METHOD: ----------------------------------------------------------------------------

//	protected override Task Run(Args args)
//	{

//		string tag = this.m_Tag.Get(args);

//		GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag); 

//		if (gameObjects.Length < 1)
//		{
//			// Debug.Log("Instruction Set Active With Tag '"+this.m_Tag.Get(args)+"' found no matching game objects");
//			return DefaultResult;
//		}

//		foreach (GameObject go in gameObjects)
//		{
//			go.SetActive( this.m_Active.Get(args) );   
//		}

//		return DefaultResult;
//	}
//}