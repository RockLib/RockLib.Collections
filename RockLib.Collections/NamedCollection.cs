using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.StringComparer;

namespace RockLib.Collections
{
    /// <summary>
    /// Represents a collection that can be retrieved by name.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class NamedCollection<T> : IReadOnlyCollection<T>, IReadOnlyDictionary<string, T>
    {
        private const string _defaultDefaultName = "default";

        private readonly IReadOnlyDictionary<string, T> _valuesByName;
        private readonly Func<T, string> _dictionaryKeySelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedCollection{T}"/> class
        /// according to the specified <paramref name="getName"/> function.
        /// </summary>
        /// <param name="values">The values that make up the collection.</param>
        /// <param name="getName">A function that gets the name of a value.</param>
        /// <param name="stringComparer">
        /// The equality comparer used to determine if names are equal. The default value is
        /// <see cref="OrdinalIgnoreCase"/>.
        /// </param>
        /// <param name="defaultName">
        /// The name that indicates an item should be used as the <see cref="DefaultValue"/>
        /// for the <see cref="NamedCollection{T}"/>.
        /// </param>
        /// <param name="strict">
        /// Whether to throw an exception if <paramref name="values"/> contains more than one
        /// default value or more than one value with the same name. If <see langword="false"/>,
        /// when there are duplicates, then the last value in the collection wins.
        /// </param>
        public NamedCollection(IEnumerable<T> values, Func<T, string> getName,
            IEqualityComparer<string> stringComparer = null, string defaultName = _defaultDefaultName, bool strict = true)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (getName == null)
                throw new ArgumentNullException(nameof(getName));

            StringComparer = stringComparer ?? OrdinalIgnoreCase;
            DefaultName = !string.IsNullOrEmpty(defaultName) ? defaultName : _defaultDefaultName;

            var valuesByName = new Dictionary<string, T>(StringComparer);
            var hasDefaultValue = false;

            foreach (var value in values)
            {
                var name = getName(value);
                if (IsDefaultName(name))
                {
                    if (strict && hasDefaultValue)
                        throw new ArgumentException("Cannot have more than one default value.", nameof(values));
                    hasDefaultValue = true;
                    DefaultValue = value;
                }
                else
                {
                    if (strict && valuesByName.ContainsKey(name))
                        throw new ArgumentException($"Cannot have more than one value with the same name: {name}.", nameof(values));
                    valuesByName[name] = value;
                }
            }

            _dictionaryKeySelector = value =>
            {
                var name = getName(value);
                return IsDefaultName(name) ? DefaultName : name;
            };

            Count = valuesByName.Count + (hasDefaultValue ? 1 : 0);

            _valuesByName = valuesByName;

#if NET451
            NamedValues = valuesByName.Values.ToList();
#else
            NamedValues = valuesByName.Values;
#endif
        }

        /// <summary>
        /// Gets the default (unnamed) value.
        /// </summary>
        public T DefaultValue { get; }

        /// <summary>
        /// Gets the named (non-default) values.
        /// </summary>
        public IReadOnlyCollection<T> NamedValues { get; }

        /// <summary>
        /// Gets the equality comparer used to determine if names are equal.
        /// </summary>
        public IEqualityComparer<string> StringComparer { get; }

        /// <summary>
        /// Gets the default name. A value with this (or <see langword="null" /> or
        /// <see cref="string.Empty"/>) as its name is considered a default value.
        /// </summary>
        public string DefaultName { get; }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the value with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name to locate in the <see cref="NamedCollection{T}" />.
        /// </param>
        /// <returns>
        /// The value with the specified name. If a value with the specified name is not found, an exception
        /// is thrown.
        /// </returns>
        public T this[string name] => TryGetValue(name, out var value) ? value : throw NameNotFound(name);

        /// <summary>
        /// Determines whether the collection contains an value with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name to locate in the <see cref="NamedCollection{T}" />.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="NamedCollection{T}" /> contains an value with the
        /// specified name; otherwise, <see langword="false" />.
        /// </returns>
        public bool Contains(string name) => TryGetValue(name, out var dummy);

        /// <summary>
        /// Attempt to retrieve a value by its name.
        /// </summary>
        /// <param name="name">
        /// The name to locate in the <see cref="NamedCollection{T}" />.
        /// </param>
        /// <param name="value">
        /// When this method returns, contains the object with a name matching the
        /// <paramref name="name"/> parameter, or the default value of the type if
        /// no value was found with a matching name.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a value with a name was found; otherwise <see langword="false" />.
        /// </returns>
        public bool TryGetValue(string name, out T value)
        {
            if (IsDefaultName(name))
            {
                value = DefaultValue;
                return value != null;
            }

            if (_valuesByName.TryGetValue(name, out value))
                return value != null;

            value = default(T);
            return false;
        }

        /// <summary>
        /// Returns whether the specified name is a default name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>
        /// <see langword="true"/> if the specified name is a default name; otherwise <see langword="false"/>.
        /// </returns>
        public bool IsDefaultName(string name) =>
            string.IsNullOrEmpty(name) || StringComparer.Equals(name, DefaultName);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (DefaultValue != null)
                yield return DefaultValue;
            foreach (var namedValue in NamedValues)
                yield return namedValue;
        }

        IEnumerable<string> IReadOnlyDictionary<string, T>.Keys => this.Select(_dictionaryKeySelector);

        IEnumerable<T> IReadOnlyDictionary<string, T>.Values => this;

        bool IReadOnlyDictionary<string, T>.ContainsKey(string key) => Contains(key);

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            foreach (var value in this)
                yield return new KeyValuePair<string, T>(_dictionaryKeySelector(value), value);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private KeyNotFoundException NameNotFound(string name) =>
            new KeyNotFoundException(IsDefaultName(name)
                ? "The named collection does not have a default value."
                : $"The given name was not present in the named collection: {name}.");
    }
}
