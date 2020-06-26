namespace BookStore
{
    sealed class Id<T>
    {
        public Id(string value) => this.huelue = value;
        public string huelue { get; }
    }
}
