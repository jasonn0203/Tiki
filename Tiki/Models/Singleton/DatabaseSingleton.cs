namespace Tiki.Models.Singleton
{
    public sealed class DatabaseSingleton
    {
        private static TikiEntities instance;

        private DatabaseSingleton() { }

        // Điều này giúp đảm bảo rằng chỉ có thể tạo ra một instance của lớp thông qua thuộc tính Instance
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