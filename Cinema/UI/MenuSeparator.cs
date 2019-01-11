using System;
namespace Cinema.entities
{
/// <summary>
/// type of separators between menu items
/// </summary>
    public class MenuSeparator : MenuItem
    {
        /// <summary>
        /// takes text and this seperates list elements
        /// </summary>
        /// <param name="text"> name of separator which user will see</param>
        public MenuSeparator(string text)
        {
            Text = text;
        }
        /// <summary>
        /// special type of printing this element. Takes the position where to print 
        /// </summary>
        /// <param name="Y">Y coordinate</param>
        /// <param name="X">X coordinate</param>
        public override void Draw(int Y, int X = 0)
        {
            Console.SetCursorPosition(X , Y );
            Console.Write(Text);
            
        }
    }
}
