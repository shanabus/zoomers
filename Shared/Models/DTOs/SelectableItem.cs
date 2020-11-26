namespace ZoomersClient.Shared.Models.DTOs
{
    public class SelectableItem<T> {

        public SelectableItem(T item)
        {
            Item = item;
        }

        public T Item { get; set; }

        public bool Selected { get; set; }
    }
}
