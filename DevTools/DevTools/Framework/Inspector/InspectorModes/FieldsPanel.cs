using System;
using System.Reflection;

namespace DevMenu.InspectorModes
{
    public class FieldsPanel
    {
        public void DrawFields(object target, FieldInfo[] fields, string searchTerm = "")
        {
            foreach (var field in fields)
            {
                // try
                // {
                    if(!InspectorPanel.SearchMatches(searchTerm, field.Name))
                        continue;

                    foreach (var inspectorDrawer in InspectorPanel.AllDrawers)
                    {
                        if (inspectorDrawer.HandlesType(field.FieldType, field.GetValue(target), target,
                            field))
                        {
                            inspectorDrawer.Draw(field.FieldType, field.Name, field.GetValue(target), target, (o, o1) => {});
                        }
                        else
                        {
                            InspectorPanel.DefaultDrawer.Draw(field.FieldType, field.Name, field.GetValue(target), target, (o, o1) => {});
                        }
                    }
                // }
                // catch (Exception e)
                // {
                //     Console.WriteLine(e);
                // }
            }
        }
    }
}
