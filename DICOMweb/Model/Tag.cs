namespace DICOMweb.Entities
{
    public class Tag
    {
        public string Name { get; set; }
        public string TagNumber { get; set; }
        public string Value { get; set; }
        public string Vr { get; set; }

        public Tag(string name, string number, string value, string vr)
        {
            Name = name;
            TagNumber = number;
            Value = value;
            Vr = vr;
        }

        public Tag(string value)
        {
            Value = value;
            TagNumber = "Unkonown";
            Name = "Unknown";
            Vr= "Unknown";
        }

            public string GetNumber()
        {
            return TagNumber;
        }

        public string GetValue()
        {
            return Value;
        }
    }
}
