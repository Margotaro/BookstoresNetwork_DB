namespace BookStore
{
    sealed class Id<T>
    {
        public Id(string value) => this.huelue = value;
        public string huelue { get; }
        public static bool CheckUniqueness(Id<object> id1, Id<object> id2)
        {
            return id1.huelue == id2.huelue;
        }
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
