using System.Collections.Generic;
using Cinema.entities;

namespace Cinema
{
    /// <summary>
    /// Data which contains collections of Hals, Movies and Sessions
    /// </summary>
    public class DataBase
    {/// <summary>
     /// all halls
     /// </summary>
        public List<Hall> Halls { get; set; }
        /// <summary>
        /// all movies
        /// </summary>
        public List<Film> Movies { get; set; }
        /// <summary>
        /// all sesions
        /// </summary>
        public List<Session> LibraryOfSessions { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public DataBase()
        {
            Halls = new List<Hall>();
            Movies = new List<Film>();
            LibraryOfSessions = new List<Session>();
        }
    }
}

