namespace PlayStats.Services
{
    public class BggGameInfo
    {
        public string Name { get; }
        public string Id { get; }

        public BggGameInfo(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}