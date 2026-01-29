using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Find by UnityTag")]
    [Category("Game Objects/Find by UnityTag")]
    
    [Image(typeof(IconTag), ColorTheme.Type.Yellow)]
    [Description("Searches the scene for a Game Object with a specific tag")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectByUnityTag : PropertyTypeGetGameObject
    {
        [SerializeField] protected PropertyGetString m_Tag = new PropertyGetString("");

        public override GameObject Get(Args args)
        {
            string tag = this.m_Tag.Get(args);
            return GameObject.FindWithTag(tag);
        }

        public override string String => this.m_Tag.ToString();

        public override GameObject SceneReference => GameObject.FindWithTag(this.m_Tag.ToString());
    }
}