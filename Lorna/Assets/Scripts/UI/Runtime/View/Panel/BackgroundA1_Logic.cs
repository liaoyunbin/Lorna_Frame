namespace FrameWork_UI
{
    using UnityEngine.UI;
    using System.Collections;
    using UnityEditor.Experimental.GraphView;
    using System.Collections.Generic;

    //转为UTF8格式：
    public partial class BackgroundA1_Design
    {
        public Dictionary<Selectable, Dictionary<Direction, Selectable>> map = new Dictionary<Selectable, Dictionary<Direction, Selectable>>();
    }
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
    }
}
