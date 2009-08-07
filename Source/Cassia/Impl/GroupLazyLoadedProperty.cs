using System;

namespace Cassia.Impl
{
    public delegate void GroupPropertyLoader();

    public class GroupLazyLoadedProperty<T>
    {
        private readonly GroupPropertyLoader _loader;
        private bool _loaded;
        private T _value;

        public GroupLazyLoadedProperty(GroupPropertyLoader loader)
        {
            _loader = loader;
        }

        public T Value
        {
            get
            {
                if (!_loaded)
                {
                    _loader();
                    if (!_loaded)
                    {
                        throw new InvalidOperationException("Property value should have been set by now");
                    }
                }
                return _value;
            }
            set
            {
                _value = value;
                _loaded = true;
            }
        }
    }
}