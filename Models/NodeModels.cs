using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kursovaya.Models
{
    [Table("tc_nodes_communication")]
    public class Node
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        public int Building { get; set; }

        [Required]
        public int Type { get; set; }

        public string Other { get; set; }

        [Column("verification_date")]
        public DateTime? VerificationDate { get; set; }

        [ForeignKey("Building")]
        public virtual Building BuildingInfo { get; set; }

        [ForeignKey("Type")]
        public virtual NodeType NodeType { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
    }

    [Table("tc_nodes_type")]
    public class NodeType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<Node> Nodes { get; set; }
    }

    [Table("ts_buildings")]
    public class Building
    {
        [Key]
        [Column("buildid")]
        public int BuildId { get; set; }

        [Required]
        [StringLength(255)]
        [Column("buildname")]
        public string BuildName { get; set; }

        public virtual ICollection<Node> Nodes { get; set; }
    }

    [Table("tc_switching_equipment")]
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        [Column("id_model")]
        public int ModelId { get; set; }

        [Column("id_nodes")]
        public int NodeId { get; set; }

        [Column("id_owner")]
        public int OwnerId { get; set; }

        [Column("serial_number")]
        [StringLength(255)]
        public string SerialNumber { get; set; }

        [Column("inventory_number")]
        [StringLength(255)]
        public string InventoryNumber { get; set; }

        [StringLength(255)]
        public string Mac { get; set; }

        [StringLength(255)]
        public string Ip { get; set; }

        [ForeignKey("ModelId")]
        public virtual EquipmentModel Model { get; set; }

        [ForeignKey("NodeId")]
        public virtual Node Node { get; set; }

        [ForeignKey("OwnerId")]
        public virtual Owner Owner { get; set; }
    }

    [Table("tc_model_equipment")]
    public class EquipmentModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("id_type")]
        public int TypeId { get; set; }

        [Column("type_install")]
        public int InstallTypeId { get; set; }

        [Column("id_manufacturer")]
        public int ManufacturerId { get; set; }

        public bool Managed { get; set; }

        public bool Console { get; set; }

        public bool Poe { get; set; }

        [ForeignKey("TypeId")]
        public virtual EquipmentType Type { get; set; }

        [ForeignKey("InstallTypeId")]
        public virtual InstallationType InstallationType { get; set; }

        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer Manufacturer { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }

        public virtual ICollection<ModelPort> Ports { get; set; }
    }

    [Table("tc_equipment_type")]
    public class EquipmentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<EquipmentModel> Models { get; set; }
    }

    [Table("tc_model_type_installation")]
    public class InstallationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<EquipmentModel> Models { get; set; }
    }

    [Table("tc_equipment_manufacturer")]
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<EquipmentModel> Models { get; set; }
    }

    [Table("tc_model_port")]
    public class ModelPort
    {
        [Key]
        public int Id { get; set; }

        [Column("id_model")]
        public int ModelId { get; set; }

        [Column("id_port")]
        public int PortTypeId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("ModelId")]
        public virtual EquipmentModel Model { get; set; }

        [ForeignKey("PortTypeId")]
        public virtual PortType PortType { get; set; }
    }

    [Table("tc_port_type")]
    public class PortType
    {
        [Key]
        public int Id { get; set; }

        [Column("id_type")]
        public int ConnectorTypeId { get; set; }

        [Column("id_speed")]
        public int SpeedId { get; set; }

        [ForeignKey("ConnectorTypeId")]
        public virtual ConnectorType ConnectorType { get; set; }

        [ForeignKey("SpeedId")]
        public virtual PortSpeed Speed { get; set; }

        public virtual ICollection<ModelPort> ModelPorts { get; set; }
    }

    [Table("tc_port_connector_type")]
    public class ConnectorType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<PortType> PortTypes { get; set; }
    }

    [Table("tc_port_speed")]
    public class PortSpeed
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<PortType> PortTypes { get; set; }
    }

    public class Owner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
    }
}