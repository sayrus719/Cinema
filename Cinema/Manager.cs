using System;
using System.Collections.Generic;
using System.Linq;
using Cinema.entities;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace Cinema
{

    enum MyEnum
    {
        Show = 0,
        Add = 1,
        Edit = 2,
        Remove = 3,
        Moviestate = 4
    }
    /// <summary>
    /// type which includes bought ticket info
    /// </summary>
    [Serializable]
    public class Ticket_Print
    {
        /// <summary>
        /// current hall
        /// </summary>
        public Hall Hall { get; set; }
        /// <summary>
        /// current film
        /// </summary>
        public Film Film { get; set; }
        /// <summary>
        /// current time
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// current ticket
        /// </summary>
        public Ticket Ticket { get; set; }
    }
    /// <summary>
    /// type which includes statistics by Date
    /// </summary>
    [Serializable]
    public class ByDate_Print
    {
        /// <summary>
        /// session 
        /// </summary>
        public Session Session { get; set; }
        /// <summary>
        /// how many sold
        /// </summary>
        public int Sold { get; set; }
    }

    /// <summary>
    ///type which includes statistics by Film
    /// </summary>
    [Serializable]
    public class ByFilm_Print
    {/// <summary>
     /// film
     /// </summary>
        public Film Film { get; set; }
        /// <summary>
        /// general information of statistics
        /// </summary>
        public double General { get; set; }
        /// <summary>
        /// list of each session
        /// </summary>
        public List<string> Each { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public ByFilm_Print()
        {
            Each = new List<string>();
        }
    }
    /// <summary>
    /// main class of this program which works with all elements
    /// </summary>
    class Manager : IWriter
    {
        private DataBase DataBase { get; set; }
        /// <summary>
        /// current menu where are you
        /// </summary>
        public Menu CurrentMenu { get; set; }
        /// <summary>
        /// history of your entering to the menu
        /// </summary>
        public Stack<Menu> History { get; set; }

        private Menu MainMenu { get; set; }
        private Menu AllHalls { get; set; }
        private Menu HallMenu { get; set; }
        private Menu RemoveMenu { get; set; }
        private Menu EditHallMenu { get; set; }
        private MenuItem Back { get; set; }

        private Menu MovieMenu { get; set; }
        private Menu AllMovies { get; set; }
        private Menu EditMovieMenu { get; set; }
        private Menu RemoveMovieMenu { get; set; }
        private Menu MoviestateMenu { get; set; }

        private Menu SessionMenu { get; set; }
        private Menu ShowAllSessions { get; set; }
        private Menu RemoveSessionMenu { get; set; }

        private Menu TicketMenu { get; set; }
        private Menu SaleTicket { get; set; }

        private Menu ChooseSession { get; set; }

        private Menu StatisticsMenu { get; set; }
        private Menu BySession { get; set; }
        private Menu ByFilm { get; set; }

        private readonly MenuSeparator past;
        private readonly MenuSeparator current;
        private readonly MenuSeparator future;

        /// <summary>
        /// logical constructor where all objects are creating
        /// </summary>
        /// <param name="First"></param>
        public Manager(bool First)
        {
            DataBase = new DataBase();
            DeSerializeToXml("database.xml");

            History = new Stack<Menu>();
            Back = new MenuItem("Back          ") { action = () => { CurrentMenu = History.Pop(); CurrentMenu.Run(); } };

            past = new MenuSeparator("\n*** PAST ***\n");
            current = new MenuSeparator("\n*** CURRENT ***\n");
            future = new MenuSeparator("\n*** FUTURE ***\n");

            //Main menu
            {
                MainMenu = new Menu { ItemsTitle = "MAIN MENU" };
                MainMenu.AllMenu.Add(new MenuItem("Hall menu"));
                MainMenu.AllMenu.Add(new MenuItem("Movie menu"));
                MainMenu.AllMenu.Add(new MenuItem("Session menu"));
                MainMenu.AllMenu.Add(new MenuItem("Ticket menu"));
                MainMenu.AllMenu.Add(new MenuItem("Statistics menu"));
                MainMenu.AllMenu.Add(new MenuItem("Exit"));
            }

            //Hall Menu
            {
                HallMenu = new Menu { ItemsTitle = "HALL MENU" };
                HallMenu.AllMenu.Add(new MenuItem("Show all Halls"));
                HallMenu.AllMenu.Add(new MenuItem("Add hall"));
                HallMenu.AllMenu.Add(new MenuItem("Edit hall"));
                HallMenu.AllMenu.Add(new MenuItem("Remove Hall"));
                HallMenu.AllMenu.Add(Back);
            }

            //hall addings
            {
                AllHalls = new Menu { ItemsTitle = "ALL Halls" };
                foreach (Hall one in DataBase.Halls) AllHalls.AllMenu.Add(new MenuItem(one.ToString()));
                AllHalls.AllMenu.Add(Back);

                EditHallMenu = new Menu { ItemsTitle = "CHOOSE HALL FOR EDIT" };
                EditHallMenu.AllMenu = AllHalls.AllMenu;

                RemoveMenu = new Menu { ItemsTitle = "CHOOSE HALL FOR REMOVE" };
                RemoveMenu.AllMenu = AllHalls.AllMenu;
            }

            //Movie Menu
            {
                MovieMenu = new Menu { ItemsTitle = "MOVIE MENU" };
                MovieMenu.AllMenu.Add(new MenuItem("Show all Movies"));
                MovieMenu.AllMenu.Add(new MenuItem("Add movie"));
                MovieMenu.AllMenu.Add(new MenuItem("Edit movie"));
                MovieMenu.AllMenu.Add(new MenuItem("Remove movie"));
                MovieMenu.AllMenu.Add(new MenuItem("Change movie state"));
                MovieMenu.AllMenu.Add(Back);
            }

            //movie addings
            {
                AllMovies = new Menu { ItemsTitle = "ALL DataBase.Movies" };
                MiniSort();

                EditMovieMenu = new Menu { ItemsTitle = "CHOOSE MOVIE FOR EDIT" };
                EditMovieMenu.AllMenu = AllMovies.AllMenu;

                RemoveMovieMenu = new Menu { ItemsTitle = "CHOOSE MOVIE FOR REMOVE" };
                RemoveMovieMenu.AllMenu = AllMovies.AllMenu;

                MoviestateMenu = new Menu { ItemsTitle = "CHOOSE MOVIE FOR STATE CHANGE" };
                MoviestateMenu.AllMenu = AllMovies.AllMenu;
            }

            //Session Menu
            {
                SessionMenu = new Menu { ItemsTitle = "SESSION CONSTRUCTOR" };
                SessionMenu.AllMenu.Add(new MenuItem("Show all sessions"));
                SessionMenu.AllMenu.Add(new MenuItem("Create session"));
                SessionMenu.AllMenu.Add(new MenuItem("Delete session"));
                SessionMenu.AllMenu.Add(Back);
            }

            //Session addings
            {
                DataBase.LibraryOfSessions.OrderByDescending(x => x.Time);
                ShowAllSessions = new Menu { ItemsTitle = "All Sessions" };
                foreach (var one in DataBase.LibraryOfSessions)
                    ShowAllSessions.AllMenu.Add(new MenuItem(one.ToString()));

                ShowAllSessions.AllMenu.Add(Back);

                RemoveSessionMenu = new Menu { ItemsTitle = "Remove Session" };
                RemoveSessionMenu.AllMenu = ShowAllSessions.AllMenu;
            }

            //Ticket Menu
            {
                TicketMenu = new Menu { ItemsTitle = "TICKET MENU" };
                TicketMenu.AllMenu.Add(new MenuItem("Sale ticket"));
                TicketMenu.AllMenu.Add(Back);
            }

            //Ticket addings
            {
                SaleTicket = new Menu { ItemsTitle = "CHOOSE FILM" };
                DataBase.Movies.Where(x => x.Condition == 1).ToList().ForEach(x => SaleTicket.AllMenu.Add(new MenuItem(x.ToString())));
                SaleTicket.AllMenu.Add(Back);
            }

            ChooseSession = new Menu { ItemsTitle = "CHOOSE SESSION" };

            //Statistics Menu
            {
                StatisticsMenu = new Menu { ItemsTitle = "STATISTICS MENU" };
                StatisticsMenu.AllMenu.Add(new MenuItem("Statistic by session"));
                StatisticsMenu.AllMenu.Add(new MenuItem("Statistic by Film"));
                StatisticsMenu.AllMenu.Add(new MenuItem("Statistic by Date"));
                StatisticsMenu.AllMenu.Add(Back);
            }

            //Statistics addings
            {
                BySession = new Menu { ItemsTitle = "CHOOSE SESSION" };
                foreach (var one in DataBase.LibraryOfSessions)
                    BySession.AllMenu.Add(new MenuItem(one.ToString()));
                BySession.AllMenu.Add(Back);

                ByFilm = new Menu { ItemsTitle = "CHOOSE MOVIE" };
                foreach (Film one in DataBase.Movies)
                    ByFilm.AllMenu.Add(new MenuItem(one.ToString()));
                ByFilm.AllMenu.Add(Back);
            }

        }


        /// <summary>
        /// main constructor wwhich calls logical constructor first and then actions and other processes are going here
        /// </summary>
        public Manager() : this(true)
        {
            CurrentMenu = MainMenu;

            MainMenu.AllMenu[0].action = () => { History.Push(CurrentMenu); CurrentMenu = HallMenu; CurrentMenu.Run(); };
            MainMenu.AllMenu[1].action = () => { History.Push(CurrentMenu); CurrentMenu = MovieMenu; CurrentMenu.Run(); };
            MainMenu.AllMenu[2].action = () => { History.Push(CurrentMenu); CurrentMenu = SessionMenu; CurrentMenu.Run(); };
            MainMenu.AllMenu[3].action = () => { History.Push(CurrentMenu); CurrentMenu = TicketMenu; CurrentMenu.Run(); };
            MainMenu.AllMenu[4].action = () => { History.Push(CurrentMenu); CurrentMenu = StatisticsMenu; CurrentMenu.Run(); };
            MainMenu.AllMenu[5].action = () => { SerializeToXml("database.xml"); Environment.Exit(0); };

            TicketMenu.AllMenu[0].action = Sale_Ticket_func;

            HallMenu.AllMenu[(int)MyEnum.Show].action = () =>
            {
                for (int i = 0; i < AllHalls.AllMenu.Count - 1; i++)
                    AllHalls.AllMenu[i].action = null;

                History.Push(CurrentMenu);
                CurrentMenu = AllHalls;
                CurrentMenu.Run();
            };
            HallMenu.AllMenu[(int)MyEnum.Add].action = AddHall;
            HallMenu.AllMenu[(int)MyEnum.Edit].action = () =>
            {
                for (int i = 0; i < EditHallMenu.AllMenu.Count - 1; i++)
                {
                    MenuItem tmp_edit = EditHallMenu.AllMenu[i];
                    Hall tmp_hall = DataBase.Halls[i];
                    EditHallMenu.AllMenu[i].action = () => EditHall(tmp_edit, tmp_hall);
                }
                History.Push(CurrentMenu); CurrentMenu = EditHallMenu; CurrentMenu.Run();
            };
            HallMenu.AllMenu[(int)MyEnum.Remove].action = () =>
            {
                for (int i = 0; i < RemoveMenu.AllMenu.Count - 1; ++i)
                {
                    MenuItem tmp_all = RemoveMenu.AllMenu[i];
                    Hall tmp_hall = DataBase.Halls[i];
                    RemoveMenu.AllMenu[i].action = () => RemoveHall(tmp_all, tmp_hall);
                }
                History.Push(CurrentMenu);
                CurrentMenu = RemoveMenu;
                CurrentMenu.Run();
            };


            MovieMenu.AllMenu[(int)MyEnum.Show].action = () =>
             {
                 for (int i = 0; i < AllMovies.AllMenu.Count - 1; i++)
                     AllMovies.AllMenu[i].action = null;

                 History.Push(CurrentMenu);
                 CurrentMenu = AllMovies;
                 CurrentMenu.Run();
             };
            MovieMenu.AllMenu[(int)MyEnum.Add].action = AddMovie;
            MovieMenu.AllMenu[(int)MyEnum.Edit].action = () =>
            {
                List<MenuItem> sorted = EditMovieMenu.AllMenu.Where(x => !(x is MenuSeparator)).ToList();

                for (int i = 0; i < sorted.Count - 1; i++)
                {
                    MenuItem tmp_edit = sorted[i];
                    Film tmp_film = DataBase.Movies[i];
                    sorted[i].action = () => EditMovie(tmp_edit, tmp_film);
                }
                History.Push(CurrentMenu); CurrentMenu = EditMovieMenu; CurrentMenu.Run();
            };
            MovieMenu.AllMenu[(int)MyEnum.Remove].action = () =>
             {
                 List<MenuItem> sorted = RemoveMovieMenu.AllMenu.Where(x => !(x is MenuSeparator)).ToList();

                 for (int i = 0; i < sorted.Count - 1; ++i)
                 {
                     MenuItem tmp = sorted[i];
                     Film tmp_film = DataBase.Movies[i];
                     sorted[i].action = () => RemoveMovie(tmp, tmp_film);
                 }
                 History.Push(CurrentMenu); CurrentMenu = RemoveMovieMenu; CurrentMenu.Run();
             };
            MovieMenu.AllMenu[(int)MyEnum.Moviestate].action = () =>
            {
                List<MenuItem> sorted = MoviestateMenu.AllMenu.Where(x => !(x is MenuSeparator)).ToList();
                sorted = sorted.Take(sorted.Count - 1).ToList();

                for (int i = 0; i < sorted.Count; ++i)
                {
                    MenuItem tmp = sorted[i];
                    Film tmp_film = DataBase.Movies[i];
                    sorted[i].action = () => MoviestateEdit(tmp, tmp_film);
                }
                History.Push(CurrentMenu); CurrentMenu = MoviestateMenu; CurrentMenu.Run();


            };

            SessionMenu.AllMenu[(int)MyEnum.Show].action = () =>
            {
                for (int i = 0; i < ShowAllSessions.AllMenu.Count - 1; i++)
                    ShowAllSessions.AllMenu[i].action = null;

                History.Push(CurrentMenu);
                CurrentMenu = ShowAllSessions;
                CurrentMenu.Run();
            };
            SessionMenu.AllMenu[(int)MyEnum.Add].action = AddSession;
            SessionMenu.AllMenu[2].action = () =>
            {
                for (int i = 0; i < RemoveSessionMenu.AllMenu.Count - 1; ++i)
                {
                    MenuItem tmp_it = RemoveSessionMenu.AllMenu[i];
                    Session tmp_ses = DataBase.LibraryOfSessions[i];
                    RemoveSessionMenu.AllMenu[i].action = () => RemoveSes(tmp_it, tmp_ses);
                }
                History.Push(CurrentMenu);
                CurrentMenu = RemoveSessionMenu;
                CurrentMenu.Run();
            };

            StatisticsMenu.AllMenu[0].action = () =>
            {
                for (int i = 0; i < BySession.AllMenu.Count - 1; i++)
                {
                    Session tmp_one = DataBase.LibraryOfSessions[i];
                    BySession.AllMenu[i].action = () => ByState_func(tmp_one);
                }

                History.Push(CurrentMenu); CurrentMenu = BySession; CurrentMenu.Run();
            };
            StatisticsMenu.AllMenu[1].action = () =>
            {
                for (int i = 0; i < ByFilm.AllMenu.Count - 1; i++)
                {
                    Film tmp_one = DataBase.Movies[i];
                    ByFilm.AllMenu[i].action = () => ByFilm_func(tmp_one);
                }

                History.Push(CurrentMenu); CurrentMenu = ByFilm; CurrentMenu.Run();
            };
            StatisticsMenu.AllMenu[2].action = ByDate;
        }


        private void ByDate()
        {
            Console.Clear();

            DateTime start = new DateTime();
            DateTime end = new DateTime();
            Console.CursorVisible = true;
            Console.WriteLine("STATISTICS BY DATE");
            Console.Write("\nEnter Start Date (e.g. 10/20/2000): ");
            start = DateTime.Parse(Console.ReadLine());
            Console.Write("Enter End Date (e.g. 10/20/2000): ");
            end = DateTime.Parse(Console.ReadLine());
            Console.CursorVisible = false;

            List<Session> sorted = DataBase.LibraryOfSessions.Where(x => (x.Time >= start && x.Time <= end)).ToList();

            Console.WriteLine("\n Statistics from " + start.ToShortDateString() + " to " + end.ToShortDateString() + ":");
            Console.WriteLine();
            foreach (var one in sorted)
            {
                Console.WriteLine("* " + one.ToString());
            }

            Console.WriteLine("\nPress any key for Back");
            Console.ReadKey();

            DialogResult res = MessageBox.Show($"Save to file?", "Save", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string path = $@"..\..\Statistics by date from {start.Day}.{start.Month}.{start.Year} to {end.Day}.{end.Month}.{end.Year}.xml";
                XmlSerializer xs = new XmlSerializer(typeof(List<Session>));

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    xs.Serialize(fs, sorted);
                }
                MessageBox.Show("Saved");
            }
        }

        private void ByFilm_func(Film current)
        {
            ByFilm_Print ByFilm = new ByFilm_Print();

            List<Session> filter = DataBase.LibraryOfSessions.Where(x => x.Film == current).ToList();
            List<int> results = new List<int>();
            foreach (var one in filter)
            {
                results.Add(one.Sold.Count * 100 / (one.Hall.Rows * one.Hall.Seats));
            }

            Console.Clear();
            Console.WriteLine("\nMovie: " + current.ToString());
            Console.WriteLine("\nGeneral procertage of seats sold: " + results.Average().ToString() + " %");
            Console.WriteLine("\nDetails Info:");
            ByFilm.General = results.Average();
            ByFilm.Film = current;

            for (int i = 0; i < filter.Count; i++)
            {
                string abu = filter[i].ToString() + " -- " + results[i].ToString() + " %";
                ByFilm.Each.Add(abu);
                Console.WriteLine(abu);
            }

            Console.WriteLine("\nPress any key for Back");
            Console.ReadKey();

            DialogResult res = MessageBox.Show($"Save to file?", "Save", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string path = $@"..\..\Statistics by film {current.Name}.xml";
                XmlSerializer xs = new XmlSerializer(typeof(ByFilm_Print));

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    xs.Serialize(fs, ByFilm);
                }
                MessageBox.Show("Printed");
            }
        }

        private void ByState_func(Session current)
        {
            ByDate_Print tmp = new ByDate_Print();
            tmp.Session = current;
            tmp.Sold = current.Sold.Count * 100 / (current.Hall.Rows * current.Hall.Seats);

            Console.Clear();
            Console.WriteLine("SESSION: " + current.ToString());
            Console.WriteLine($"\nPercentage of seats  sold {tmp.Sold} %");
            Console.WriteLine("\n--------------------");
            Console.WriteLine("\nPress any key for Back");
            Console.ReadKey();
            DialogResult res = MessageBox.Show($"Save to file?", "Save", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string path = $@"..\..\Statistics by session {current.Film.Name} {current.Hall.Title}.xml";
                XmlSerializer xs = new XmlSerializer(typeof(ByDate_Print));

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    xs.Serialize(fs, tmp);
                }
                MessageBox.Show("Printed");
            }

        }

        private void DrawEachHall(int X, int Y, char symbol)
        {
            int Height = 3;
            int Width = 4;
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

                    else if (i == 1 || i == Width - 2)
                    {
                        if (symbol == '#') Console.Write("##");
                        else if (symbol == '*') Console.Write("**");
                        else if (symbol == ' ') Console.Write("  ");
                    }

                }
            }

        }

        private void Hall_Risovashka(Session ses)
        {
            Console.Clear();
            Console.WriteLine(ses.ToString() + "\n\n");

            int x = 1, y = 3;
            char Ok = ' ';

            for (int i = 0; i < ses.Hall.Seats; i++)
            {
                Console.SetCursorPosition(x += 4, y);
                Console.Write(i + 1);
            }

            x = 0; y = 2;

            for (int i = 0; i < ses.Hall.Rows; i++)
            {
                Console.SetCursorPosition(x, y += 3);
                Console.Write(i + 1);
            }

            int my_x = 1, my_y = 1;
            var key = System.ConsoleKey.A;

            while (true)
            {
                x = 0; y = 4;

                for (int i = 0; i < ses.Hall.Rows; i++)
                {
                    for (int j = 0; j < ses.Hall.Seats; j++)
                    {
                        x += 4;
                        foreach (var one in ses.Sold)
                        {
                            if (one.Place == j + 1 && one.Row == i + 1)
                            {
                                Ok = '#'; break;
                            }
                        }
                        if (my_x == j + 1 && my_y == i + 1) Ok = '*';
                        DrawEachHall(x, y, Ok);
                        Ok = ' ';
                    }
                    x = 0; y += 3;
                }

                key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        if (my_x - 1 >= 1) --my_x;
                        break;
                    case ConsoleKey.UpArrow:
                        if (my_y - 1 >= 1) --my_y;
                        break;

                    case ConsoleKey.RightArrow:
                        if (my_x + 1 <= ses.Hall.Seats) ++my_x;
                        break;


                    case ConsoleKey.DownArrow:
                        if (my_y + 1 <= ses.Hall.Rows) ++my_y;
                        break;

                    case ConsoleKey.Enter:
                        bool chk = false;
                        foreach (var one in ses.Sold)
                        {
                            if (my_x == one.Place && my_y == one.Row)
                            {
                                chk = true; break;
                            }
                        }
                        if (!chk)
                        {
                            ses.Sold.Add(new Ticket { Row = my_y, Place = my_x });

                            DialogResult res = MessageBox.Show($"Ticket (row {my_y} seat {my_x}) Sold.\nDo you want to print it?", "Ticket", MessageBoxButtons.YesNo);
                            if (res == DialogResult.Yes)
                            {
                                Ticket_Print for_print = new Ticket_Print { Film = ses.Film, Hall = ses.Hall, Time = ses.Time, Ticket = ses.Sold.Last() };

                                string path = $@"..\..\{ses.Film.Name} Row{my_y} seat{my_x}.xml";
                                XmlSerializer xs = new XmlSerializer(typeof(Ticket_Print));

                                using (FileStream fs = new FileStream(path, FileMode.Create))
                                {
                                    xs.Serialize(fs, for_print);
                                }
                                MessageBox.Show("Printed");
                            }
                            return;
                        }
                        break;
                }

            }

        }

        private void AddSession()
        {
            Console.Clear();

            Session creation = new Session();
            for (int i = 0; i < AllMovies.AllMenu.Count - 1; i++)
            {
                MenuItem tmp_f = AllMovies.AllMenu[i];
                AllMovies.AllMenu[i].action = () =>
                {
                    foreach (Film one in DataBase.Movies)
                    {
                        if (one.ToString() == tmp_f.Text)
                        {
                            creation.Film = one;
                            break;
                        }
                    }

                    for (int j = 0; j < AllHalls.AllMenu.Count - j; j++)
                    {
                        Hall tmp_hallmain = DataBase.Halls[j];
                        AllHalls.AllMenu[j].action = () =>
                        {
                            creation.Hall = tmp_hallmain;

                            Console.Clear();
                            Console.WriteLine("Enter date and time:");

                            Console.CursorVisible = true;

                            creation.Time = DateTime.Parse(MyRegex(@"^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$", "Enter the date (e.g. 10/22/1987): "));
                            int hour = int.Parse(MyRegex(@"^[0-9]{1,2}", "\nEnter the hours: "));
                            int min = int.Parse(MyRegex(@"^[0-9]{1,2}", "Enter the minutes: "));
                            Console.CursorVisible = false;

                            TimeSpan ts = new TimeSpan(hour, min, 0);
                            creation.Time = creation.Time.Date + ts;

                            DataBase.LibraryOfSessions.Add(creation);
                            DataBase.LibraryOfSessions.OrderByDescending(x => x.Time);
                            ShowAllSessions.AllMenu.Clear();

                            foreach (var one in DataBase.LibraryOfSessions)
                            {
                                ShowAllSessions.AllMenu.Add(new MenuItem(one.ToString()));
                            }
                            ShowAllSessions.AllMenu.Add(Back);

                            CurrentMenu = History.Pop();
                            CurrentMenu = History.Pop();
                            CurrentMenu.Run();
                        };
                    }

                    History.Push(CurrentMenu);
                    CurrentMenu = AllHalls;
                    CurrentMenu.Run();

                };
            }

            History.Push(CurrentMenu);
            CurrentMenu = AllMovies;
            CurrentMenu.Run();
        }

        private void RemoveSes(MenuItem tmp_it, Session tmp_ses)
        {
            DialogResult res = MessageBox.Show($"Do you want to remove {tmp_it.Text}?", "Removing", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                DataBase.LibraryOfSessions.Remove(tmp_ses);
                RemoveSessionMenu.AllMenu.Remove(tmp_it);
                MessageBox.Show("Removed successfully!");
            }
        }

        private void Sale_Ticket_func()
        {
            SaleTicket.AllMenu.Clear();
            DataBase.Movies.Where(x => x.Condition == 1).ToList().ForEach(x => SaleTicket.AllMenu.Add(new MenuItem(x.ToString())));
            SaleTicket.AllMenu.Add(Back);
            var tmp_mov = DataBase.Movies.Where(x => x.Condition == 1).ToList();

            for (int i = 0; i < SaleTicket.AllMenu.Count - 1; i++)
            {
                MenuItem tmp_it = SaleTicket.AllMenu[i];
                Film tmp_one = tmp_mov[i];

                tmp_it.action = () =>
                {
                    ChooseSession.AllMenu.Clear();
                    DataBase.LibraryOfSessions.Where(x => x.Film == tmp_one).ToList().ForEach(x => ChooseSession.AllMenu.Add(new MenuItem(x.ToString())));

                    for (int k = 0; k < ChooseSession.AllMenu.Count; k++)
                    {
                        MenuItem tmp_ses = ChooseSession.AllMenu[k];

                        tmp_ses.action = () =>
                        {
                            foreach (var one in DataBase.LibraryOfSessions)
                            {
                                if (one.ToString() == tmp_ses.Text)
                                {
                                    tmp_ses.action = () => Hall_Risovashka(one);
                                    break;
                                }
                            }

                            if (tmp_ses.action != null) tmp_ses.action.Invoke();
                        };
                    }

                    ChooseSession.AllMenu.Add(Back);
                    History.Push(CurrentMenu); CurrentMenu = ChooseSession; CurrentMenu.Run();
                };
            }
            History.Push(CurrentMenu); CurrentMenu = SaleTicket; CurrentMenu.Run();
        }

        private void MiniSort()
        {
            List<MenuItem> tmp = AllMovies.AllMenu.Where(x => !(x is MenuSeparator)).ToList();
            AllMovies.AllMenu.Clear();
            DataBase.Movies.OrderBy(x => x.Condition);
            bool check = false;

            for (int i = 0; i < 3; i++)
            {
                if (i == 0) AllMovies.AllMenu.Add(past);
                else if (i == 1) AllMovies.AllMenu.Add(current);
                else if (i == 2) AllMovies.AllMenu.Add(future);

                DataBase.Movies.Where(x => x.Condition == i).ToList().ForEach(x =>
                {
                    foreach (MenuItem o in tmp)
                    {
                        if (o.Text == x.ToString())
                        {
                            AllMovies.AllMenu.Add(new MenuItem { Text = x.ToString(), action = o.action });
                            check = true;
                            break;
                        }
                    }

                    if (check == false) AllMovies.AllMenu.Add(new MenuItem { Text = x.ToString() });

                    check = false;
                });
            }
            AllMovies.AllMenu.Add(Back);
        }

        private void AddMovie()
        {
            Film tmp_film = new Film();

            Console.Clear();
            Console.WriteLine("\n  ENTER DATA ABOUT NEW MOVIE\n");
            string tester;
            Console.CursorVisible = true;

            tmp_film.Name = MyRegex(@"^[a-z0-9]+$", "Name: ");
            tmp_film.Year = Int32.Parse(MyRegex(@"^[0-9]{4}$", "\nYear: "));
            tmp_film.Type = MyRegex(@"^[a-z0-9]+$", "\nType: ");
            tmp_film.Producer = MyRegex(@"^[a-z0-9]+$", "\nProducer: ");

            while (true)
            {
                Console.Write("\nCondition (0. PAST   1. CURRENT  2. FUTURE): ");
                tester = Console.ReadLine();
                if (Regex.IsMatch(tester, @"^[0-2]{1}$", RegexOptions.IgnoreCase))
                {
                    switch (tester)
                    {
                        case "0": tmp_film.Condition = 0; break;
                        case "1": tmp_film.Condition = 1; break;
                        case "2": tmp_film.Condition = 2; break;
                    }
                    break;
                }
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect data!");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }

            Console.CursorVisible = false;
            DataBase.Movies.Add(tmp_film);
            MiniSort();
            MessageBox.Show(" Movie added! ");
        }

        private void EditMovie(MenuItem tmp_edit, Film film)
        {
            Console.Clear();
            int prev = film.Condition;
            Console.WriteLine("\n  ENTER DATA ABOUT NEW MOVIE\n");
            Console.CursorVisible = true;

            film.Name = MyRegex(@"^[a-z0-9]+$", "Name: ");
            film.Year = Int32.Parse(MyRegex(@"^[0-9]{4}$", "\nYear: "));
            film.Type = MyRegex(@"^[a-z0-9]+$", "\nType: ");
            film.Producer = MyRegex(@"^[a-z0-9]+$", "\nProducer: ");

            Console.CursorVisible = false;
            tmp_edit.Text = film.ToString();
            if (prev != film.Condition) MiniSort();

            MessageBox.Show(" Movie added! ");
        }

        private void RemoveMovie(MenuItem tmp, Film film)
        {
            DialogResult res = MessageBox.Show($"Do you want to remove {tmp.Text}?", "Removing", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                DataBase.Movies.Remove(film);
                RemoveMovieMenu.AllMenu.Remove(tmp);
                MessageBox.Show("Removed successfully!");
            }
        }

        private void MoviestateEdit(MenuItem tmp, Film film)
        {
            //может вынести в поле?
            Menu FilmState = new Menu { ItemsTitle = $"CHOOSE STATE FOR {film.Name}" };

            FilmState.AllMenu.Add(item: new MenuItem { Text = "Past", action = () => { film.Condition = 0; MessageBox.Show("State changed! "); MiniSort(); Back.action.Invoke(); } });
            FilmState.AllMenu.Add(item: new MenuItem { Text = "Current", action = () => { film.Condition = 1; MessageBox.Show("State changed! "); MiniSort(); Back.action.Invoke(); } });
            FilmState.AllMenu.Add(item: new MenuItem { Text = "Future", action = () => { film.Condition = 2; MessageBox.Show("State changed! "); MiniSort(); Back.action.Invoke(); } });
            FilmState.AllMenu.Add(Back);

            FilmState.AllMenu[film.Condition].IsSelected = true;

            History.Push(CurrentMenu); CurrentMenu = FilmState; CurrentMenu.Run();
        }


        private string MyRegex(string pattern, string header)
        {
            string tester;
            while (true)
            {
                Console.Write(header);
                tester = Console.ReadLine();
                if (Regex.IsMatch(tester, pattern, RegexOptions.IgnoreCase))
                {
                    return tester;
                }
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect data!");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
        }
        private void AddHall()
        {
            Hall tmp_hall = new Hall();

            Console.Clear();
            Console.WriteLine("\n  ENTER DATA ABOUT NEW HALL\n");
            Console.CursorVisible = true;

            string tester;
            tmp_hall.Title = MyRegex(@"^[a-z0-9]+$", "Title: ");
            tmp_hall.Rows = Int32.Parse(MyRegex(@"^[0-9]+$", "\nRows: "));
            tmp_hall.Seats = Int32.Parse(MyRegex(@"^[0-9]+$", "\nSeats by row: "));

            while (true)
            {
                Console.Write("\nType (1. 2D   2. 3D  3. IMAX): ");
                tester = Console.ReadLine();
                if (Regex.IsMatch(tester, @"^[1-3]{1}$", RegexOptions.IgnoreCase))
                {
                    switch (tester)
                    {
                        case "1": tmp_hall.Type = "2D"; break;
                        case "2": tmp_hall.Type = "3D"; break;
                        case "3": tmp_hall.Type = "IMAX"; break;
                    }
                    break;
                }
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect data!");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }

            Console.CursorVisible = false;

            DataBase.Halls.Add(tmp_hall);
            AllHalls.AllMenu.Insert(AllHalls.AllMenu.Count - 1, new MenuItem(tmp_hall.ToString()));

            MessageBox.Show(" Hall added! ");
        }

        private void EditHall(MenuItem tmp_edit, Hall tmp_hall)
        {
            Console.Clear();
            Console.WriteLine("\n  EDITOR\n");
            Console.CursorVisible = true;

            string tester;

            tmp_hall.Title = MyRegex(@"^[a-z0-9]+$", "Title: ");
            tmp_hall.Rows = Int32.Parse(MyRegex(@"^[0-9]+$", "\nRows: "));
            tmp_hall.Seats = Int32.Parse(MyRegex(@"^[0-9]+$", "\nSeats by row: "));

            while (true)
            {
                Console.Write("\nType (1. 2D   2. 3D  3. IMAX): ");
                tester = Console.ReadLine();
                if (Regex.IsMatch(tester, @"^[1-3]{1}$", RegexOptions.IgnoreCase))
                {
                    switch (tester)
                    {
                        case "1": tmp_hall.Type = "2D"; break;
                        case "2": tmp_hall.Type = "3D"; break;
                        case "3": tmp_hall.Type = "IMAX"; break;
                    }
                    break;
                }
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Incorrect data!");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }


            tmp_edit.Text = tmp_hall.ToString();
            MessageBox.Show("Edited successfully!");
        }

        private void RemoveHall(MenuItem tmp_all, Hall tmp_hall)
        {
            DialogResult res = MessageBox.Show($"Do you want to remove {tmp_all.Text}?", "Removing", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                DataBase.Halls.Remove(tmp_hall);
                RemoveMenu.AllMenu.Remove(tmp_all);
                MessageBox.Show("Removed successfully!");
            }
        }

        /// <summary>
        /// serialization method into DataBase file XML
        /// </summary>
        /// <param name="path">path of file</param>
        public void SerializeToXml(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DataBase));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                xs.Serialize(fs, DataBase);
            }
        }
        /// <summary>
        /// deserialization method fromDataBase file XML
        /// </summary>
        /// <param name="path">path from which file shoud be taken info</param>
        public void DeSerializeToXml(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DataBase));

            using (Stream reader = new FileStream(path, FileMode.OpenOrCreate))
            {
                FileInfo sizecheck = new FileInfo(path);
                if (sizecheck.Length > 0)
                    DataBase = (DataBase)serializer.Deserialize(reader);
            }
        }
        /// <summary>
        /// start of the current menu
        /// </summary>
        public void Start()
        {
            CurrentMenu.Run();
        }
    }
}
