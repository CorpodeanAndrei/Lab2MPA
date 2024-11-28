﻿namespace Lab2MPA.Models
{
    public class PublishedBook
    {
        public int ID { get; set; }
        public int PublisherID { get; set; }
        public int BookID { get; set; }
        public Publisher Publisher { get; set; }
        public Book Book { get; set; }
    }
}