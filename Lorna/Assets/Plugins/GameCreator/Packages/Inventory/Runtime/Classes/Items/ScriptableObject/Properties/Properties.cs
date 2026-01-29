using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.Inventory
{
    [Serializable]
    public class Properties : TItemList<Property>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private PropertiesOverrides m_Overrides = new PropertiesOverrides();

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Properties() : base()
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsInherited(IdString propertyID)
        {
            foreach (Property property in this.m_List)
            {
                if (property.ID.Hash == propertyID.Hash) return false;
            }

            return true;
        }

        public Property Get(IdString propertyID, Item item)
        {
            if (item == null) return null;
            foreach (Property property in item.Properties.m_List)
            {
                if (property.ID.Hash != propertyID.Hash) continue;
                return property;
            }

            return item.Parent != null
                ? item.Parent.Properties.Get(propertyID, item.Parent)
                : null;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static Dictionary<IdString, Property> FlattenHierarchy(Item item)
        {
            Dictionary<IdString, Property> map = new Dictionary<IdString, Property>();
            if (item == null) return map;

            foreach (Property listItem in item.Properties.m_List) map[listItem.ID] = listItem.Clone;

            if (item.Parent != null && item.Properties.InheritFromParent)
            {
                Dictionary<IdString, Property> parentList = FlattenHierarchy(item.Parent);

                foreach (KeyValuePair<IdString, Property> entry in parentList)
                {
                    if (!map.ContainsKey(entry.Key))
                    {
                        map[entry.Key] = entry.Value;

                        PropertiesOverrides overrides = item.Properties.m_Overrides;

                        if (overrides.ContainsKey(entry.Key))
                        {
                            //overrides[entry.Key].Number = entry.Value.Number;
                        }

                        if (overrides.TryGetValue(entry.Key, out PropertyOverride replacement))
                        {

                            if (replacement.Override) map[entry.Key].Number = (PropertyVariable)replacement.Number.Copy;
                            else
                            {
                                //Debug.Log(replacement.Number.Value+"  "+entry.Value.Number.Value);

                                //item.Properties.Get(entry.Key,item).Number = (PropertyVariable)map[entry.Key].Number;
                            }
                            //Debug.Log(item.ID+"  "+entry.Key+" "+replacement.Number.Value);
                        }


                    }
                }
            }

            return map;
        }

        // SERIALIZATION CALLBACK: ----------------------------------------------------------------

        internal void OnBeforeSerialize(Item item)
        {
            if (item.Parent == null || !item.Properties.InheritFromParent)
            {
                this.m_Overrides = new PropertiesOverrides();
                return;
            }

            PropertiesOverrides overrides = new PropertiesOverrides();

            Dictionary<IdString, Property> parentList = FlattenHierarchy(item.Parent);
            foreach (KeyValuePair<IdString, Property> entry in parentList)
            {
                overrides[entry.Key] = this.m_Overrides.ContainsKey(entry.Key)
                    ? this.m_Overrides[entry.Key]
                    : new PropertyOverride();
                if (this.InheritFromParent)
                {
                    if(overrides[entry.Key].Number==null)
                    {
                        overrides[entry.Key].Number = new();
                    }

                    if (overrides[entry.Key].Number.TypeID != entry.Value.Number.TypeID)
                    {
                        //Debug.LogError(item.name + "物品继承属性类型不对应，已修正");
                        overrides[entry.Key].Number = (PropertyVariable)entry.Value.Number.Copy;
                    }


                    //if (m_Overrides.ContainsKey(entry.Key)&&(!m_Overrides[entry.Key].Override) && m_Overrides[entry.Key].Number.Value != entry.Value.Number.Value)
                    //    overrides[entry.Key].Number = (PropertyVariable)entry.Value.Number.Copy;
                }
                //Debug.Log(entry.Key+" "+(this.m_Overrides.ContainsKey(entry.Key)?parentList[entry.Key].Number.Value:new PropertyOverride()));
            }

            this.m_Overrides = overrides;
        }
    }
}