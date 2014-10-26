namespace NHibernate.QueryHelpers.Transformers
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

        public string ColumnName { get; private set; }
    }
}
