using System;
using UnityEngine;
using GameCreator.Runtime.Variables;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Inventory
{
    [Serializable]
    public class PropertyOverride
    {
        [SerializeField] private bool m_Override;
        [SerializeField] private PropertyVariable m_Number;

        [SerializeField] private IdString m_TypeID;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Override => this.m_Override;
        public PropertyVariable Number { get => this.m_Number; set { this.m_Number = value; } }

        public IdString TypeID
        {
            get => this.m_Number.TypeID;

            set
            {
                this.m_Number = new PropertyVariable(value);
            }
        }
    }
}