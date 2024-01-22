namespace System.Linq.Dynamic.Core.Parser;

internal static class ConstantExpressionHelperFactory
{
    private static readonly object Lock = new();
    private static ConstantExpressionHelper? _instance;

    public static ConstantExpressionHelper GetInstance(ParsingConfig config)
    {
        if (_instance == null)
        {
            lock (Lock)
            {
                _instance ??= new ConstantExpressionHelper(config);
            }
        }

        return _instance;
    }
}