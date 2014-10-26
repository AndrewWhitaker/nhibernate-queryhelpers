namespace NHibernate.QueryHelpers.Transformers
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Impl;

    /// <summary>
    /// Maps column names to class members using a <code>Dictionary</code>.
    /// </summary>
    /// <typeparam name="TResultType">The type to map to.</typeparam>
    public class DictionaryMappingResultTransformer<TResultType> : AliasMappingResultTransformer<TResultType>
    {
        private readonly Dictionary<string, MemberInfo> memberInfoMapping;

        public DictionaryMappingResultTransformer(IDictionary<string, Expression<Func<TResultType, object>>> mapping)
        {
            this.memberInfoMapping = new Dictionary<string, MemberInfo>();

            foreach (string key in mapping.Keys)
            {
                string memberName = ExpressionProcessor.FindMemberExpression(mapping[key].Body);

                MemberInfo memberInfo =
                    this.ResultType.GetProperty(memberName) ?? (MemberInfo)this.ResultType.GetField(memberName);

                this.memberInfoMapping.Add(key, memberInfo);
            }
        }

        protected override IDictionary<string, MemberInfo> AliasMemberMapping
        {
            get { return this.memberInfoMapping; }
        }
    }
}
