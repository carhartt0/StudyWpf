namespace WpfNaverMovieFinder.models
{
    internal class MovieItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string SubTitle { get; set; }
        public string Pubdate { get; set; }
        public string Director { get; set; }
        public string Actor { get; set; }
        public string UserRating { get; set; }

        public MovieItem(string title, string link, string image, string subTitle,
                         string pubdate, string director, string actor, string userRating)
        {
            Title = title;
            Link = link;
            Image = image;
            SubTitle = subTitle;
            Pubdate = pubdate;
            Director = director;
            Actor = actor;
            UserRating = userRating;
        }
    }
}
