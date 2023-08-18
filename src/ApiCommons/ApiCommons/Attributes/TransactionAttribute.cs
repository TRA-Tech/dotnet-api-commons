namespace ApiCommons.Attributes
{
    /// <summary>
    /// Represents an attribute that is used to mark methods that should be executed within a database transaction.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DbTransactionAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the DbContext that the transaction should be used with.
        /// </summary>
        public Type DbContextType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DbTransactionAttribute class with the specified DbContext type.
        /// </summary>
        /// <param name="dbContextType">The type of the DbContext that the transaction should be used with.</param>
        public DbTransactionAttribute(Type dbContextType)
        {
            DbContextType = dbContextType;
        }
    }
}
