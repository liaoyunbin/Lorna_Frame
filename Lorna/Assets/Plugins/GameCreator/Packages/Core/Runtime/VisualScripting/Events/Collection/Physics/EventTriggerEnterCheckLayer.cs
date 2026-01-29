using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("检测特定物理层级")]
    [Description("检测特定物理层级")]
    [Category("Custom自定义拓展/物理/检测特定物理层级并执行逻辑")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Blue)]

    [Serializable]
    public class EventTriggerEnterCheckLayer : Event
    {
        [SerializeField] private LayerMask m_LayerMask;
        
        // METHODS: -------------------------------------------------------------------------------
        
        protected internal override void OnAwake(Trigger trigger)
        {
            base.OnAwake(trigger);
            trigger.RequireRigidbody();
        }
        
        protected internal override void OnTriggerEnter3D(Trigger trigger, Collider collider)
        {
            base.OnTriggerEnter3D(trigger, collider);
            
            if (!this.IsActive) return;

            if ((m_LayerMask.value & (1 << collider.gameObject.layer)) != 0)
            {
                GetGameObjectLastTriggerEnter.Instance = collider.gameObject;
                _ = this.m_Trigger.Execute(collider.gameObject);
            }
        }
        
        protected internal override void OnTriggerEnter2D(Trigger trigger, Collider2D collider)
        {
            base.OnTriggerEnter2D(trigger, collider);
            
            if (!this.IsActive) return;

            if ((m_LayerMask.value & (1 << collider.gameObject.layer)) != 0)
            {
                GetGameObjectLastTriggerEnter.Instance = collider.gameObject;
                _ = this.m_Trigger.Execute(collider.gameObject);
            }
        }
    }
}