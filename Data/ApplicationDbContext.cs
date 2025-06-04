using Kursovaya.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Kursovaya.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            // Отключаем автоматические миграции для подготовки к переносу на EF Core
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        // DbSets для основных сущностей
        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeType> NodeTypes { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<EquipmentModel> EquipmentModels { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<InstallationType> InstallationTypes { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<ModelPort> ModelPorts { get; set; }
        public DbSet<PortType> PortTypes { get; set; }
        public DbSet<ConnectorType> ConnectorTypes { get; set; }
        public DbSet<PortSpeed> PortSpeeds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Убираем конвенцию множественного числа для имен таблиц
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Настройка связей и ограничений
            ConfigureNodeRelationships(modelBuilder);
            ConfigureEquipmentRelationships(modelBuilder);
            ConfigureModelRelationships(modelBuilder);
            ConfigurePortRelationships(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureNodeRelationships(DbModelBuilder modelBuilder)
        {
            // Настройка связи Node -> Building
            modelBuilder.Entity<Node>()
                .HasRequired(n => n.BuildingInfo)
                .WithMany(b => b.Nodes)
                .HasForeignKey(n => n.Building)
                .WillCascadeOnDelete(false);

            // Настройка связи Node -> NodeType
            modelBuilder.Entity<Node>()
                .HasRequired(n => n.NodeType)
                .WithMany(nt => nt.Nodes)
                .HasForeignKey(n => n.Type)
                .WillCascadeOnDelete(false);

            // Настройка индексов для оптимизации
            modelBuilder.Entity<Node>()
                .HasIndex(n => n.Name)
                .HasName("IX_Node_Name");

            modelBuilder.Entity<Node>()
                .HasIndex(n => n.Building)
                .HasName("IX_Node_Building");
        }

        private void ConfigureEquipmentRelationships(DbModelBuilder modelBuilder)
        {
            // Настройка связи Equipment -> EquipmentModel
            modelBuilder.Entity<Equipment>()
                .HasRequired(e => e.Model)
                .WithMany(m => m.Equipment)
                .HasForeignKey(e => e.ModelId)
                .WillCascadeOnDelete(false);

            // Настройка связи Equipment -> Node
            modelBuilder.Entity<Equipment>()
                .HasRequired(e => e.Node)
                .WithMany(n => n.Equipment)
                .HasForeignKey(e => e.NodeId)
                .WillCascadeOnDelete(false);

            // Настройка связи Equipment -> Owner
            modelBuilder.Entity<Equipment>()
                .HasRequired(e => e.Owner)
                .WithMany(o => o.Equipment)
                .HasForeignKey(e => e.OwnerId)
                .WillCascadeOnDelete(false);

            // Настройка уникальных индексов
            modelBuilder.Entity<Equipment>()
                .HasIndex(e => e.Mac)
                .IsUnique()
                .HasName("IX_Equipment_Mac_Unique");

            modelBuilder.Entity<Equipment>()
                .HasIndex(e => e.Ip)
                .IsUnique()
                .HasName("IX_Equipment_Ip_Unique");

            modelBuilder.Entity<Equipment>()
                .HasIndex(e => e.SerialNumber)
                .HasName("IX_Equipment_SerialNumber");
        }

        private void ConfigureModelRelationships(DbModelBuilder modelBuilder)
        {
            // Настройка связи EquipmentModel -> EquipmentType
            modelBuilder.Entity<EquipmentModel>()
                .HasRequired(m => m.Type)
                .WithMany(t => t.Models)
                .HasForeignKey(m => m.TypeId)
                .WillCascadeOnDelete(false);

            // Настройка связи EquipmentModel -> InstallationType
            modelBuilder.Entity<EquipmentModel>()
                .HasRequired(m => m.InstallationType)
                .WithMany(it => it.Models)
                .HasForeignKey(m => m.InstallTypeId)
                .WillCascadeOnDelete(false);

            // Настройка связи EquipmentModel -> Manufacturer
            modelBuilder.Entity<EquipmentModel>()
                .HasRequired(m => m.Manufacturer)
                .WithMany(mf => mf.Models)
                .HasForeignKey(m => m.ManufacturerId)
                .WillCascadeOnDelete(false);

            // Настройка индексов
            modelBuilder.Entity<EquipmentModel>()
                .HasIndex(m => m.Name)
                .HasName("IX_EquipmentModel_Name");

            modelBuilder.Entity<EquipmentModel>()
                .HasIndex(m => new { m.ManufacturerId, m.Name })
                .IsUnique()
                .HasName("IX_EquipmentModel_Manufacturer_Name_Unique");
        }

        private void ConfigurePortRelationships(DbModelBuilder modelBuilder)
        {
            // Настройка связи ModelPort -> EquipmentModel
            modelBuilder.Entity<ModelPort>()
                .HasRequired(mp => mp.Model)
                .WithMany(m => m.Ports)
                .HasForeignKey(mp => mp.ModelId)
                .WillCascadeOnDelete(true);

            // Настройка связи ModelPort -> PortType
            modelBuilder.Entity<ModelPort>()
                .HasRequired(mp => mp.PortType)
                .WithMany(pt => pt.ModelPorts)
                .HasForeignKey(mp => mp.PortTypeId)
                .WillCascadeOnDelete(false);

            // Настройка связи PortType -> ConnectorType
            modelBuilder.Entity<PortType>()
                .HasRequired(pt => pt.ConnectorType)
                .WithMany(ct => ct.PortTypes)
                .HasForeignKey(pt => pt.ConnectorTypeId)
                .WillCascadeOnDelete(false);

            // Настройка связи PortType -> PortSpeed
            modelBuilder.Entity<PortType>()
                .HasRequired(pt => pt.Speed)
                .WithMany(ps => ps.PortTypes)
                .HasForeignKey(pt => pt.SpeedId)
                .WillCascadeOnDelete(false);

            // Уникальный индекс для ModelPort
            modelBuilder.Entity<ModelPort>()
                .HasIndex(mp => new { mp.ModelId, mp.PortTypeId })
                .IsUnique()
                .HasName("IX_ModelPort_Model_PortType_Unique");
        }

        // Метод для подготовки к переносу на EF Core
        public void PrepareForEfCore()
        {
            // Отключаем ленивую загрузку
            Configuration.LazyLoadingEnabled = false;

            // Отключаем автоматическое отслеживание изменений
            Configuration.AutoDetectChangesEnabled = false;

            // Отключаем проверку прокси
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Освобождаем ресурсы
            }
            base.Dispose(disposing);
        }
    }
}