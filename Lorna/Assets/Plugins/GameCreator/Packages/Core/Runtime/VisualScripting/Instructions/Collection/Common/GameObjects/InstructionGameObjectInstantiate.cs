//using System;
//using System.Threading.Tasks;
//using GameCreator.Runtime.Characters;
//using GameCreator.Runtime.Common;
//using Plugins.EscapeGameWrap.Effect;
//using UnityEngine;

//namespace GameCreator.Runtime.VisualScripting
//{
//    [Title("Instantiate")]
//    [Description("Creates a new instance of a referenced game object")]

//    [Category("Game Objects/Instantiate")]
    
//    [Parameter("Game Object", "Game Object reference that is instantiated")]
//    [Parameter("Position", "The position where the new game object is instantiated")]
//    [Parameter("Rotation", "The rotation that the new game object has")]
//    [Parameter("Save", "Optional value where the newly instantiated game object is stored")]
    
//    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayPlus))]
    
//    [Keywords("Create", "New", "Game Object")]
//    [Serializable]
//    public class InstructionGameObjectInstantiate : Instruction
//    {
//        // MEMBERS: -------------------------------------------------------------------------------

//#if UNITY_EDITOR
//        [SerializeField]
//        private PropertyGetInstantiate m_GameObject = new PropertyGetInstantiate();
//#endif     
//        [SerializeField]
//        public string PrefabAddress;

//        [SerializeField] 
//        private PropertyGetLocation m_Location = GetLocationCharacter.Create;

//        [SerializeField] 
//        private PropertyGetGameObject m_Parent = GetGameObjectNone.Create();

//        [SerializeField]
//        private PropertySetGameObject m_Save = SetGameObjectNone.Create;
        
//        // PROPERTIES: ----------------------------------------------------------------------------

//        // public override string Title => $"Instantiate {this.m_GameObject}";
//        public override string Title => $"Instantiate {this.PrefabAddress}";

//        // RUN METHOD: ----------------------------------------------------------------------------

//        protected override Task Run(Args args)
//		{
//			if (null == args || null == m_Location) { return DefaultResult; }//简单容错
//            Location location = this.m_Location.Get(args);

//            Vector3 position = location.HasPosition
//                ? location.GetPosition(args.Self)
//                : args.Self != null ? args.Self.transform.position : Vector3.zero;

//            Quaternion rotation = location.HasRotation
//                ? location.GetRotation(args.Self)
//                : args.Self != null ? args.Self.transform.rotation : Quaternion.identity;

//            // GameObject instance = this.m_GameObject.Get(args, position, rotation);
//            //2025/10/24 Effect池处理,旧逻辑插件内代码使用过多无法直接替换，故只能改动插件代码
//            if (string.IsNullOrEmpty(PrefabAddress))
//            {
//                return DefaultResult;
//            }
//            // GameObject instance = MMEffectManagerWrap.Spawn<GameObject>(PrefabAddress);
//            ParticleSystem par = MMEffectManagerWrap.Spawn<ParticleSystem>(PrefabAddress);
//            GameObject instance = par.gameObject;
//            instance.transform.position = position;
//            instance.transform.rotation = rotation;

//            if (instance != null)
//            {
//                Transform parent = this.m_Parent.Get<Transform>(args);
//                if (parent != null) instance.transform.SetParent(parent);
                
//                this.m_Save.Set(instance, args);
//            }

//            return DefaultResult;
//        }
//    }
//}
