namespace SQLite_DB_LIB
{
    public static class ExtensionMethods
    {

        public static bool IsInt(this object value)
        {
            return value is short || value is ushort || value is int
                || value is uint || value is long || value is ulong;
        }
    }
}
