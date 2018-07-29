namespace BugChang.DES.Core.Exchanges.Barcodes
{
    public enum EnumBarcodeStatus
    {
        已就绪,
        已投递,
        已签收,
        已勘误,
        已退回,
        申请退回
    }

    public enum EnumBarcodeSubStatus
    {
        正常,
        退回
    }
}
