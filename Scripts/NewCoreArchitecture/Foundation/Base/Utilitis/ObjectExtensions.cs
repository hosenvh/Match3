using Newtonsoft.Json;


namespace Match3.Foundation.Base.Utility
{
    public static class ObjectExtensions
    {
        public static string ToSerializedString(this object self)
        {
            return JsonConvert.SerializeObject(self);
        }
    }
}