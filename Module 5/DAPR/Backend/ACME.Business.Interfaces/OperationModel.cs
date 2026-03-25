namespace ACME.Business.Interfaces;

public class OperationModel
{
    public DateTime DateAdded { get; set; }
    public char Operation { get; set; }
    public int A { get; set; }
    public int B { get; set; }
    public int Result { get; set; }
}
