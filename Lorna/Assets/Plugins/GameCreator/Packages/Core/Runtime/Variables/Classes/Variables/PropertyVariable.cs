using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Serializable]
    public class PropertyVariable : TVariable
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        //[SerializeField] private IdString m_Name;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        //public string Name => this.m_Name.String;

        public override string Title => $"{this.m_Value}";

        public override TVariable Copy => new PropertyVariable
        {
           // m_Name = this.m_Name,
            m_Value = this.m_Value.Copy
        };

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PropertyVariable() : base()
        { }
        
        public PropertyVariable(IdString typeID) : base(typeID)
        { }

        public PropertyVariable(TValue value) : this()
        {
            //this.m_Name = new IdString(name);
            this.m_Value = value;
        }
    }
}