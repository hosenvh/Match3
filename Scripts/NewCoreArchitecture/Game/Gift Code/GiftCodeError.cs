public struct GiftCodeError
{
    public int statusCode;
    public string statusDescription;

    public GiftCodeError(int statusCode, string statusDescription)
    {
        this.statusCode = statusCode;
        this.statusDescription = statusDescription;
    }
}