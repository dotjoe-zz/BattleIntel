using BattleIntel.Core.Db.Mapping;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleIntel.Core.Db
{
    public class NHibernateConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureByCode">optional, supply custom configuration by code or leave null to look for configuration file</param>
        /// <param name="schemaExportPath">optional, supply a path to export schema and hbm.xml files. NOT for production use.</param>
        /// <param name="exposeCfg">optional, NH configuration is exposed after configured and schema exported.</param>
        /// <returns></returns>
        public static ISessionFactory BuildSessionFactory(Action<Configuration> configureByCode = null, string schemaExportPath = null, Action<Configuration> exposeCfg = null)
        {
            var configAction = configureByCode ?? (x => x.Configure());

            var cfg = new Configuration();
            configAction(cfg);

            var entityMappings = new EntityModelMapper().CompileMappings();
            cfg.AddMapping(entityMappings);

            if (!string.IsNullOrEmpty(schemaExportPath))
            {
                ExportSchema(cfg, schemaExportPath);
                ExportHbmMappingsXml(entityMappings, "Entity", schemaExportPath);
            }

            cfg.SetInterceptor(new UtcDateTimeInterceptor());

            if (exposeCfg != null) exposeCfg(cfg);

            return cfg.BuildSessionFactory();
        }

        private static void ExportSchema(Configuration cfg, string schemaExportPath)
        {
            new SchemaExport(cfg)
                .SetOutputFile(Path.Combine(schemaExportPath, "Create-db.sql"))
                .Create(false, false);

            using (TextWriter tw = new StreamWriter(File.Open(Path.Combine(schemaExportPath, "Alter-db.sql"), FileMode.Create, FileAccess.Write)))
            {
                new SchemaUpdate(cfg)
                    .Execute(s => tw.Write(s), false);
            }
        }

        private static void ExportHbmMappingsXml(HbmMapping hbmMapping, string name, string exportPath)
        {
            using (TextWriter tw = new StreamWriter(File.Open(Path.Combine(exportPath, name + ".hbm.xml"), FileMode.Create, FileAccess.Write)))
            {
                tw.Write(hbmMapping.AsString());
            }
        }
    }
}
