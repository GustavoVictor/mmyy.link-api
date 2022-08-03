public static class QueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> itens, int skip = 0, int take = 15)
    {
        return itens.Skip(skip * take).Take(take);
    }
}