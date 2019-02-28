namespace System.Linq.Dynamic.Core.Parser
{
    interface IKeywordsHelper
    {
        bool TryGetValue(string name, out object type);
    }
}
