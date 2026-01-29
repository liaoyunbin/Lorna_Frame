using GameCreator.Runtime.Variables;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
    public class PropertyVariableView : TVariableView<PropertyVariable>
    {
        private const string LABEL_NAME = "Name";

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Title => this.m_Variable.Title;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PropertyVariableView(PropertyVariable variable) : base(variable)
        {
            this.SetupHead();
            this.SetupBody();
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------
        
        protected override VisualElement MakeBody()
        {
            VisualElement container = new VisualElement();
            this.GetFieldValue(container);
            return container;
        }
    }
}