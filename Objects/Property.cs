using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore
{
    class Property<T>
    {

        public Property(T value, IPropertyListener listener)
        {
            this._value = value;
            this.listener = listener;
        }

        IPropertyListener listener;

        T _value;
        public T value
        {
            get => _value;
            set
            {
                _value = value;
                listener?.propertyChanged();
            }
        }

    }
}
