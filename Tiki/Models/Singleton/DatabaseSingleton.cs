namespace Tiki.Models.Singleton
{
    public class DatabaseSingleton
    {
        private static TikiEntities instance;

        private DatabaseSingleton() { }

        public static TikiEntities Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TikiEntities();
                }

                return instance;
            }
        }
    }
}