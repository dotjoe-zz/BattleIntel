using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Db.Mapping
{
    class EntityModelMapper : ConventionModelMapper
    {
        private readonly Type baseEntityType = typeof(Entity);

        public EntityModelMapper()
        {
            this.DefaultConventions();

            IsEntity((t, declared) => baseEntityType.IsAssignableFrom(t) && baseEntityType != t && !t.IsInterface && !t.IsAbstract);
            IsRootEntity((t, declared) => baseEntityType.Equals(t.BaseType));

            ClassSpecificMappings();
        }

        private void ClassSpecificMappings()
        {
            Class<Entity>(map =>
            {
                map.Id(x => x.Id, m => m.Generator(Generators.Identity));
            });

            this.BeforeMapManyToOne += (insp, prop, map) => 
            {
                if (prop.LocalMember.Name == "LastUpdatedBy")
                {
                    map.NotNullable(false);
                }
            };

            this.UniqueColumn<UserOpenId>(x => x.OpenIdentifier, 255);
            this.UniqueColumn<Battle>(x => x.Name, 255);
            this.UniqueColumn<Team>(x => x.Name, 255);

            Class<BattleStat>(map =>
            {
                map.Component(x => x.Stat);
            });
        }

        void EntityModelMapper_BeforeMapManyToOne(IModelInspector modelInspector, PropertyPath member, IManyToOneMapper propertyCustomizer)
        {
            throw new NotImplementedException();
        }

        public HbmMapping CompileMappings()
        {
            return CompileMappingFor(baseEntityType.Assembly.GetExportedTypes().Where(x => x.IsSubclassOf(baseEntityType)));
        }
    }
}
