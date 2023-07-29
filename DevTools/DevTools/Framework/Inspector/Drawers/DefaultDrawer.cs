using System;
using ImGuiNET;

namespace DevMenu.Drawers
{
    public class DefaultDrawer : IInspectorDrawer
    {
        public bool HandlesType(Type type, object getValue, object value, object target)
        {
            return true;
        }

        public void Draw(Type memberType, string memberName, object value, object target, Action<object, object> setValue)
        {
            ImGui.Text(memberName + ": " + value);
        }
    }
}
