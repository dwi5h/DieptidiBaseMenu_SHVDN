using System;
using System.Collections.Generic;
using NativeUI;

namespace DieptidiBaseMenu_SHVDN
{
    public abstract class DieptidiBaseMenu
    {
        public List<UIMenuItem> MenuItems { get; set; }
        public abstract Action<UIMenuItem, UIMenu> OnItemSelect { get; }
        public abstract Action Aborted { get; }
        public abstract Action Tick { get; }

        public DieptidiBaseMenu()
        {
            MenuItems = new List<UIMenuItem>();
            BuildItem();
        }

        public abstract void BuildItem();
    }
}
