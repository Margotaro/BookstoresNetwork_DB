namespace BookStore
{
    public sealed class Id<T>
    {
        public Id(string value) => this.huelue = value;
        public string huelue { get; }
        public override string ToString()
        {
            return huelue;
        }

        public override bool Equals(object obj)
        {
            if (obj is Id<T>)
            {
                var another = obj as Id<T>;
                return huelue == another.huelue;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return huelue.GetHashCode();
        }
    }
}
