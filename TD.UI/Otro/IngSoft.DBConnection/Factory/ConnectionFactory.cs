namespace IngSoft.DBConnection.Factory
{
    public abstract class ConnectionFactory
    {
        public static IConnection CreateSqlServerConnection()
        {
            return new SqlServerConnection();
        }
    }
}
