using System;

namespace DevMenu.Drawers
{
    public interface IInspectorDrawer
    {
        bool HandlesType(Type type, object value, object o, object target);

        void Draw(Type memberType, string memberName, object value, object target,
            Action<object, object> setValue);
    }
}
