namespace Cinema.entities
{
    /// <summary>
    /// hall of the cinema
    /// </summary>
    public class Hall
    {

        /// <summary>
        /// name of the hall
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// type of this hall
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// how many rows here
        /// </summary>
        public int Rows { get; set; }
        /// <summary>
        /// how many seats here 
        /// </summary>
        public int Seats { get; set; }
        /// <summary>
        /// overload to string of hall
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Hall: {Title} ({Type}, {Rows * Seats} seats)";
        }
    }
}
