using ParadoxNotion;
using NodeCanvas.Framework;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace NodeCanvas.DialogueTrees
{

    ///<summary>An interface to use for whats being said by a dialogue actor</summary>
    public interface IStatement
    {
        string         insId   { get; }
        string         lanKey  { get; }
        string         text    { get; }
        AudioClip      audio   { get; }
        Sprite         image   { get; }
        bool           flip    { get; }
        AnimationCurve Flow    { get;}
        StatementType  Type    { get; }
        float          Duration{ get; }
        string         meta    { get; }
    }

    ///<summary>Holds data of what's being said usualy by an actor</summary>
    [System.Serializable]
    public class Statement : IStatement
    {

        [SerializeField] private string _insId = string.Empty;
        [SerializeField] private string _lanKey = string.Empty;
        [SerializeField] private string _text = string.Empty;

        [SerializeField] private Sprite _image = null;
        [SerializeField] private bool _flip = false;

        [SerializeField] private AudioClip _audio;
        [SerializeField] private AnimationCurve _flow = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private StatementType _type;
        [SerializeField] private string _meta = string.Empty;
        [SerializeField]
        [Tooltip("单条对话持续时间(默认2s)")]
        private float _duration = 2.0f;

        public string insId
        {
            get { return this._insId; }
            set { this._insId = value; }
        }

        public string lanKey
        {
            get { return this._lanKey; }
            set { this._lanKey = value; }
        }

        public string text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Sprite image
        {
            get { return _image; }
            set { _image = value; }
        }

        public bool flip
        {
            get { return _flip; }
            set { _flip = value; }
        }

        public AudioClip audio
        {
            get { return _audio; }
            set { _audio = value; }
        }

        public AnimationCurve Flow
        {
            get { return _flow; }
            set { _flow = value; }
        }

        public StatementType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public string meta
        {
            get { return _meta; }
            set { _meta = value; }
        }

        //required
        public Statement() { }
        public Statement(string text)
        {
            this.text = text;
        }

        public Statement(string text, AudioClip audio)
        {
            this.text = text;
            this.audio = audio;
        }

        public Statement(string text, AudioClip audio, string meta)
        {
            this.text = text;
            this.audio = audio;
            this.meta = meta;
        }

        ///<summary>Replace the text of the statement found in brackets, with blackboard variables ToString and returns a Statement copy</summary>
        public IStatement BlackboardReplace(IBlackboard bb)
        {
            var copy = ParadoxNotion.Serialization.JSONSerializer.Clone<Statement>(this);

            copy.text = copy.text.ReplaceWithin('[', ']', (input) =>
            {
                object o = null;
                if (bb != null)
                { //referenced blackboard replace
                    var v = bb.GetVariable(input, typeof(object));
                    if (v != null) { o = v.value; }
                }

                if (input.Contains("/"))
                { //global blackboard replace
                    var globalBB = GlobalBlackboard.Find(input.Split('/').First());
                    if (globalBB != null)
                    {
                        var v = globalBB.GetVariable(input.Split('/').Last(), typeof(object));
                        if (v != null) { o = v.value; }
                    }
                }
                return o != null ? o.ToString() : input;
            });

            return copy;
        }

        public override string ToString()
        {
            return text;
        }
    }
    
    
    [System.Serializable]
    public class CGNodeData
    {
        public string UsingUXName;
        public string CGImgAddress;
    }
}