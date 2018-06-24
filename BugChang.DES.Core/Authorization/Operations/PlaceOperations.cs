namespace BugChang.DES.Core.Authorization.Operations
{
    public class PlaceOperations : IOperations
    {
        public string GetMenuUrl()
        {
            return "/Exchange/Place";
        }

        public string GetModuleName()
        {
            return "Place";
        }

        public Operation PlaceCreate => new Operation
        {
            Name = Operation.CreateName,
            Code = "Place.Create"
        };

        public Operation PlaceEdit => new Operation
        {
            Name = Operation.EditName,
            Code = "Place.Edit"
        };

        public Operation PlaceDelete => new Operation
        {
            Name = Operation.CreateName,
            Code = "Place.Delete"
        };
    }
}
