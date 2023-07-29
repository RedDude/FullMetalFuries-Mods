using System;
using System.Reflection;

namespace DevMenu.InspectorModes
{
    public class PropertiesPanel
    {
        public void DrawProperties(object target, PropertyInfo[] pros, string searchTerm = "")
        {
            foreach (var prop in pros)
            {
                try
                {
                    if(!InspectorPanel.SearchMatches(searchTerm, prop.Name))
                        continue;

                    foreach (var inspectorDrawer in InspectorPanel.AllDrawers)
                    {
                        if (inspectorDrawer.HandlesType(prop.PropertyType, prop.GetValue(target), target,
                            prop))
                        {
                            inspectorDrawer.Draw(prop.PropertyType, prop.Name, prop.GetValue(target), target,
                                (o, o1) => { });
                        }
                        else
                        {
                            InspectorPanel.DefaultDrawer.Draw(prop.PropertyType, prop.Name, prop.GetValue(target), target,
                                (o, o1) => { });
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
