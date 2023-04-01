namespace LRBee.Utilities
{
    public class UnsupportedEnumValueException<TEnum> : Exception
        where TEnum : Enum
    {
        public UnsupportedEnumValueException(TEnum @enum)
            : base($"Unsupported enum {typeof(TEnum).Name} value: {@enum}.")
        {
        }
    }
}