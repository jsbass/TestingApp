using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestingApp.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<TreeItem<T>> GenerateTree<T, K>(this IEnumerable<T> collection, Func<T, K> idSelector, Func<T, K> parentIdSelector, K rootId = default(K))
        {
            foreach (var item in collection.Where(i => Equals(parentIdSelector(i),rootId)))
            {
                yield return new TreeItem<T>()
                {
                    Item = item,
                    Children = collection.GenerateTree(idSelector, parentIdSelector, idSelector(item))
                };
            }
        }
    }

    public class TreeItem<T>
    {
        public T Item { get; set; }
        public IEnumerable<TreeItem<T>> Children { get; set; }
    }
}
