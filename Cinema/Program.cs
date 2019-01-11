using System;

namespace Cinema
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;

            Manager M = new Manager();
            M.Start();
        }
    }
}


