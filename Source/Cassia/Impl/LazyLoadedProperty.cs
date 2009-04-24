namespace Cassia.Impl
{
    public delegate T PropertyLoader<T>();

    /// <summary>
    /// A property that is evaluated lazily.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public class LazyLoadedProperty<T>
    {
        private readonly PropertyLoader<T> _loader;
        private bool _loaded;
        private T _value;

        public LazyLoadedProperty(PropertyLoader<T> loader)
        {
            _loader = loader;
        }

        public T Value
        {
            get
            {
                if (!_loaded)
                {
                    _value = _loader();
                    _loaded = true;
                }
                return _value;
            }
        }
    }
}