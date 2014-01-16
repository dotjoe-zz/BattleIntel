using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Db.Mapping
{
    static class ModelMapperExtensions
    {
        /// <summary>
        /// "Name" properties are mapped as non-null.
        /// non-nullable value types are mapped as non-null
        /// Component columns named with the property prefix and allow nulls
        /// ManyToOne FK naming convention and default non-null
        /// </summary>
        /// <param name="mapper"></param>
        public static void DefaultConventions(this ModelMapper mapper)
        {
            mapper.BeforeMapProperty += (insp, prop, map) =>
            {
                var propType = prop.LocalMember.GetPropertyOrFieldType();
                var isComponent = insp.IsComponent(prop.LocalMember.DeclaringType);

                if (propType.Equals(typeof(string)))
                {
                    //All "Name" (non-component level) properties must be non-null
                    if (prop.LocalMember.Name == "Name" && !isComponent)
                    {
                        map.NotNullable(true);
                    }
                }
                else if (propType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
                {
                    //non-nullable value type should default to non-null
                    map.NotNullable(true);
                }

                if (isComponent)
                {
                    //map a component's property name as column prefix
                    map.Column(string.Format("{0}_{1}", prop.PreviousPath.LocalMember.Name, prop.LocalMember.Name));
                }
            };

            mapper.BeforeMapManyToOne += (insp, prop, map) =>
            {
                if (insp.IsComponent(prop.LocalMember.DeclaringType))
                {
                    //map a component's property name as column prefix and allow non-nulls
                    map.Column(string.Format("{0}_{1}Id", prop.PreviousPath.LocalMember.Name, prop.LocalMember.Name));
                    map.NotNullable(false);
                }
                else
                {
                    //default all many-to-one's as not nullable
                    map.Column(prop.LocalMember.Name + "Id");
                    map.NotNullable(true);
                }
                map.Cascade(Cascade.None);
                map.ForeignKey(string.Format("FK_{0}_{1}", prop.GetContainerEntity(insp).Name, prop.LocalMember.Name));
            };

            mapper.BeforeMapSet += (insp, prop, map) =>
            {
                map.Key(km => km.Column(prop.GetContainerEntity(insp).Name + "Id"));
                map.Cascade(Cascade.All);
            };
        }


        /// <summary>
        /// Bi-directional Many-to-Many
        /// </summary>
        /// <typeparam name="TControllingEntity"></typeparam>
        /// <typeparam name="TInverseEntity"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="controllingProperty"></param>
        /// <param name="inverseProperty"></param>
        public static void ManyToMany<TControllingEntity, TInverseEntity>(
            this ModelMapper mapper,
            Expression<Func<TControllingEntity, IEnumerable<TInverseEntity>>> controllingProperty,
            Expression<Func<TInverseEntity, IEnumerable<TControllingEntity>>> inverseProperty
            )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var inverseEntityTypeName = typeof(TInverseEntity).Name;
            var controllingEntityTypeName = typeof(TControllingEntity).Name;

            var controllingColumnName = string.Format("{0}Id", inverseEntityTypeName);
            var inverseColumnName = string.Format("{0}Id", controllingEntityTypeName);
            var tableName = string.Format("{0}To{1}", controllingEntityTypeName, inverseEntityTypeName);

            mapper.Class<TControllingEntity>(map =>
            {
                map.Set(controllingProperty, cm =>
                {
                    cm.Cascade(Cascade.Persist);
                    cm.Table(tableName);
                    cm.Key(km =>
                    {
                        km.Column(controllingColumnName);
                        km.NotNullable(true);
                    });
                },
                em =>
                {
                    em.ManyToMany(m => m.Column(inverseColumnName));
                });
            });

            mapper.Class<TInverseEntity>(map =>
            {
                map.Set(inverseProperty, cm =>
                {
                    cm.Cascade(Cascade.None);
                    cm.Table(tableName);
                    cm.Inverse(true);
                    cm.Key(km =>
                    {
                        km.Column(inverseColumnName);
                        km.NotNullable(true);
                    });
                },
                em =>
                {
                    em.ManyToMany(m => m.Column(controllingColumnName));
                });
            });
        }

        /// <summary>
        /// Uni-directional Many-to-Many
        /// </summary>
        /// <typeparam name="TControllingEntity"></typeparam>
        /// <typeparam name="TInverseEntity"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="controllingProperty"></param>
        public static void ManyToMany<TControllingEntity, TInverseEntity>(
            this ModelMapper mapper,
            Expression<Func<TControllingEntity, IEnumerable<TInverseEntity>>> controllingProperty
            )
            where TControllingEntity : class
            where TInverseEntity : class
        {
            var inverseEntityTypeName = typeof(TInverseEntity).Name;
            var controllingEntityTypeName = typeof(TControllingEntity).Name;

            var controllingColumnName = string.Format("{0}Id", inverseEntityTypeName);
            var inverseColumnName = string.Format("{0}Id", controllingEntityTypeName);
            var tableName = string.Format("{0}To{1}", controllingEntityTypeName, inverseEntityTypeName);

            mapper.Class<TControllingEntity>(map =>
            {
                map.Set(controllingProperty, cm =>
                {
                    cm.Cascade(Cascade.Persist);
                    cm.Table(tableName);
                    cm.Key(km =>
                    {
                        km.Column(controllingColumnName);
                        km.NotNullable(true);
                    });
                }, em =>
                {
                    em.ManyToMany(m => m.Column(inverseColumnName));
                });
            });
        }

        /// <summary>
        /// One-to-Many bi-directional
        /// </summary>
        /// <typeparam name="TOne"></typeparam>
        /// <typeparam name="TMany"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="manyProperty"></param>
        /// <param name="oneProperty"></param>
        public static void OneToMany<TOne, TMany>(
            this ModelMapper mapper,
            Expression<Func<TOne, IEnumerable<TMany>>> manyProperty,
            Expression<Func<TMany, TOne>> oneProperty,
            Cascade OneToManyCascade = Cascade.None
            )
            where TOne : class
            where TMany : class
        {
            var propertyName = ((MemberExpression)oneProperty.Body).Member.Name;
            var columnName = string.Format("{0}Id", propertyName);

            mapper.Class<TOne>(map =>
            {
                map.Set(manyProperty, cm =>
                {
                    cm.Inverse(true);
                    cm.Cascade(OneToManyCascade);
                    cm.Key(km =>
                    {
                        km.Column(columnName);
                        km.NotNullable(true);
                    });
                }, em =>
                {
                    em.OneToMany();
                });
            });

            mapper.Class<TMany>(map =>
            {
                map.ManyToOne(oneProperty, m =>
                {
                    m.Column(columnName);
                    m.NotNullable(true);
                });
            });
        }

        /// <summary>
        /// One-to-Many uni-directional
        /// </summary>
        /// <typeparam name="TOne"></typeparam>
        /// <typeparam name="TMany"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="manyProperty"></param>
        /// <param name="oneProperty"></param>
        public static void OneToMany<TOne, TMany>(
            this ModelMapper mapper,
            Expression<Func<TOne, IEnumerable<TMany>>> manyProperty
            )
            where TOne : class
            where TMany : class
        {
            var columnName = string.Format("{0}Id", typeof(TOne).Name);

            mapper.Class<TOne>(map =>
            {
                map.Set(manyProperty, cm =>
                {
                    cm.Cascade(Cascade.All);
                    cm.Key(km =>
                    {
                        km.Column(columnName);
                        km.NotNullable(true);
                    });
                }, em =>
                {
                    em.OneToMany();
                });
            });
        }

        /// <summary>
        /// Unique not-null column with a specific length
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="uniqueProperty"></param>
        /// <param name="length"></param>
        public static void UniqueColumn<TEntity>(
            this ModelMapper mapper,
            Expression<Func<TEntity, string>> uniqueProperty,
            int length
            )
            where TEntity : class
        {
            mapper.Class<TEntity>(map =>
            {
                map.Property(uniqueProperty, m =>
                {
                    m.Unique(true);
                    m.NotNullable(true);
                    m.Length(length);
                });
            });
        }

        public static void UniqueComposite<TEntity, TMany>(
            this ModelMapper mapper,
            Expression<Func<TEntity, TMany>> uniqueManyToOne,
            Expression<Func<TEntity, string>> uniqueProperty,
            int length
            )
            where TEntity : class
            where TMany : class
        {
            mapper.Class<TEntity>(map =>
            {
                /*
                 * This unique key name is not actually used as the constraint name.
                 * But, it used in the generated hbm.xml file so we'll pick a nice unique name.
                 */
                var manyToOneName = ((MemberExpression)uniqueManyToOne.Body).Member.Name;
                var propertyName = ((MemberExpression)uniqueProperty.Body).Member.Name;
                string uqKey = string.Format("UQ_{0}_{1}_{2}", typeof(TEntity).Name, manyToOneName, propertyName);

                map.ManyToOne(uniqueManyToOne, m =>
                {
                    m.UniqueKey(uqKey);
                });
                map.Property(uniqueProperty, m =>
                {
                    m.UniqueKey(uqKey);
                    m.Length(length);
                });
            });
        }
    }
}
