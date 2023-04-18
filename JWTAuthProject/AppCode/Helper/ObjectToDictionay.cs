using System.Reflection;

namespace JWTAuthProject.AppCode.Helper
{
    public static class ObjectToDictionay
    {
        public static Dictionary<string, dynamic> ToDictionary(this object someObject)
        {
            var res = someObject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => (dynamic)prop.GetValue(someObject, null));
            return res;
        }
    }
}
