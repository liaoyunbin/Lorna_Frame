namespace LornaGame.UIExtensions
{
    using LornaGame.ModuleExtensions;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    //转为UTF8格式：
    public partial class UIMgr
    {
        private bool m_InitCanvas = false;

        /// <summary>
        /// 设计分辨率
        /// </summary>
        private Vector2 referenceResolution = new Vector2(1920, 1080);
        private Dictionary<UILayer, GameObject> m_AllCanvas = new Dictionary<UILayer, GameObject>();
        private Dictionary<UILayer, CanvasScaler> m_AllCanvasScales = new Dictionary<UILayer, CanvasScaler>();


        private void InitCanvas()
        {
            if (m_InitCanvas)
            {
                return;
            }
            m_AllCanvasScales.Clear();
            m_AllCanvas.Clear();

            m_InitCanvas = true;
            var layerList = (UILayer[])Enum.GetValues(typeof(UILayer));
            GameObject root = new GameObject("UI_ROOT");
            root.transform.SetParentEx(null);
            root.layer = Layer.UI;
            GameObject.DontDestroyOnLoad(root);

            float matchValue = GetMatchValue();
            foreach (var layer in layerList)
            {
                GameObject canvasGo = new GameObject("Canvas_" + layer.ToString());
                RectTransform rectTransform = canvasGo.AddComponent<RectTransform>();
                rectTransform.SetParentEx(root.transform);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;

                canvasGo.layer = Layer.UI;
                Canvas canvas = canvasGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.overrideSorting = true;
                canvas.sortingOrder = (int)layer;
                canvas.worldCamera = WrapMgr.cameraManager.GetUICamera();
                canvas.pixelPerfect = false;

                CanvasScaler canvasScaler = canvasGo.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = referenceResolution;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = matchValue;
                canvasGo.AddComponent<GraphicRaycaster>();

                m_AllCanvas.Add(layer, canvasGo);
                m_AllCanvasScales.Add(layer, canvasScaler);
            }
        }

        private int lastWidth;
        private int lastHeight;
        public void DoUpdate(System.Object o)
        {
            // 检查分辨率变化
            if (lastWidth != Screen.width || lastHeight != Screen.height)
            {
                lastWidth = Screen.width;
                lastHeight = Screen.height;
                AdaptToScreenChanges();
            }
        }
        private void AdaptToScreenChanges()
        {
            float matchValue = GetMatchValue();
            foreach (var i in m_AllCanvasScales)
            {
                i.Value.matchWidthOrHeight = matchValue;
            }
        }
        private float GetMatchValue()
        {
            float currentAspect = (float)Screen.width / Screen.height;
            float targetAspect = referenceResolution.x / referenceResolution.y;
            // 根据宽高比调整匹配值
            //1f:宽屏，更适配高度
            //0f:宽屏，高屏，更适配宽度
            return (currentAspect > targetAspect)?1f: 0f;

        }

        public GameObject GetLayerParent(UILayer _layer)
        {
            InitCanvas();
            m_AllCanvas.TryGetValue(_layer, out GameObject go);
            return go;
        }

    }
}
