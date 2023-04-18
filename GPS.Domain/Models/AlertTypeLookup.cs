namespace GPS.Domain.Models
{
    public class AlertTypeLookup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NameEn { get; set; }

        public int? RowOrder { get; set; }

        public bool IsRange { get; set; }
        
        public bool HasMinValue { get; set; }
        
        public bool HasMaxValue { get; set; }

        public string DataType { get; set; }

        public string Unit { get; set; }

        public string UnitEn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
