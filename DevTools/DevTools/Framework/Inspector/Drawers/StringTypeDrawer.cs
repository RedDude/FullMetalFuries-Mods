using System;
using System.Text;
using ImGuiNET;

namespace DevMenu.Drawers
{
    public class StringTypeDrawer : IInspectorDrawer
    {
        public bool HandlesType(Type type, object getValue, object value, object target)
        {
            return type == typeof(string);
        }

        public void Draw(Type memberType, string memberName, object value, object target, Action<object, object> setValue)
        {
            ImGui.TextColored(new ImVec4(10, 10, 10, 0), memberName + ": " + value);
            // byte[] bytes = Encoding.ASCII.GetBytes(value.ToString());
            // ImGui.InputText(memberName, bytes, uint.MaxValue);
        }
    }
}
