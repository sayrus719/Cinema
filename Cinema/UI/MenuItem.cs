using System;


namespace Cinema
{
    /// <summary>
    /// each element of the menu is type of this class
    /// </summary>
    public class MenuItem
    {
        /// <summary>
        /// includes reference to the function what must happen after pressing on this item
        /// </summary>
        public Action action;
        /// <summary>
        /// name of the current menu item
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// is cursor on this item  or not
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public MenuItem()
        {
            Text = "";
            action = null;
            IsSelected = false;
        }
        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="text"> name of the current menu item</param>
        public MenuItem(string text)
        {
            Text = text;
            action = null;
            IsSelected = false;
        }
        /// <summary>
        /// draws the element 
        /// </summary>
        /// <param name="Y">Oy coordinate</param>
        /// <param name="X">Ox coordinate</param>
        public virtual void Draw(int Y, int X = 0)
        {
            int Height = 3;
            int Width = Text.Length + 4;

            for (int j = 0; j < Height; ++j)
            {
                for (int i = 0; i < Width; ++i)
                {
                    Console.SetCursorPosition(X + i, Y + j);
                    if (i == 0 && j == 0) Console.Write('┌');

                    else if (i == 0 && j == Height - 1) Console.Write('└');

                    else if (i == Width - 1 && j == 0) Console.Write('┐');

                    else if (i == Width - 1 && j == Height - 1) Console.WriteLine('┘');

                    else if (i == 0 || i == Width - 1) Console.Write('│');

                    else if (j == 0 || j == Height - 1) Console.Write('─');

                    else if (i == 1 || i == Width - 2) Console.Write(' ');

                    else Console.Write(Text[i - 2]);
                }
            }

        }

    }
}


