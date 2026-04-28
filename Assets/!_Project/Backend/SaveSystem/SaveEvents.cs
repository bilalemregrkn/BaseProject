namespace Backend.Systems.Save
{
    public readonly struct DataSaved
    {
        public readonly string Key;

        public DataSaved(string key) => Key = key;
    }

    public readonly struct DataDeleted
    {
        public readonly string Key;

        public DataDeleted(string key) => Key = key;
    }

    public readonly struct AllDataDeleted { }
}
