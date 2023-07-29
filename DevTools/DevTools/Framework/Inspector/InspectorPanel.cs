using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CDGEngine;
using DevMenu.Drawers;
using DevMenu.InspectorModes;
using HarmonyLib;
using ImGuiNET;

namespace DevMenu
{
    public class InspectorPanel
    {
        private object target;
        public static IList<IInspectorDrawer> AllDrawers;
        public static DefaultDrawer DefaultDrawer;


        private FieldsPanel _fieldsPanel = new FieldsPanel();
        private PropertiesPanel _propertiesPanel = new PropertiesPanel();
        private MethodPanel _methodPanel = new MethodPanel();

        public string[] inspectModeOptions = new[] {"All", "Fields", "Properties", "Methods"};
        private int inspectorModeSelected = 1;
        private FieldInfo[] targetFields;
        private PropertyInfo[] targetProperties;
        private MethodInfo[] targetMethods;
        private string searchTerm = "";

        public InspectorPanel()
        {
            if (AllDrawers != null) return;
            AllDrawers = ReflectionUtil.GetAllImplementingInstancesOfInterface<IInspectorDrawer>().ToList();
            var dd = AllDrawers.First(d => d.GetType() == typeof(DefaultDrawer));
            DefaultDrawer = (DefaultDrawer) dd;
            AllDrawers.Remove(dd);
        }

        public void SetInspectedObject(object selection)
        {
            if(this.target == selection)
                return;

            this.target = selection;
            this.targetFields = this.GetFields();
            this.targetProperties = this.GetProperties();
            this.targetMethods = this.GetMethods();
        }

        private void PopUp()
        {
            //if (ImGui.Button(this.Selected == -1 ? "<None>" : this.options[this.Selected]))
            if (ImGui.Button(this.inspectModeOptions[this.inspectorModeSelected]))
                ImGui.OpenPopup("inspect_mode");

            if (ImGui.BeginPopup("inspect_mode"))
            {
                for (int i = 0; i < this.inspectModeOptions.Length; i++)
                    if (ImGui.Selectable(this.inspectModeOptions[i]))
                    {
                        this.inspectorModeSelected = i;
                    }

                ImGui.EndPopup();
            }
        }

        public void Draw()
        {
            this.PopUp();
            byte[] searchTermBytes = new byte[50];

            // TODO: Figure how to handle the empty space form the utfString
            // ImGui.InputText("", searchTermBytes, 50);
            // string utfString = Encoding.UTF8.GetString(searchTermBytes, 0, searchTermBytes.Length);
            // this.searchTerm = utfString.TrimEnd('\0');

            string currentSearchTerm = this.searchTerm;
            if (this.inspectorModeSelected == 0)
            {
                this.DrawFields(this.targetFields, currentSearchTerm);
                this.DrawProperties(this.targetProperties, currentSearchTerm);
                this.DrawMethods(this.targetMethods, currentSearchTerm);
            }

            if (this.inspectorModeSelected == 1)
                this.DrawFields(this.targetFields, currentSearchTerm);
            if (this.inspectorModeSelected == 2)
                this.DrawProperties(this.targetProperties,currentSearchTerm);
            if (this.inspectorModeSelected == 3)
                this.DrawMethods(this.targetMethods, currentSearchTerm);
        }

        private FieldInfo[] GetFields(bool includePublic = true, bool includePrivate = true)
        {
            var fields = !includePublic ? new FieldInfo[0] : this.target.GetType().GetFields();

            var fieldsPrivate = !includePrivate ? new FieldInfo[0] : this.target.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldsPrivate.AddRangeToArray(fields);
        }

        private PropertyInfo[] GetProperties(bool includePublic = true, bool includePrivate = true)
        {
            var props = !includePublic ? new PropertyInfo[0] : this.target.GetType().GetProperties();
            var propsPrivate = !includePrivate
                ? new PropertyInfo[0]
                : this.target.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
            return propsPrivate.AddRangeToArray(props);
        }

        private MethodInfo[] GetMethods(bool includePublic = true, bool includePrivate = true)
        {
            var methods = !includePublic ? new MethodInfo[0] : this.target.GetType().GetMethods();
            var methodsPrivate =  !includePrivate ? new MethodInfo[0] : this.target.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            return methodsPrivate.AddRangeToArray(methods);
        }

        private void DrawFields(FieldInfo[] fields, string searchTerm = null)
        {
            try
            {
                this._fieldsPanel.DrawFields(this.target, fields, searchTerm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DrawProperties(PropertyInfo[] properties, string searchTerm = null)
        {
            try
            {
                this._propertiesPanel.DrawProperties(this.target, properties, searchTerm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DrawMethods(MethodInfo[] methods, string searchTerm = null)
        {
            try
            {
                this._methodPanel.DrawMethods(this.target.GetType(), this.target, searchTerm, methods);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static bool SearchMatches(string searchTerm, string candidateName)
        {
            if (string.IsNullOrEmpty(searchTerm)) return true;
            if (string.IsNullOrWhiteSpace(searchTerm)) return true;
            if (searchTerm.Length == 0) return true;

            string[] split = searchTerm.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);

            foreach (string searchTermPart in split)
            {
                if (!candidateName.Contains(searchTermPart)) //, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            return true;
        }
    }

}
