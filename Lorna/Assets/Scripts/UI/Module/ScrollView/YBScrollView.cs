using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork_UI
{

    // 序列化字段未被代码赋值的警告，这里屏蔽掉
#pragma warning disable 649
    /// <summary>
    /// 刷新时的界面显示
    /// </summary>
    public delegate void OnItemRefresh(int index, YBScrollerItem rt);
    public delegate void OnRefreshItemWithComponentHash(int index, int componentIndex, YBScrollerItem rt);

    /// <summary>
    /// 根据index获取ItemSize的大小
    /// </summary>
    public delegate Vector2 ItemSizeProvider(int index);


    public class YBScrollView : MonoBehaviour {

        public enum E_ScrollMode {
            Vertical,
            Horizontal
        }

        public enum E_VerticalDirection {
            TopToBottom,
            BottomToTop
        }

        public enum E_HorizontalDirection {
            LeftToRight,
            RightToLeft
        }
        /// <summary>
        /// 预设体
        /// </summary>
        [SerializeField] private GameObject m_ItemPrefab;
        /// <summary>
        /// 默认大小
        /// </summary>
        [SerializeField] private Vector2 m_DefaultItemSize = new Vector2(100, 100);
        /// <summary>
        /// 横向/竖向滑动
        /// </summary>
        [SerializeField] private E_ScrollMode m_ScrollMode;

        [SerializeField] private E_VerticalDirection m_VerticalDirection;
        [SerializeField] private E_HorizontalDirection m_HorizontalDirection;

        /// <summary>
        /// 可见面板能展示的列数
        /// </summary>
        [SerializeField] private int m_VisiableColumnCount = 1;
        /// <summary>
        /// 可见面板能展示的行数
        /// </summary>
        [SerializeField] private int m_VisiableRowCount = 1;
        /// <summary>
        ///间隙？待定
        /// </summary>
        [SerializeField] private Vector2 m_PaddingForSize;

        public void Init( int dataLength ) {
            m_DatasLength = dataLength;

            ResetData();

            if(m_DatasLength == 0) {
                return;
            }

            if(m_ScrollMode == E_ScrollMode.Vertical) {
                CalculateVerticalRowColumnSize();
            }
            else {
                CalculateHorizontalRowColumnSize();
            }

            if(m_ScrollMode == E_ScrollMode.Vertical) {
                if(m_VerticalDirection == E_VerticalDirection.TopToBottom) {
                    for(var r = 0; r < m_VisiableRowCount + 1 && r < m_DataRowCount; ++r) {
                        for(var c = 0; c < m_VisiableColumnCount; ++c) {
                            var index = r * m_VisiableColumnCount + c;
                            if(index >= 0 && index < m_DatasLength) {
                                AddItem(index, m_ColumnOffsetLookup[c], -m_RowOffsetLookup[r]);
                                //通过第一行计算并设置content的宽度
                                if(r == 0) {
                                    ResizeContentWidth(GetColumnWidth(c));
                                }
                                continue;
                            }
                            break;
                        }
                    }
                }
                else if(m_VerticalDirection == E_VerticalDirection.BottomToTop) {
                    for(var r = 0; r < m_VisiableRowCount + 1 && r < m_DataRowCount; ++r) {
                        for(var c = 0; c < m_VisiableColumnCount; ++c) {
                            var index = r * m_VisiableColumnCount + c;
                            if(index >= 0 && index < m_DatasLength) {
                                AddItem(index, m_ColumnOffsetLookup[c], m_RowOffsetLookup[r]);
                                if(r == 0) {
                                    ResizeContentWidth(GetColumnWidth(c));
                                }
                                continue;
                            }
                            break;
                        }
                    }
                }
            }
            else if(m_ScrollMode == E_ScrollMode.Horizontal) {
                var initColumnCount = m_VisiableColumnCount + 1;
                if(m_HorizontalDirection == E_HorizontalDirection.LeftToRight) {
                    for(var c = 0; c < initColumnCount && c < m_DataColumnCount; ++c) {
                        for(var r = 0; r < m_VisiableRowCount; ++r) {
                            var index = r * m_DataColumnCount + c;
                            if(index >= 0 && index < m_DatasLength) {
                                AddItem(r * m_DataColumnCount + c, m_ColumnOffsetLookup[c], -m_RowOffsetLookup[r]);
                                if(c == 0) {
                                    ResizeContentHeight(GetRowHeight(r));
                                }
                                continue;
                            }
                            break;
                        }
                    }
                }
                else if(m_HorizontalDirection == E_HorizontalDirection.RightToLeft) {
                    for(var c = 0; c < initColumnCount && c < m_DataColumnCount; ++c) {
                        for(var r = 0; r < m_VisiableRowCount; ++r) {
                            var index = r * m_DataColumnCount + c;
                            if(index >= 0 && index < m_DatasLength) {
                                AddItem(r * m_DataColumnCount + c, -m_ColumnOffsetLookup[c], -m_RowOffsetLookup[r]);
                                if(c == 0) {
                                    ResizeContentHeight(GetRowHeight(r));
                                }
                                continue;
                            }
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 滑动到指定比例
        /// </summary>
        /// <param name="_per"></param>
        public void ScrollTo( float _per ) {
            if(m_ScrollMode == E_ScrollMode.Vertical) {
                if(m_ScrollRect) {
                    float per = Mathf.Clamp(1 - _per, 0f, 1f);
                    m_ScrollRect.verticalNormalizedPosition = per;
                }
            }
        }

        /// <summary>
        /// 跳到固定下标
        /// </summary>
        /// <param name="_dataIndex"></param>
        public void JumpToIndex( int _dataIndex ) {
            if(!IsIndexValid(_dataIndex)) {
                return;
            }
            if(m_ScrollMode == E_ScrollMode.Horizontal) {

            }
            else if(m_ScrollMode == E_ScrollMode.Vertical) {

                float visiableHeight = m_ScrollRect.viewport.rect.height;
                int visiableCount = 0;
                float sumHeight =0;
                
                for(int i = m_RowHeightLookup.Count-1;i>=1;i--) {
                    float height = m_RowHeightLookup[i];
                    sumHeight += height;
                    if (sumHeight < visiableHeight) {
                        visiableCount++;
                    }
                }

                var max = m_DataRowCount < visiableCount ? m_DataRowCount: m_DataRowCount - visiableCount;
                if(m_VerticalDirection == E_VerticalDirection.TopToBottom) {
                    var rowIndex = Mathf.Clamp(_dataIndex / m_VisiableColumnCount, 0, max);
                    var pos = m_ScrollRect.content.anchoredPosition;
                    pos.y = m_RowOffsetLookup[rowIndex];
                    m_ScrollRect.content.anchoredPosition = pos;
                }
                else if(m_VerticalDirection == E_VerticalDirection.BottomToTop) {
                    var rowIndex = Mathf.Clamp(_dataIndex / m_VisiableColumnCount, 0, max);
                    var pos = m_ScrollRect.content.anchoredPosition;
                    pos.y = -m_RowOffsetLookup[rowIndex];
                    m_ScrollRect.content.anchoredPosition = pos;
                }
            }
        }

        public T GetTemplateInfo<T>() where T : UnityEngine.Component {
            return m_ItemPrefab.GetComponentInChildren<T>();
        }

        public void RefreshImmediate() {
            foreach(var _item in m_DataIndexToItemLookup.Values) {
                m_OnRefreshItem.Invoke(_item.dataIndex, _item);
            }
        }

        public void ScrollToTop() {
            ScrollTo(0);
        }
        public void ScrollToBottom() {
            ScrollTo(1);
        }

        public void AddItemRefreshListener( OnItemRefresh callback ) {
            m_OnRefreshItem = callback;
        }

        public void AddItemSizeProvider( ItemSizeProvider provider ) {
            m_ItemSizeProvider = provider;
        }

        public void RemoveAllListeners() {
            m_OnRefreshItem = null;
            m_ItemSizeProvider = null;
        }

        public void SetScrollRectMoveTypeToClamped() {
            m_ScrollRect.movementType = ScrollRect.MovementType.Clamped;
        }

        #region 具体内部实现相关
        [SerializeField] private ScrollRect m_ScrollRect;

        /// <summary>
        /// 缓存队列。
        /// </summary>
        private readonly Queue<YBScrollerItem> m_FreeItemQueue = new Queue<YBScrollerItem>();
        /// <summary>
        /// 对应key存的KYScrollerItem
        /// </summary>
        private readonly Dictionary<int, YBScrollerItem> m_DataIndexToItemLookup = new Dictionary<int, YBScrollerItem>();
        /// <summary>
        /// 第一个可见的行
        /// </summary>
        private int m_VisiableFirstRowIndex;
        /// <summary>
        /// 第一个可见的列
        /// </summary>
        private int m_VisiableFirstColumnIndex;
        /// <summary>
        ///当前数据 总共能显示几行
        /// </summary>
        private int m_DataRowCount;
        /// <summary>
        ///当前数据 总共能显示几列
        /// </summary>
        private int m_DataColumnCount;

        /// <summary>
        ///  总数据个数
        /// </summary>
        private int m_DatasLength;
        private float m_RecordContentY;
        private float m_RecordContentX;

        /// <summary>
        /// key:第Index行  value:高度
        /// </summary>
        private readonly Dictionary<int, float> m_RowHeightLookup = new Dictionary<int, float>();
        /// <summary>
        /// key:第Index列  value:宽度
        /// </summary>
        private readonly Dictionary<int, float> m_ColumnWidthLookup = new Dictionary<int, float>();
        /// <summary>
        /// key:第Index行  value:大小
        /// </summary>
        private readonly Dictionary<int, Vector2> m_ItemSizeLookup = new Dictionary<int, Vector2>();
        /// <summary>
        /// k:第Index行，所对应的与第0行的偏移
        /// </summary>
        private readonly Dictionary<int, float> m_RowOffsetLookup = new Dictionary<int, float>();
        /// <summary>
        /// k:第Index行，所对应的与第0列的偏移
        /// </summary>
        private readonly Dictionary<int, float> m_ColumnOffsetLookup = new Dictionary<int, float>();

        private OnItemRefresh m_OnRefreshItem;
        private ItemSizeProvider m_ItemSizeProvider;

        protected void Awake() {
            //InitScrollRect();
            m_VisiableFirstRowIndex = 0;
            if(m_ItemPrefab != null) {
                m_ItemPrefab.SetActive(false);
            }
        }

        /// <summary>
        /// 设置位置，进行当前index的刷新
        /// </summary>
        private void AddItem( int _dataIndex, float _x, float _y ) {
            if(m_DataIndexToItemLookup.ContainsKey(_dataIndex)) {
            }
            else {
                YBScrollerItem item = Request(_dataIndex);
                item.Rt.anchoredPosition = new Vector2(_x, _y);
                m_DataIndexToItemLookup[_dataIndex] = item;
                if(_dataIndex < m_DatasLength && _dataIndex >= 0) {
                    m_OnRefreshItem?.Invoke(_dataIndex, item);
                }
            }
        }

        private float GetItemWidth( int dataIndex ) {
            //数据中没有存这个item的大小。重新获取下大小，存入数据
            if(!m_ItemSizeLookup.TryGetValue(dataIndex, out var newItemSize)) {
                if(m_ItemSizeProvider != null) {
                    newItemSize = m_ItemSizeProvider(dataIndex);
                }
                else {
                    newItemSize = m_DefaultItemSize;
                }
                m_ItemSizeLookup.Add(dataIndex, newItemSize);
            }
            // 计算宽度在列表中的索引
            var columnWidthIndex = m_ScrollMode == E_ScrollMode.Vertical ? dataIndex % m_VisiableColumnCount : dataIndex % m_DataColumnCount;
            // 设置行宽，取最大值
            if(!m_ColumnWidthLookup.ContainsKey(columnWidthIndex)) {
                m_ColumnWidthLookup.Add(columnWidthIndex, newItemSize.x);
            }
            else {
                if(m_ColumnWidthLookup[columnWidthIndex] < newItemSize.x) {
                    m_ColumnWidthLookup[columnWidthIndex] = newItemSize.x;
                }
            }
            //Debug.Log($" -------- itemWidth = {dataIndex} => {m_ColumnWidthLookup[columnWidthIndex]}");
            return m_ColumnWidthLookup[columnWidthIndex];
        }

        private float GetItemHeight( int dataIndex ) {

            if(!m_ItemSizeLookup.TryGetValue(dataIndex, out var newItemSize)) {
                if(m_ItemSizeProvider != null) {
                    newItemSize = m_ItemSizeProvider(dataIndex);
                }
                else {
                    newItemSize = m_DefaultItemSize;
                }
                m_ItemSizeLookup.Add(dataIndex, newItemSize);
            }

            // 计算高度在列表中的索引
            var rowHeightIndex = m_ScrollMode == E_ScrollMode.Vertical ? dataIndex / m_VisiableColumnCount : dataIndex / m_DataColumnCount;
            // 设置行高，取最大值  （k:这个索引对应的行的宽度为这行item Y的最大值）
            if(!m_RowHeightLookup.ContainsKey(rowHeightIndex)) {
                m_RowHeightLookup.Add(rowHeightIndex, newItemSize.y);
            }
            else {
                if(m_RowHeightLookup[rowHeightIndex] < newItemSize.y) {
                    m_RowHeightLookup[rowHeightIndex] = newItemSize.y;
                }
            }
            return m_RowHeightLookup[rowHeightIndex];
        }

        private YBScrollerItem Request( int _dataIndex ) {
            YBScrollerItem item;
            if(m_FreeItemQueue.Count > 0) {
                item = m_FreeItemQueue.Dequeue();
            }
            else {
                item = Instantiate(m_ItemPrefab).AddComponent<YBScrollerItem>();
#if UNITY_EDITOR
                item.name = "item_" + item.GetHashCode();
#endif
            }
            item.dataIndex = _dataIndex;
            if(!item.gameObject.activeSelf) {
                item.gameObject.SetActive(true);
            }
            item.gameObject.layer = LayerMask.NameToLayer("UI");
            item.Rt.SetParent(m_ScrollRect.content);
            item.Rt.localPosition = Vector3.zero;
            item.Rt.localRotation = Quaternion.identity;
            item.Rt.localScale = Vector3.one;
            if(m_ScrollMode == E_ScrollMode.Vertical) {
                if(m_VerticalDirection == E_VerticalDirection.TopToBottom) {
                    item.Rt.pivot = item.Rt.anchorMin = item.Rt.anchorMax = Vector2.up;
                }
                else if(m_VerticalDirection == E_VerticalDirection.BottomToTop) {
                    item.Rt.pivot = item.Rt.anchorMin = item.Rt.anchorMax = Vector2.zero;
                }
            }
            else if(m_ScrollMode == E_ScrollMode.Horizontal) {
                if(m_HorizontalDirection == E_HorizontalDirection.LeftToRight) {
                    item.Rt.pivot = item.Rt.anchorMin = item.Rt.anchorMax = Vector2.up;
                }
                else if(m_HorizontalDirection == E_HorizontalDirection.RightToLeft) {
                    item.Rt.pivot = item.Rt.anchorMin = item.Rt.anchorMax = Vector2.one;
                }
            }
            item.Rt.sizeDelta = m_ItemSizeLookup[_dataIndex];
            return item;
        }

        private void Release( YBScrollerItem rt ) {
            rt.gameObject.SetActive(false);
            m_DataIndexToItemLookup.Remove(rt.dataIndex);
            m_FreeItemQueue.Enqueue(rt);
        }

        private void ReleaseWithDataIndex( int _dataIndex ) {
            if(m_DataIndexToItemLookup.TryGetValue(_dataIndex, out var _item)) {
                Release(_item);
            }
            else {
                //hale说可以频闭了
                //Debug.Log($"找不到数据索引{_dataIndex}对应的Item");
            }
        }

        private float GetRowHeight( int rowIndex ) {
            if(!m_RowHeightLookup.ContainsKey(rowIndex)) {
                Debug.Log("!!!!!!!!!!!!!!! --- get row height error: " + rowIndex);
            }
            return m_RowHeightLookup.TryGetValue(rowIndex, out var _height) ? _height : -1;
        }

        private float GetColumnWidth( int columnIndex ) {
            if(!m_ColumnWidthLookup.ContainsKey(columnIndex)) {
                Debug.Log("!!!!!!!!!!!!!!! --- get column width error: " + columnIndex);
            }
            return m_ColumnWidthLookup.TryGetValue(columnIndex, out var _width) ? _width : -1;
        }

        private void ResizeContentHeight( float _height ) {
            if(_height > 0) {
                m_ScrollRect.content.sizeDelta += new Vector2(0, _height);
            }
        }

        private void ResizeContentWidth( float _width ) {
            if(_width > 0) {
                m_ScrollRect.content.sizeDelta += new Vector2(_width, 0);
            }
        }

        private void CalculateHorizontalRowColumnSize() {
            //k:水平布局，所以关心总列数
            m_DataColumnCount = Mathf.CeilToInt(m_DatasLength * 1f / m_VisiableRowCount);
            var offset = 0f;
            for(int c = 0; c < m_DataColumnCount; ++c) {
                for(int r = 0; r < m_VisiableRowCount; ++r) {
                    var index = r * m_DataColumnCount + c;
                    if(index > m_DatasLength - 1) {
                        break;
                    }
                    GetItemWidth(index);
                }
                if(c > 0) {
                    offset += GetColumnWidth(c - 1);
                }
                m_ColumnOffsetLookup.Add(c, offset);
            }
            offset = 0;
            for(int r = 0; r < m_VisiableRowCount; ++r) {
                for(int c = 0; c < m_DataColumnCount; ++c) {
                    var index = r * m_DataColumnCount + c;
                    if(index > m_DatasLength - 1) {
                        break;
                    }
                    GetItemHeight(index);
                }
                if(r > 0) {
                    offset += GetRowHeight(r - 1);
                }
                m_RowOffsetLookup.Add(r, offset);
            }

            ResizeContentWidth(m_ColumnOffsetLookup[m_DataColumnCount - 1] + GetItemWidth(m_DatasLength - 1) + m_PaddingForSize.x);
        }

        /// <summary>
        /// k:计算垂直布局 行高和列宽
        /// </summary>
        private void CalculateVerticalRowColumnSize() {
            //k:垂直布局，所以关心总行数
            m_DataRowCount = Mathf.CeilToInt(m_DatasLength * 1f / m_VisiableColumnCount);
            // 遍历计算所有的单位的宽高
            // 目的是确定行高和列宽
            var offset = 0f;
            //k:这边横着去遍历，因为要确定每行的高度
            for(int i = 0; i < m_DataRowCount; ++i) {
                for(int j = 0; j < m_VisiableColumnCount; ++j) {
                    var index = i * m_VisiableColumnCount + j;
                    if(index > m_DatasLength - 1) {
                        break;
                    }
                    GetItemHeight(index);
                }
                if(i > 0) {
                    offset += GetRowHeight(i - 1);
                }
                m_RowOffsetLookup.Add(i, offset);
            }
            offset = 0;
            //k:这里竖着去遍历，因为要确定每列的宽度
            for(int i = 0; i < m_VisiableColumnCount; ++i) {
                for(int j = 0; j < m_DataRowCount; ++j) {
                    var index = j * m_VisiableColumnCount + i;
                    if(index > m_DatasLength - 1) {
                        break;
                    }
                    GetItemWidth(index);
                }
                if(i > 0) {
                    offset += GetColumnWidth(i - 1);
                }
                m_ColumnOffsetLookup.Add(i, offset);
            }
            //k:到这一步之前，可以获得每行的高、每列的宽、每个item的宽高字典、每行每列相对于起点的偏移（也就是可以知道每个item的坐标）字典
            //k:因为是垂直布局，所以content的高度是最后一行的Y方向偏移，加上最后一行的高度,再加上设定的padding
            ResizeContentHeight(m_RowOffsetLookup[m_DataRowCount - 1] + GetItemHeight(m_DatasLength - 1) + m_PaddingForSize.y);
        }

        void Update() {
            if(m_ScrollRect.content) {
                HandleVertical();
                HandleHorizontal();
            }
        }

        private void HandleHorizontal() {
            if(m_ScrollMode != E_ScrollMode.Horizontal) {
                return;
            }
            var contentX = m_ScrollRect.content.anchoredPosition.x;
            //只滑动非常小的距离，不做处理
            if(Mathf.Abs(m_RecordContentX - contentX) < .5f) {
                return;
            }

            //往左滑
            var isScrollLeft = contentX < m_RecordContentX;
            if(m_HorizontalDirection == E_HorizontalDirection.LeftToRight) {
                if(contentX > 0) {
                    contentX = 0;
                }
            }
            if(isScrollLeft) {
                if(m_HorizontalDirection == E_HorizontalDirection.LeftToRight) {
                    //当前并未滑到最右侧
                    if(m_VisiableFirstColumnIndex + m_VisiableColumnCount < m_DataColumnCount) {
                        //当前滑动到的位置 超出第一个可见列的偏移
                        while(Mathf.Abs(contentX) >= m_ColumnOffsetLookup[m_VisiableFirstColumnIndex]) {
                            if(m_VisiableFirstColumnIndex > 1) {
                                for(var i = m_VisiableRowCount - 1; i >= 0; --i) {
                                    var idx = i * m_DataColumnCount + m_VisiableFirstColumnIndex - 2;
                                    if(IsIndexValid(idx)) {
                                        ReleaseWithDataIndex(idx);
                                    }
                                    if(m_VisiableFirstColumnIndex >= m_DataColumnCount - m_VisiableColumnCount - 1) {
                                        idx = i * m_DataColumnCount + m_VisiableFirstColumnIndex - 1;
                                        if(IsIndexValid(idx)) {
                                            ReleaseWithDataIndex(idx);
                                        }
                                    }
                                }
                            }
                            var index = m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1;
                            if(index >= 0 && index < m_DataColumnCount) {
                                for(var i = 0; i < m_VisiableRowCount; ++i) {
                                    index = i * m_DataColumnCount + m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1;
                                    if(IsIndexValid(index)) {
                                        AddItem(index, m_ColumnOffsetLookup[m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1], -m_RowOffsetLookup[i]);
                                        continue;
                                    }
                                    break;
                                }
                            }

                            m_VisiableFirstColumnIndex += 1;

                            if(m_VisiableFirstColumnIndex + m_VisiableColumnCount >= m_DataColumnCount - 1) {
                                break;
                            }
                        }
                    }
                }
                else if(m_HorizontalDirection == E_HorizontalDirection.RightToLeft) {
                    if(m_VisiableFirstColumnIndex > 0) {
                        while(contentX < m_ColumnOffsetLookup[m_VisiableFirstColumnIndex]) {
                            if(m_VisiableFirstColumnIndex + m_VisiableColumnCount < m_DataColumnCount - 1) {
                                for(var i = m_VisiableRowCount - 1; i >= 0; --i) {
                                    var idx = i * m_DataColumnCount + m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1;
                                    if(IsIndexValid(idx)) {
                                        ReleaseWithDataIndex(idx);
                                    }
                                    if(m_VisiableFirstColumnIndex <= 1) {
                                        ReleaseWithDataIndex(i * m_DataColumnCount + m_VisiableFirstColumnIndex + m_VisiableColumnCount);
                                    }
                                }
                            }

                            var index = m_VisiableFirstColumnIndex - 1;
                            var vaild = index >= 0 && index < m_DatasLength;

                            if(vaild) {
                                for(var i = 0; i < m_VisiableRowCount; ++i) {
                                    index = i * m_DataColumnCount + m_VisiableFirstColumnIndex - 2;
                                    if(IsIndexValid(index)) {
                                        AddItem(index, -m_ColumnOffsetLookup[m_VisiableFirstColumnIndex - 2], -m_RowOffsetLookup[i]);
                                        continue;
                                    }
                                    break;
                                }
                            }

                            m_VisiableFirstColumnIndex -= 1;
                            if(m_VisiableFirstColumnIndex <= 0) {
                                break;
                            }
                        }
                    }
                }
            }
            else {
                if(m_HorizontalDirection == E_HorizontalDirection.LeftToRight) {
                    if(m_VisiableFirstColumnIndex > 0) {
                        while(Mathf.Abs(contentX) <= m_ColumnOffsetLookup[m_VisiableFirstColumnIndex]) {
                            if(m_VisiableFirstColumnIndex + m_VisiableColumnCount < m_DataColumnCount - 1) {
                                for(var i = m_VisiableRowCount - 1; i >= 0; --i) {
                                    var idx = i * m_DataColumnCount + m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1;
                                    if(IsIndexValid(idx)) {
                                        ReleaseWithDataIndex(idx);
                                    }
                                    if(m_VisiableFirstColumnIndex <= 1) {
                                        idx = i * m_DataColumnCount + m_VisiableFirstColumnIndex + m_VisiableColumnCount;
                                        if(IsIndexValid(idx)) {
                                            ReleaseWithDataIndex(idx);
                                        }
                                    }
                                }
                            }
                            for(var i = 0; i < m_VisiableRowCount; ++i) {
                                var index = i * m_DataColumnCount + m_VisiableFirstColumnIndex - 2;
                                if(IsIndexValid(index)) {
                                    AddItem(index, m_ColumnOffsetLookup[m_VisiableFirstColumnIndex - 2], -m_RowOffsetLookup[i]);
                                    continue;
                                }
                                break;
                            }
                            m_VisiableFirstColumnIndex -= 1;
                            if(m_VisiableFirstColumnIndex <= 0) {
                                break;
                            }
                        }
                    }
                }
                else if(m_HorizontalDirection == E_HorizontalDirection.RightToLeft) {
                    if(m_VisiableFirstColumnIndex + m_VisiableColumnCount < m_DataColumnCount) {
                        while(contentX > m_ColumnOffsetLookup[m_VisiableFirstColumnIndex]) {
                            if(m_VisiableFirstColumnIndex > 1) {
                                for(var i = 0; i < m_VisiableRowCount; ++i) {
                                    ReleaseWithDataIndex(i * m_DataColumnCount + m_VisiableFirstColumnIndex - 2);
                                    if(m_VisiableFirstColumnIndex + m_VisiableColumnCount >= m_DataColumnCount - 1) {
                                        ReleaseWithDataIndex(i * m_DataColumnCount + m_VisiableFirstColumnIndex - 1);
                                    }
                                }
                            }

                            var index = m_VisiableFirstColumnIndex + m_VisiableColumnCount;
                            var vaild = index >= 0 && index < m_DatasLength;
                            if(vaild) {
                                for(var i = 0; i < m_VisiableRowCount; ++i) {
                                    index = i * m_DataColumnCount + m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1;
                                    if(index >= 0 && index < m_DatasLength) {
                                        var key = m_VisiableFirstColumnIndex + m_VisiableColumnCount + 1;
                                        if(m_ColumnOffsetLookup.TryGetValue(key, out var _offset)) {
                                            AddItem(index, -_offset, -m_RowOffsetLookup[i]);
                                        }
                                        continue;
                                    }
                                    break;
                                }
                            }
                            m_VisiableFirstColumnIndex += 1;
                            if(m_VisiableFirstColumnIndex + m_VisiableColumnCount >= m_DataColumnCount - 1) {
                                break;
                            }
                        }
                    }
                }
            }
            m_RecordContentX = contentX;
        }

        private void HandleVertical() {
            if(m_ScrollMode != E_ScrollMode.Vertical) {
                return;
            }
            var contentY = m_ScrollRect.content.anchoredPosition.y;
            //k:0.5f为设置的Y轴移动偏移量，当拖动超过这个偏移量，就会触发
            if(Mathf.Abs(m_RecordContentY - contentY) < .5f) {
                return;
            }
            var isScrollUp = contentY > m_RecordContentY;

            if(m_VerticalDirection == E_VerticalDirection.BottomToTop) {
                if(contentY > 0) {
                    contentY = 0;
                }
            }
            if(isScrollUp) {
                if(m_VerticalDirection == E_VerticalDirection.TopToBottom) {
                    //k:如果当前视图已展示所有行，则不需要更新（上拉，已拉到底）
                    if(m_VisiableFirstRowIndex + m_VisiableRowCount < m_DataRowCount) {
                        //k:如果拖动偏移大于当前视图第一行的高，则需要更新元素
                        while(contentY > m_RowOffsetLookup[m_VisiableFirstRowIndex]) {
                            //k:如果已经到第3行及以上
                            if(m_VisiableFirstRowIndex > 1) {
                                for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                    //k:把前2行的元素回收
                                    var index = ( m_VisiableFirstRowIndex - 2 ) * m_VisiableColumnCount + i;
                                    if(IsIndexValid(index)) {
                                        ReleaseWithDataIndex(index);
                                    }
                                    if(m_VisiableFirstRowIndex >= m_DataRowCount - m_VisiableRowCount - 1) {
                                        //k:把前1行的元素回收
                                        index = ( m_VisiableFirstRowIndex - 1 ) * m_VisiableColumnCount + i;
                                        if(IsIndexValid(index)) {
                                            ReleaseWithDataIndex(index);
                                        }
                                    }
                                }
                            }
                            var bottomRowIndex = m_VisiableFirstRowIndex + m_VisiableRowCount + 1;
                            if(bottomRowIndex < m_DataRowCount) {
                                for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                    var index = bottomRowIndex * m_VisiableColumnCount + i;
                                    if(IsIndexValid(index)) {
                                        AddItem(index, m_ColumnOffsetLookup[i], -m_RowOffsetLookup[bottomRowIndex]);
                                    }
                                }
                            }
                            m_VisiableFirstRowIndex += 1;
                            if(m_VisiableFirstRowIndex + m_VisiableRowCount >= m_DataRowCount - 1) {
                                break;
                            }
                        }
                    }
                }
                else if(m_VerticalDirection == E_VerticalDirection.BottomToTop) {
                    if(m_VisiableFirstRowIndex > 0) {
                        while(Mathf.Abs(contentY) < m_RowOffsetLookup[m_VisiableFirstRowIndex]) {
                            for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                var index = ( m_VisiableFirstRowIndex + m_VisiableRowCount + 1 ) * m_VisiableColumnCount + i;
                                if(IsIndexValid(index)) {
                                    ReleaseWithDataIndex(index);
                                }
                                if(m_VisiableFirstRowIndex <= 1) {
                                    index = ( m_VisiableFirstRowIndex + m_VisiableRowCount ) * m_VisiableColumnCount + i;
                                    if(IsIndexValid(index)) {
                                        ReleaseWithDataIndex(index);
                                    }
                                }
                            }
                            for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                var index = ( m_VisiableFirstRowIndex - 2 ) * m_VisiableColumnCount;
                                if(IsIndexValid(index)) {
                                    AddItem(index + i, m_ColumnOffsetLookup[i], m_RowOffsetLookup[m_VisiableFirstRowIndex - 2]);
                                }
                            }
                            m_VisiableFirstRowIndex -= 1;
                            if(m_VisiableFirstRowIndex <= 0) {
                                break;
                            }
                        }
                    }
                }
            }
            else {
                if(m_VerticalDirection == E_VerticalDirection.TopToBottom) {
                    if(m_VisiableFirstRowIndex > 0) {
                        while(contentY < m_RowOffsetLookup[m_VisiableFirstRowIndex]) {
                            if(m_VisiableFirstRowIndex + m_VisiableRowCount < m_DataRowCount - 1) {
                                for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                    var index = ( m_VisiableFirstRowIndex + m_VisiableRowCount + 1 ) * m_VisiableColumnCount + i;
                                    if(IsIndexValid(index)) {
                                        ReleaseWithDataIndex(index);
                                    }
                                    if(m_VisiableFirstRowIndex <= 1) {
                                        index = ( m_VisiableFirstRowIndex + m_VisiableRowCount ) * m_VisiableColumnCount + i;
                                        if(IsIndexValid(index)) {
                                            ReleaseWithDataIndex(index);
                                        }
                                    }
                                }
                            }
                            for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                var index = ( m_VisiableFirstRowIndex - 2 ) * m_VisiableColumnCount + i;
                                if(IsIndexValid(index)) {
                                    AddItem(index, m_ColumnOffsetLookup[i], -m_RowOffsetLookup[m_VisiableFirstRowIndex - 2]);
                                }
                            }
                            m_VisiableFirstRowIndex -= 1;
                            if(m_VisiableFirstRowIndex <= 0) {
                                break;
                            }
                        }
                    }
                }
                else if(m_VerticalDirection == E_VerticalDirection.BottomToTop) {
                    if(m_VisiableFirstRowIndex + m_VisiableRowCount < m_DataRowCount) {
                        while(Mathf.Abs(contentY) > m_RowOffsetLookup[m_VisiableFirstRowIndex]) {
                            if(m_VisiableFirstRowIndex > 1) {
                                for(var i = 0; i < m_VisiableColumnCount; ++i) {
                                    var index = ( m_VisiableFirstRowIndex - 2 ) * m_VisiableColumnCount + i;
                                    if(IsIndexValid(index)) {
                                        ReleaseWithDataIndex(index);
                                    }
                                    if(m_VisiableFirstRowIndex >= m_DataRowCount - m_VisiableRowCount - 1) {
                                        index = ( m_VisiableFirstRowIndex - 1 ) * m_VisiableColumnCount + i;
                                        if(IsIndexValid(index)) {
                                            ReleaseWithDataIndex(index);
                                        }
                                    }
                                }
                            }
                            var bottomRowIndex = m_VisiableFirstRowIndex + m_VisiableRowCount + 1;
                            if(bottomRowIndex < m_DataRowCount) {
                                for(var j = 0; j < m_VisiableColumnCount; ++j) {
                                    var index = bottomRowIndex * m_VisiableColumnCount;
                                    if(IsIndexValid(index)) {
                                        AddItem(index + j, m_ColumnOffsetLookup[j], m_RowOffsetLookup[bottomRowIndex]);
                                    }
                                }
                            }
                            m_VisiableFirstRowIndex += 1;
                            if(m_VisiableFirstRowIndex + m_VisiableRowCount >= m_DataRowCount - 1) {
                                break;
                            }
                        }
                    }
                }
            }
            m_RecordContentY = contentY;
        }

        /// <summary>
        /// 重设数据
        /// </summary>
        private void ResetData() {
            var removeDataIndexList = new List<int>();
            foreach(var item in m_DataIndexToItemLookup.Values) {
                item.gameObject.SetActive(false);
                m_FreeItemQueue.Enqueue(item);
                removeDataIndexList.Add(item.dataIndex);
            }
            foreach(var dataIndex in removeDataIndexList) {
                m_DataIndexToItemLookup.Remove(dataIndex);
            }
            removeDataIndexList.Clear();
            m_DataIndexToItemLookup.Clear();
            m_ScrollRect.content.anchoredPosition = Vector3.zero;
            m_ScrollRect.content.sizeDelta = Vector2.zero;
            m_RowHeightLookup.Clear();
            m_RowOffsetLookup.Clear();
            m_ColumnOffsetLookup.Clear();
            m_ColumnWidthLookup.Clear();
            m_ItemSizeLookup.Clear();
            m_DataIndexToItemLookup.Clear();
            m_RecordContentY = 0;
            m_RecordContentX = 0;
            m_VisiableFirstRowIndex = 0;
            m_VisiableFirstColumnIndex = 0;
            m_DataRowCount = 0;
            m_DataColumnCount = 0;
    }

        /// <summary>
        /// 是否为有效数据
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        private bool IsIndexValid( int _index ) {
            return _index >= 0 && _index < m_DatasLength;
        }

        #endregion

        #region 新增逻辑

        /// <summary>
        /// 重计算目标宽高
        /// </summary>
        public void ChangeTargetSize( ) {

            if(m_ScrollMode == E_ScrollMode.Vertical) {
                CalculateVerticalRowColumnSize();
            }
            else {
                CalculateHorizontalRowColumnSize();
            }
        }

        /// <summary>
        /// 获取目标位置的y坐标
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public float GetTargetIndexRowOffset( int _index ) {
            var offset = 0f;
            for(int r = 0; r < m_DataRowCount; ++r) {
                var index = r * m_VisiableColumnCount;

                if(r > 0) {
                    offset += GetRowHeight(r - 1);
                }
                if(_index == index) {
                    break;
                }
            }
            if(m_VerticalDirection == E_VerticalDirection.TopToBottom) {
                return -offset;
            }
            else {
                return offset;
            }
        }
        #endregion
    }
}