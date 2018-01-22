

public class ObservedValue<T>
{
    public delegate void OnModifiedDelegate(T oldValue, T newValue);

    public OnModifiedDelegate OnChanged;
    private T _value;

    public T Value
    {
        get { return _value; }
        set
        {
            T oldValue = _value;
            _value = value;
            if (OnChanged != null) OnChanged(oldValue, _value);
        }
    }

    public ObservedValue(T initialValue, OnModifiedDelegate onChanged = null, bool callOnChangedOnInit = true)
    {
        _value = initialValue;
        if (onChanged != null)
        {
            OnChanged += onChanged;
            if (callOnChangedOnInit) OnChanged(_value, _value);
        }
    }
}