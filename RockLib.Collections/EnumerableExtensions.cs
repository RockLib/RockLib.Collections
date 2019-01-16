using System;
using System.Collections.Generic;

namespace RockLib.Collections
{
    /// <summary>
    /// Provides methods that extend the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Creates a <see cref="NamedCollection{T}"/> from an <see cref="IEnumerable{T}"/>
        /// according to the specified <paramref name="getName"/> function.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="values"/>.</typeparam>
        /// <param name="values">The values that make up the collection.</param>
        /// <param name="getName">A function that gets the name of a value.</param>
        /// <param name="stringComparer">
        /// The equality comparer used to determine if names are equal. The default value is
        /// <see cref="StringComparer.OrdinalIgnoreCase"/>.
        /// </param>
        /// <param name="defaultName">
        /// The name that indicates an item should be used as the <see cref="NamedCollection{T}.DefaultValue"/>
        /// for the <see cref="NamedCollection{T}"/>.
        /// </param>
        /// <returns></returns>
        public static NamedCollection<T> ToNamedCollection<T>(this IEnumerable<T> values, Func<T, string> getName,
            IEqualityComparer<string> stringComparer = null, string defaultName = "default") =>
            new NamedCollection<T>(values, getName, stringComparer, defaultName);
    }
}
