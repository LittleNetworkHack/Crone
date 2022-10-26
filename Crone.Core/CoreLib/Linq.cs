namespace Crone;

public static partial class CoreLib
{
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        foreach (var item in source)
            action(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, int, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> query, bool condition, Expression<Func<TSource, int, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    //public static CoreDataTable ToCoreTable<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> expression)
    //{
    //    var paramExpr = expression.Parameters.Single();

    //    var initList = new List<ElementInit>();
    //    var addMethod = typeof(CoreDataRow).GetMethod("Add", new[] { typeof(string), typeof(object) });
    //    var propertyList = expression.GetPropertyAccessList();
    //    foreach (var property in propertyList)
    //    {
    //        var expr = Expression.ElementInit(addMethod, new Expression[]
    //        {
    //            Expression.Constant(property.Name, typeof(string)),
    //            Expression.Convert(Expression.Property(paramExpr, property.GetGetMethod()), typeof(object))
    //        });
    //        initList.Add(expr);
    //    }

    //    var newExpression = Expression.New(typeof(CoreDataRow));
    //    var body = Expression.ListInit(newExpression, initList);
    //    var lambda = Expression.Lambda<Func<TSource, CoreDataRow>>(body, paramExpr);

    //    var query = source.Select(lambda);
    //    var result = new CoreDataTable();
    //    foreach (var item in source.Select(lambda))
    //    {
    //        result.Add(item);
    //    }
    //    return result;
    //}
}
