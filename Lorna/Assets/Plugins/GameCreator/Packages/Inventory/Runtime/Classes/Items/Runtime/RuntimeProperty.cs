using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Variables;

namespace GameCreator.Runtime.Inventory
{
    [Serializable]
    public class RuntimeProperty
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private IdString m_ID;

        [SerializeField] private PropertyVariable m_Number;
        [SerializeField] private string m_Text;

        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString ID => this.m_ID;

        public PropertyVariable Number
        {
            get => this.m_Number;
            set
            {
                this.m_Number = value;
                EventChange?.Invoke();
            }
        }

        public string Text
        {
            get => this.m_Text;
            set
            {
                this.m_Text = value;
                EventChange?.Invoke();
            }
        }

        public Sprite Icon { get; }
        public Color Color { get; }
        
        // EVENTS: --------------------------------------------------------------------------------

        internal event Action EventChange;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RuntimeProperty(RuntimeItem runtimeItem, Property property)
        {
            this.m_ID = property.ID;

            this.m_Number = property.Number;
            this.m_Text = property.Text(runtimeItem.Bag != null ? runtimeItem.Bag.Args : null);
            
            this.Icon = property.Icon;
            this.Color = property.Tint;
        }
        
        public RuntimeProperty(RuntimeItem runtimeItem, RuntimeProperty runtimeProperty)
        {
            this.m_ID = runtimeProperty.ID;
            this.m_Number = runtimeProperty.Number;
            this.m_Text = runtimeProperty.Text;
            
            this.Icon = runtimeProperty.Icon;
            this.Color = runtimeProperty.Color;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsVisible(RuntimeItem runtimeItem)
        {
            Property property = runtimeItem.Item.Properties.Get(this.m_ID, runtimeItem.Item);

            var Count = this.GetTotalNumber(runtimeItem);
            return property.Visible switch
            {
                Property.Visibility.AlwaysVisible => true,
                Property.Visibility.HideIfZero => Count > 0||Count<=-9999,
                Property.Visibility.Hidden => false,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        
        public double GetTotalNumber(RuntimeItem runtimeItem)
        {
            if(this.Number.TypeID.String!="number")
                return -9999;

            double value = (double)(this.Number.Value);

            foreach (KeyValuePair<IdString, RuntimeSocket> entrySocket in runtimeItem.Sockets)
            {
                if (!entrySocket.Value.HasAttachment) continue;
                
                RuntimeItem attachment = entrySocket.Value.Attachment;
                if (attachment.Properties.TryGetValue(this.m_ID, out RuntimeProperty property))
                {
                    value += property.GetTotalNumber(attachment);
                }
            }
                
            return value;
        }
        
        public bool Equivalent(RuntimeProperty other)
        {
            //Debug.Log("test + "+ other.ID.Hash);
            return this.Number.Value.Equals(other.Number.Value) && this.ID.Hash == other.ID.Hash;
        }
        
        public bool Equivalent(Property other)
        {
            return this.Number.Value.Equals(other.Number.Value) && this.ID.Hash == other.ID.Hash;
        }
    }
}