namespace NHibernate.QueryHelpers.Transformers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Maps column names to property names based on the ColumnAttribute attached to properties or fields
    /// within the class.
    /// </summary>
    /// <typeparam name="TResultType">The type to map to.</typeparam>
    public class ColumnAttributeResultTransformer<TResultType> : AliasMappingResultTransformer<TResultType>
    {
        private readonly Dictionary<string, MemberInfo> memberColumnMap;

        public ColumnAttributeResultTransformer()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            this.memberColumnMap = new Dictionary<string, MemberInfo>();

            MemberInfo[] members =
                this.ResultType.GetProperties(flags)
                    .Cast<MemberInfo>()
                    .Concat(this.ResultType.GetFields(flags))
                    .ToArray();

            foreach (MemberInfo member in members)
            {
                var attr = member.GetCustomAttribute<ColumnAttribute>();

                this.memberColumnMap.Add(attr == null ? member.Name : attr.ColumnName, member);
            }
        }

        protected override IDictionary<string, MemberInfo> AliasMemberMapping
        {
            get { return this.memberColumnMap; }
        }
    }
}
