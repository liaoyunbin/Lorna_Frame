using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats
{
    [CustomEditor(typeof(Formula))]
    public class FormulaEditor : UnityEditor.Editor
    {
        private const int VALIDATE_DELAY_MS = 500;
        
        private const string USS_PATH = EditorPaths.PACKAGES + "Stats/Editor/StyleSheets/Formula";
        
        private const string PROP_FORMULA = "m_Formula";
        private const string PROP_TABLE = "m_Table";

        private const string CLASS_TITLE = "gc-stats-formula-title";
        private const string CLASS_MONOSPACE = "gc-monospace";
        
        private const string NAME_FORMULA = "GC-Stats-Formula-Formula";
        private const string NAME_TABLE = "GC-Stats-Formula-Table";

        private const string LABEL_TABLE = "Table (optional)";
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        private VisualElement m_Root;
        private VisualElement m_Message;
        
        private IVisualElementScheduledItem m_ScheduleValidation;
        
        // PAINT METHODS: -------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            
            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) this.m_Root.styleSheets.Add(sheet);
            
            SerializedProperty propertyFormula = this.serializedObject.FindProperty(PROP_FORMULA);
            SerializedProperty propertyTable = this.serializedObject.FindProperty(PROP_TABLE);

            Label labelTitle = new Label("Formula");
            labelTitle.AddToClassList(CLASS_TITLE);
            
            this.m_Message = new VisualElement();

            TextField fieldFormula = new TextField(string.Empty, int.MaxValue, false, false, '*')
            {
                name = NAME_FORMULA,
                bindingPath = propertyFormula.propertyPath,
                multiline = true
            };

            fieldFormula.AddToClassList(CLASS_MONOSPACE);

            PropertyField fieldTable = new PropertyField(propertyTable, LABEL_TABLE)
            {
                name = NAME_TABLE
            };

            fieldFormula.RegisterValueChangedCallback(_ =>
            {
                this.m_Message.Clear();
                
                this.m_ScheduleValidation ??= this.m_Root.schedule.Execute(this.RefreshValidation);
                this.m_ScheduleValidation.ExecuteLater(VALIDATE_DELAY_MS);
            });
            
            fieldTable.RegisterValueChangeCallback(_ =>
            {
                this.RefreshValidation();
            });
            
            this.m_Root.Add(labelTitle);
            this.m_Root.Add(fieldFormula);
            this.m_Root.Add(this.m_Message);

            this.RefreshValidation();
            this.PrintDocumentation();

            this.m_Root.Add(fieldTable);
            return this.m_Root;
        }

        private void RefreshValidation()
        {
            string formula = this.serializedObject
                .FindProperty(PROP_FORMULA)
                .stringValue;
            
            Table table = this.serializedObject
                .FindProperty(PROP_TABLE)
                .objectReferenceValue as Table;
            
            this.m_Message.Clear();
            string message = ValidateFormula(formula, table);
            
            if (!string.IsNullOrEmpty(message)) this.m_Message.Add(new ErrorMessage(message));
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // VALIDATION: ----------------------------------------------------------------------------
        
        private static string ValidateFormula(string formula, Table table)
        {
            if (!ValidateDelimiters(formula, '(', ')')) return "Invalid use of parenthesis";
            if (!ValidateDelimiters(formula, '[', ']')) return "Invalid use of brackets";
            if (!ValidateTable(formula, table)) return "Formula tries to access a null Table reference";
            return string.Empty;
        }

        private static bool ValidateDelimiters(string formula, char open, char close)
        {
            Stack<char> delimiters = new Stack<char>();

            foreach (char character in formula)
            {
                if (character == open) delimiters.Push(character);
                else if (character == close)
                {
                    if (delimiters.Count <= 0 || delimiters.Peek() != open) return false;
                    delimiters.Pop();
                }
            }

            return delimiters.Count <= 0;
        }

        private static bool ValidateTable(string formula, Table table)
        {
            if (!formula.Contains("table:")) return true;
            return table != null;
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        // DOCUMENTATION: -------------------------------------------------------------------------

        private void PrintDocumentation()
        {
            VisualElement documentation = new VisualElement();
            
            Label labelTitle = new Label("Help");
            labelTitle.AddToClassList(CLASS_TITLE);
            documentation.Add(labelTitle);

            documentation.Add(new FormulaHelpTool(
                "Stats",
                new FormulaHelpTool.Parameter(
                    "source.base[name]", 
                    "返回当前对象的名为name的Stats属性基础值"
                ),
                new FormulaHelpTool.Parameter(
                    "source.stat[name]", 
                    "返回当前对象的名为name的Stats属性当前值"
                ),
                new FormulaHelpTool.Parameter(
                    "source.attr[name]", 
                    "返回当前对象的名为name的Attr变量当前值"
                ),
                new FormulaHelpTool.Parameter(
                    "target.base[name]", 
                    "返回目标对象的名为name的Stats属性基础值"
                ),
                new FormulaHelpTool.Parameter(
                    "target.stat[name]", 
                    "返回目标对象的名为name的Stats属性当前值"
                ),
                new FormulaHelpTool.Parameter(
                    "target.attr[name]", 
                    "返回目标对象的名为name的Attr变量当前值"
                )
            ));

            documentation.Add(new FormulaHelpTool(
                "Variables",
                new FormulaHelpTool.Parameter(
                    "source.var[name]", 
                    "Returns the value of a Local Name Variable attached to the source game object."
                ),
                new FormulaHelpTool.Parameter(
                    "target.var[name]", 
                    "Returns the value of a Local Name Variable attached to the targeted game object."
                )
            ));

            documentation.Add(new FormulaHelpTool(
                "RNG",
                new FormulaHelpTool.Parameter(
                    "random[min, max]", 
                    "返回包含min和Max的区间随机数值"
                ),
                new FormulaHelpTool.Parameter(
                    "dice[rolls, sides]", 
                    "扔出Rolls个sides面的骰子，计算结果并返回"
                ),
                new FormulaHelpTool.Parameter(
                    "chance[value]", 
                    "生成一个0~1的随机数，假如随机数小于等于valu，返回1，否则返回0"
                )
            ));

            documentation.Add(new FormulaHelpTool(
                "Arithmetic",
                new FormulaHelpTool.Parameter(
                    "min[a, b]", 
                    "返回a和b之间最小的数值"
                ),
                new FormulaHelpTool.Parameter(
                    "max[a, b]", 
                    "返回a和b之间最大的数值"
                ),
                new FormulaHelpTool.Parameter(
                    "round[value]", 
                    "返回value四舍五入的结果"
                ),
                new FormulaHelpTool.Parameter(
                    "floor[value]", 
                    "返回value向下取整的结果"
                ),
                new FormulaHelpTool.Parameter(
                    "ceil[value]", 
                    "返回value向上取整的结果"
                )
            ));

            documentation.Add(new FormulaHelpTool(
                "Tables",
                new FormulaHelpTool.Parameter(
                    "table.level[value]", 
                    "根据输入的数据值总和value，返回能够抵达的Level"
                ),
                new FormulaHelpTool.Parameter(
                    "table.value[level]", 
                    "返回表中抵达Level对应的数据值总和"
                ),
                new FormulaHelpTool.Parameter(
                    "table.increment[level]", 
                    "返回表中Level对应的数据值"
                ),
                new FormulaHelpTool.Parameter(
                    "table.current[value]", 
                    "Returns the value gained for the current level"
                ),
                new FormulaHelpTool.Parameter(
                    "table.next[value]", 
                    "Returns the value left to gain in order to reach the next level"
                ),
                new FormulaHelpTool.Parameter(
                    "table.ratio[value]", 
                    "Returns a unit ratio indicating the progress made at the current level"
                )
            ));

            this.m_Root.Add(documentation);
        }
    }
}