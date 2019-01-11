using System;
using System.Collections.Generic;
using System.Text;
using Cinema.entities;

namespace Cinema
{
    class Menu
    {
        /// <summary>
        /// each section of the menu
        /// </summary>
        public List<MenuItem> AllMenu { get; set; }
        /// <summary>
        /// header text of the current menu
        /// </summary>
        public string ItemsTitle { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public Menu()
        {
            AllMenu = new List<MenuItem>();
            ItemsTitle = ".";
        }
        /// <summary>
        /// gets list of menutems and name of current menu
        /// </summary>
        /// <param name="menuitems"> list of menu items in current menu</param>
        /// <param name="name"> item title </param>
        public Menu(List<MenuItem> menuitems, string name)
        {
            AllMenu = menuitems;
            ItemsTitle = name;
        }
        /// <summary>
        /// start the the working process in this menu
        /// </summary>
        public void Run()
        {
            int maxlen = 0;
            StringBuilder tmp = new StringBuilder();
            foreach (var one in AllMenu)
            {
                if (one.Text.Length > maxlen) maxlen = one.Text.Length;
            }

            foreach (var one in AllMenu)
            {
                if (one.Text.Length < maxlen && !(one is MenuSeparator))
                {
                    for (int i = 0; i < maxlen - one.Text.Length; i++) tmp.Insert(0, " ");

                    one.Text = one.Text + tmp.ToString();
                    tmp.Clear();
                }
            }
            
            int y = 0;
            var key = ConsoleKey.A;
            int cursor = 0;

            foreach (MenuItem one in AllMenu)
            {
                if (one.IsSelected)
                {
                    cursor = AllMenu.IndexOf(one);
                    break;
                }
            }
            if (cursor == 0) AllMenu[0].IsSelected = true;


            while (true)
            {
                Console.Clear();
                Console.WriteLine($"\n  {ItemsTitle}\n");

                foreach (MenuItem one in AllMenu)
                {
                    if (one.IsSelected) Console.BackgroundColor = ConsoleColor.DarkBlue;
                    one.Draw(y += 3);
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                y = 0;
                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        if (cursor - 1 >= 0)
                        {
                            cursor -= 1;
                            foreach (MenuItem v in AllMenu) v.IsSelected = false;
                            AllMenu[cursor].IsSelected = true;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (cursor + 1 < AllMenu.Count)
                        {
                            cursor += 1;
                            foreach (MenuItem v in AllMenu) v.IsSelected = false;
                            AllMenu[cursor].IsSelected = true;
                        }
                        break;

                    case ConsoleKey.Enter:

                        if (cursor == AllMenu.Count - 1) AllMenu[cursor].IsSelected = false;

                        foreach (MenuItem v in AllMenu) v.IsSelected = false;

                        if (AllMenu[cursor].action != null)
                        {
                            AllMenu[cursor].action.Invoke();
                            cursor = 0; AllMenu[cursor].IsSelected = true;
                        }
                        else AllMenu[cursor].IsSelected = true;

                        break;

                }



            }
        }


    }
}
