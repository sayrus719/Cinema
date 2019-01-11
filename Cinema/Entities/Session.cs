using Cinema.entities;
using System;
using System.Collections.Generic;

namespace Cinema
{
    /// <summary>
    /// session 
    /// </summary>
    public class Session
    {
        /// <summary>
        /// hall of this session
        /// </summary>
        public Hall Hall { get; set; }
        /// <summary>
        /// film of this session
        /// </summary>
        public Film Film { get; set; }
        /// <summary>
        /// time of this session
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// list of sold tickets
        /// </summary>
        public List<Ticket> Sold { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public Session()
        {
            Hall = new Hall();
            Film = new Film();
            Time = new DateTime();
            Sold = new List<Ticket>();
        }
        /// <summary>
        /// this object to sthing overload
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Film.Name} - {Hall.Title} - {Hall.Type} - {Time}";
        }
    }
}
