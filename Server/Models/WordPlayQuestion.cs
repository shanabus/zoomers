public class WordPlayQuestion
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string CategoriesString { get; set; }
        public string[] Categories { get {
            return CategoriesString.Split(",");
        }}
    }