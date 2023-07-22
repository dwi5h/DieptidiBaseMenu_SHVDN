using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.UI;
using NativeUI;
using System.Reflection;

namespace DieptidiBaseMenu_SHVDN
{
    public class MainMenu : Script
    {
        MenuPool menuPool;
        UIMenu uIBaseMenu;
        List<UIMenuItem> menuItems;
        List<Action<UIMenuItem, UIMenu>> onItemSelects;
        List<Action> aborteds, ticks;

        public MainMenu()
        {
            menuItems = new List<UIMenuItem>();
            aborteds = new List<Action>();
            ticks = new List<Action>();
            onItemSelects = new List<Action<UIMenuItem, UIMenu>>();

            getAssembly();
            buildMenu();

            uIBaseMenu.OnItemSelect += UIBaseMenu_OnItemSelect;

            menuPool = new MenuPool();
            menuPool.Add(uIBaseMenu);

            Tick += MainMenu_Tick;
            KeyUp += MainMenu_KeyUp;
            Aborted += MainMenu_Aborted;
        }

        private void UIBaseMenu_OnItemSelect(UIMenu sender, UIMenuItem selectedItem, int index)
        {
            foreach (var action in onItemSelects)
            {
                action(selectedItem, uIBaseMenu);
            }
        }
        private void MainMenu_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                uIBaseMenu.Visible = true;
            }
        }
        private void MainMenu_Tick(object sender, EventArgs e)
        {
            menuPool.ProcessMenus();

            foreach (var tick in ticks)
            {
                tick();
            }
        }
        private void MainMenu_Aborted(object sender, EventArgs e)
        {
            foreach (var aborted in aborteds)
            {
                aborted();
            }
        }

        void getAssembly()
        {
            string path = Directory.GetCurrentDirectory() + @"\scripts";
            string[] assemblyPaths = Directory.GetFiles(path, "*.dll");
            try
            {
                foreach (string assemblyPath in assemblyPaths)
                {
                    Assembly assembly = null;
                    assembly = Assembly.LoadFrom(assemblyPath);

                    var subclassTypes = assembly.GetExportedTypes().Where(t => t.IsSubclassOf(typeof(DieptidiBaseMenu)));

                    foreach (var item in subclassTypes)
                    {
                        object child = (DieptidiBaseMenu)Activator.CreateInstance(item);
                        object menuItemsCh = item.GetProperty("MenuItems").GetValue(child, null);
                        object onClickCh = item.GetProperty("OnItemSelect").GetValue(child, null);
                        object abortedCh = item.GetProperty("Aborted").GetValue(child, null);
                        object tickCh = item.GetProperty("Tick").GetValue(child, null);
                        var menuItemsChCast = (List<UIMenuItem>)menuItemsCh;
                        var onClickChCast = (Action<UIMenuItem, UIMenu>)onClickCh;
                        var abortedChCast = (Action)abortedCh;
                        var tickChCast = (Action)tickCh;

                        if (menuItemsChCast != null)
                            menuItems.AddRange(menuItemsChCast);

                        if (onClickChCast != null)
                            onItemSelects.Add(onClickChCast);

                        if (abortedChCast != null)
                            aborteds.Add(abortedChCast);

                        if (tickChCast != null)
                            ticks.Add(tickChCast);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void buildMenu()
        {
            uIBaseMenu = new UIMenu("Dieptidi Menu", "");
            uIBaseMenu.MouseControlsEnabled = true;

            foreach (var item in menuItems)
            {
                uIBaseMenu.AddItem(item);
            }
        }
    }
}
