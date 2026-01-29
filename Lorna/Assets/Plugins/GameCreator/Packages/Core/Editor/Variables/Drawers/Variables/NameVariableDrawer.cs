using GameCreator.Editor.Common;
using GameCreator.Runtime.Variables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Variables
{
   [CustomPropertyDrawer(typeof(NameVariable))]
    public class NameVariableDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            
            SerializedProperty propertyName = property.FindPropertyRelative("m_Name");
            SerializedProperty propertyValue = property.FindPropertyRelative("m_Value");
            SerializedProperty propertyDataType = property.FindPropertyRelative("m_DataType");
            SerializedProperty propertyReset = property.FindPropertyRelative("m_ResetOnReturn");
            SerializedProperty propertyNotResetOnTempReturn = property.FindPropertyRelative("m_NotResetOnTempReturn");
//            SerializedProperty propertyResetUsedStateOnReturn = property.FindPropertyRelative("m_ResetUsedStateOnReturn");
            SerializedProperty propertyDesc = property.FindPropertyRelative("m_Desc");



            PropertyField fieldName = new PropertyField(propertyName);
            PropertyField fieldDataType = new PropertyField(propertyDataType);
            PropertyField fieldDesc = new PropertyField(propertyDesc);
            PropertyField fieldReset = new PropertyField(propertyReset);
            PropertyField fieldNotReset = new PropertyField(propertyNotResetOnTempReturn);
//            PropertyField fieldResetUsedStateOnReturn = new PropertyField(propertyResetUsedStateOnReturn);

            PropertyElement fieldValue = new PropertyElement(
                propertyValue,
                propertyValue.displayName,
                true
            );

            root.Add(fieldName);
            root.Add(new SpaceSmaller());
            root.Add(fieldValue);
            root.Add(new SpaceSmaller());
            root.Add(fieldDataType);
            root.Add(new SpaceSmaller());
            root.Add(fieldReset);
            root.Add(new SpaceSmaller());
            root.Add(fieldNotReset);
            root.Add(new SpaceSmaller());
//            root.Add(fieldResetUsedStateOnReturn);
//            root.Add(new SpaceSmaller());
            root.Add(fieldDesc);
       

            return root;
        }
    }
}