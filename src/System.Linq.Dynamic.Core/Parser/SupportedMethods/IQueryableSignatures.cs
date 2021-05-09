using System.Collections;

namespace System.Linq.Dynamic.Core.Parser.SupportedMethods
{
    internal interface IQueryableSignatures
    {
        void All(bool predicate);
        void Any();
        void Any(bool predicate);
        void Average(decimal? selector);
        void Average(decimal selector);
        void Average(double? selector);
        void Average(double selector);
        void Average(float? selector);
        void Average(float selector);
        void Average(int? selector);
        void Average(int selector);
        void Average(long? selector);
        void Average(long selector);
        void Concat(IEnumerable enumerable);
        void Cast(string type);
        void Cast(Type type);
        void Count();
        void Count(bool predicate);
        void DefaultIfEmpty();
        void DefaultIfEmpty(object defaultValue);
        void Distinct();
        void Except(IEnumerable enumerable);
        void First();
        void First(bool predicate);
        void FirstOrDefault();
        void FirstOrDefault(bool predicate);
        void GroupBy(object keySelector);
        void GroupBy(object keySelector, object elementSelector);
        void Intersect(IEnumerable enumerable);
        void Last();
        void Last(bool predicate);
        void LastOrDefault();
        void LastOrDefault(bool predicate);
        void LongCount();
        void LongCount(bool predicate);
        void Max(object selector);
        void Min(object selector);
        void OfType(string type);
        void OfType(Type type);
        void OrderBy(object selector);
        void OrderByDescending(object selector);
        void Select(object selector);
        void SelectMany(object selector);
        void Single();
        void Single(bool predicate);
        void SingleOrDefault();
        void SingleOrDefault(bool predicate);
        void Skip(int count);
        void SkipWhile(bool predicate);
        void Sum(decimal? selector);
        void Sum(decimal selector);
        void Sum(double? selector);
        void Sum(double selector);
        void Sum(float? selector);
        void Sum(float selector);
        void Sum(int? selector);
        void Sum(int selector);
        void Sum(long? selector);
        void Sum(long selector);
        void Take(int count);
        void TakeWhile(bool predicate);
        void ThenBy(object selector);
        void ThenByDescending(object selector);
        void Union(IEnumerable enumerable);
        void Where(bool predicate);
    }
}
