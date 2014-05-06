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

            this.UniqueColumn<UserOpenId>(x => x.OpenIdentifier, 255);
            this.UniqueColumn<Battle>(x => x.Name, 255);
            this.UniqueColumn<Team>(x => x.Name, 255);

            Class<User>(map =>
            {
                map.Property(x => x.Email, m => m.NotNullable(true));
                map.Property(x => x.DisplayName, m => m.NotNullable(true));
            });

            Class<BattleStat>(map =>
            {
                map.Component(x => x.Stat);
                map.ManyToOne(x => x.IntelReport, m => m.NotNullable(false));
            });

            Class<IntelReport>(map =>
            {
                map.Property(x => x.Text, m =>  
                {
                    m.Length(8001);
                    m.NotNullable(true);
                });
                map.Property(x => x.TextHash, m =>
                {
                    m.Length(40);
                    m.NotNullable(true);
                });
                map.ManyToOne(x => x.DuplicateOf, m => m.NotNullable(false));
                map.Set(x => x.Stats, m =>
                {
                    m.Key(x => x.Column("IntelReportId"));
                    m.Inverse(true);
                    m.Cascade(Cascade.None);
                });
            });
        }

        public HbmMapping CompileMappings()
        {
            return CompileMappingFor(baseEntityType.Assembly.GetExportedTypes().Where(x => x.IsSubclassOf(baseEntityType)));
        }
    }
}
