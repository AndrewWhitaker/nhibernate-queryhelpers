namespace NHibernate.QueryHelpers.Transformers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Transform;
    using Environment = Cfg.Environment;

    /// <summary>
    /// Base class for result transformers that manually map column names to properties.
    /// </summary>
    /// <typeparam name="TResultType">The type to map to.</typeparam>
    public abstract class AliasMappingResultTransformer<TResultType> : AliasedTupleSubsetResultTransformer
    {
        protected readonly ConstructorInfo Constructor;
        protected readonly Type ResultType = typeof(TResultType);

        /// <summary>
        /// Creates a new <code>AliasMappingResultTransformer</code>.
        /// </summary>
        protected AliasMappingResultTransformer()
        {
            this.Constructor = this.ResultType.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (this.Constructor == null && this.ResultType.IsClass)
            {
                throw new ArgumentException(
                    "The target class of a QueryColumnAttributeTransformer needs a parameterless constructor");
            }
        }

        /// <summary>
        /// Gets the mapping between columns and class members.
        /// </summary>
        protected abstract IDictionary<string, MemberInfo> AliasMemberMapping { get; }

        public override bool IsTransformedValueATupleElement(string[] aliases, int tupleLength)
        {
            return false;
        }

        public override IList TransformList(IList collection)
        {
            return collection;
        }

        public override object TransformTuple(object[] tuple, string[] aliases)
        {
            if (aliases == null)
            {
                throw new ArgumentNullException("aliases");
            }

            object result;

            try
            {
                result = this.ResultType.IsClass
                    ? this.Constructor.Invoke(null)
                    : Environment.BytecodeProvider.ObjectsFactory.CreateInstance(this.ResultType, true);

                for (int i = 0; i < aliases.Length; i++)
                {
                    string alias = aliases[i];

                    MemberInfo member;

                    if (this.AliasMemberMapping.TryGetValue(alias, out member))
                    {
                        if (member.MemberType == MemberTypes.Property)
                        {
                            ((PropertyInfo)member).SetValue(result, tuple[i]);
                        }
                        else if (member.MemberType == MemberTypes.Field)
                        {
                            ((FieldInfo)member).SetValue(result, tuple[i]);
                        }
                    }
                    else
                    {
                        throw new ArgumentException(
                            string.Format("{0} has no field or property mapped to column '{1}'", this.ResultType, alias));
                    }
                }
            }
            catch (MemberAccessException e)
            {
                throw new HibernateException("Could not instantiate result class: " + this.ResultType, e);
            }

            return result;
        }
    }
}