using System;
using UnityEngine;
namespace LornaGame.Runtime
{
    public abstract class BuffEffectBase
    {

        #region private param
        /// <summary>
        /// 当前持续时间
        /// </summary>
        private float duration;
        /// <summary>
        /// 持续时间为负数时为永久buff
        /// </summary>
        private bool IsPermanentBuff;
        /// <summary>
        /// 当前经过时间
        /// </summary>
        private float m_Time;
        /// <summary>
        /// 当前层数
        /// </summary>
        private int m_NowLayer;
        /// <summary>
        /// 最大层数
        /// </summary>
        private int m_MaxLayers;
        /// <summary>
        /// 当前buff是否结束
        /// </summary>
        private bool m_IsEnd;
        #endregion

        #region protected  function
        protected abstract void OnInit();
        /// <summary>
        /// buff进入
        /// </summary>
        protected abstract void OnEnter();
        /// <summary>
        /// buff退出
        /// </summary>
        protected abstract void OnEnd();
        /// <summary>
        /// buff效果更新
        /// </summary>
        protected abstract void OnUpdate();

        protected virtual void DoAddEffect(int _beforeNum, int _afterNum)
        {

        }
        protected virtual void DoReduceEffect(int _beforeNum, int _afterNum)
        {

        }
        #endregion


        #region public/ protected param

        public string Id;
        public BuffEffectType BuffType;
        /// <summary>
        /// 当前buff是否结束
        /// </summary>
        public bool IsEnd
        {
            get
            {
                return m_IsEnd;
            }set
            {
                m_IsEnd = value;
            }
        }

        public int nowLayer
        {
            get
            {
                return m_NowLayer;
            }
            set
            {
                m_NowLayer = value;
            }
        }
        public float timeRemain
        {
            get
            {
                return IsPermanentBuff?-1:(duration - m_Time);
            }
        }
        #endregion

        #region public function
        public void Init(string _bufferName, int _layerCount = 1)
        {
            duration = ThousandToOne(0);
            nowLayer = Mathf.Clamp(_layerCount,0, m_MaxLayers);
            IsEnd = false;
            m_Time = 0;
            OnInit();
            OnEnter();
        }


        public void Update(float _deltaTime)
        {
            if (IsEnd)
            {
                return;
            }
            OnUpdate();
            m_Time += _deltaTime;
            //持续时间为负数时为永久buff
            if (IsPermanentBuff)
            {
                return;
            }
            if (m_Time > duration)
            {
                IsEnd = true;
            }
        }
        public void UnInit()
        {
            OnEnd();
            m_Time = 0f;
            IsEnd = true;
        }

        /// <summary>
        /// 叠层规则：默认只有层数1，不做加层，每次加层当前时间计数归0
        /// </summary>
        public void AddLayerCount(int _layerCount)
        {
            if (IsEnd)
            {
                return;
            }
            //叠层处理时，重新进行计时
            m_Time = 0;
            int oldLayer = nowLayer;
            nowLayer = Math.Clamp(nowLayer + _layerCount, 0, m_MaxLayers);

            if (oldLayer != nowLayer)
            {
                DoAddEffect(oldLayer, nowLayer);
            }
        }

        /// <summary>
        ///减层规则：默认只有层数1，减层时buff移除
        /// </summary>
        /// <param name="_layerCount"></param>
        public void ReduceLayer(int _layerCount)
        {
            int oldLayer = nowLayer;
            nowLayer = Math.Clamp(nowLayer - _layerCount, 0, m_MaxLayers);
            if(oldLayer != nowLayer)
            {
                DoReduceEffect(oldLayer, nowLayer);
            }
            if (nowLayer <= 0)
            {
                IsEnd = true;
            }
        }

        #endregion

        #region private function

        private float ThousandToOne(float _number)
        {
            return _number * 0.001f;
        }
        #endregion
    }
}