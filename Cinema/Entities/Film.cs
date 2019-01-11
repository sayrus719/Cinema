namespace Cinema.entities
{
    /// <summary>
    /// Movie
    /// </summary>
    public class Film
    {
        /// <summary>
        /// name of movie
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// year of movie
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// type of movies 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// how is the producer
        /// </summary>
        public string Producer { get; set; }
        /// <summary>
        /// condition past current future
        /// </summary>
        public int Condition { get; set; }
        /// <summary>
        /// overload to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}, {Year}, {Type}, {Producer}";
        }
    }
}
