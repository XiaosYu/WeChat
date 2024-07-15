namespace WeChat.WebApi.Servers
{
    public class DBContextFactory
    {
        private ConnectionSetting Setting = new ConnectionSetting();
        public DBContextFactory(Config.Config config) => Setting = config.GetSetting<ConnectionSetting>("connection.json");
        public T Create<T>()where T:DbContext
        {
            //获取option
            DbContextOptionsBuilder<T> builder = new DbContextOptionsBuilder<T>();

            //builder.UseSqlServer(Setting[typeof(T).Name.Replace("Context", "")]);
            builder.UseSqlite(Setting[typeof(T).Name.Replace("Context", "")]);
            return (T)Activator.CreateInstance(typeof(T),builder.Options);
        }
    }
}
