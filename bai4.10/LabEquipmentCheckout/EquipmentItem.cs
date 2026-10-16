namespace LabEquipmentCheckout;

public class EquipmentItem
{
    public string Ma { get; set; } = "";
    public string Ten { get; set; } = "";
    public override string ToString() => Ten;
}
