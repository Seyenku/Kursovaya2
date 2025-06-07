using System;
using System.Collections.Generic;

namespace Kursovaya.Models.Reporting
{
    public class FieldDescriptor<TEntity>
    {
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public Func<TEntity, string> Value { get; private set; }

        public FieldDescriptor(string name, string displayName, Func<TEntity, string> value)
        {
            Name = name;
            DisplayName = displayName;
            Value = value;
        }
    }

    public static class FieldRegistry
    {
        public static readonly FieldDescriptor<EquipmentModel>[] Model = new[]
        {
            new FieldDescriptor<EquipmentModel>("Name", "Модель", m => m.Name),
            new FieldDescriptor<EquipmentModel>("Manufacturer", "Производитель", m => m.Manufacturer != null ? m.Manufacturer.Name : ""),
            new FieldDescriptor<EquipmentModel>("Type", "Тип оборуд.", m => m.Type != null ? m.Type.Name : ""),
            new FieldDescriptor<EquipmentModel>("Managed", "Управляемый", m => m.Managed == 1 ? "Да" : "Нет"),
            new FieldDescriptor<EquipmentModel>("Console", "Console port", m => m.Console == 1 ? "Да" : "Нет"),
            new FieldDescriptor<EquipmentModel>("Poe", "PoE", m => m.Poe == 1 ? "Да" : "Нет")
        };

        public static readonly FieldDescriptor<Node>[] Node = new[]
        {
            new FieldDescriptor<Node>("Name", "Название узла", n => n.Name),
            new FieldDescriptor<Node>("Building", "Корпус", n => n.BuildingInfo != null ? n.BuildingInfo.BuildName : ""),
            new FieldDescriptor<Node>("Type", "Тип узла", n => n.NodeType != null ? n.NodeType.Name : ""),
            new FieldDescriptor<Node>("Verification", "Дата проверки", n => n.VerificationDate.HasValue ? n.VerificationDate.Value.ToShortDateString() : "")
        };

        public static readonly FieldDescriptor<Equipment>[] Equipment = new[]
        {
            new FieldDescriptor<Equipment>("Ip", "IP", e => e.Ip),
            new FieldDescriptor<Equipment>("Mac", "MAC", e => e.Mac),
            new FieldDescriptor<Equipment>("SerialNumber", "Серийный №", e => e.SerialNumber),
            new FieldDescriptor<Equipment>("Model", "Модель", e => e.Model != null ? e.Model.Name : ""),
            new FieldDescriptor<Equipment>("Node", "Узел", e => e.Node != null ? e.Node.Name : ""),
            new FieldDescriptor<Equipment>("Owner", "Владелец", e => e.Owner != null ? e.Owner.Name : "")
        };

        public static readonly Dictionary<string, IList<object>> Map = new Dictionary<string, IList<object>>(StringComparer.OrdinalIgnoreCase)
        {
            { "EquipmentModels", ConvertToObjectList(Model) },
            { "Nodes", ConvertToObjectList(Node) },
            { "Equipment", ConvertToObjectList(Equipment) }
        };

        private static IList<object> ConvertToObjectList<T>(FieldDescriptor<T>[] input)
        {
            var list = new List<object>();
            foreach (var item in input)
                list.Add(item);
            return list;
        }
    }
}
