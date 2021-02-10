using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RockLib.Collections.Tests
{
    public class NamedCollectionTests
    {
        [Fact]
        public void ConstructorThrowsWhenValuesIsNull()
        {
            Action action = () => new NamedCollection<Foo>(null, f => f.Name);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ConstructorThrowsWhenGetNameIsNull()
        {
            var values = new[] { new Foo("bar") };

            Action action = () => new NamedCollection<Foo>(values, null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ConstructorThrowsWhenThereIsMoreThanOneDefaultValue()
        {
            var values = new[] { new Foo("default"), new Foo("default") };

            Action action = () => new NamedCollection<Foo>(values, f => f.Name);

            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("Cannot have more than one default value.*Parameter* *values*");
        }

        [Fact]
        public void ConstructorThrowsWhenThereIsMoreThanOneValueWithTheSameName()
        {
            var values = new[] { new Foo("bar"), new Foo("bar") };

            Action action = () => new NamedCollection<Foo>(values, f => f.Name);

            action.Should().ThrowExactly<ArgumentException>()
                .WithMessage("Cannot have more than one value with the same name: bar.*Parameter* *values*");
        }

        [Fact]
        public void ConstructorDoesNotThrowWhenThereIsMoreThanOneDefaultValueAndStrictIsFalse()
        {
            var values = new[] { new Foo("default"), new Foo("default") };

            Action action = () => new NamedCollection<Foo>(values, f => f.Name, strict: false);

            action.Should().NotThrow();
        }

        [Fact]
        public void ConstructorDoesNotThrowWhenThereIsMoreThanOneValueWithTheSameNameAndStrictIsFalse()
        {
            var values = new[] { new Foo("bar"), new Foo("bar") };

            Action action = () => new NamedCollection<Foo>(values, f => f.Name, strict: false);

            action.Should().NotThrow();
        }

        [Fact]
        public void ConstructorSetsStringComparerToOrdinalIgnoreCaseWhenNotSpecified()
        {
            var values = new[] { new Foo("bar") };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.StringComparer.Should().BeSameAs(StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void ConstructorSetsStringComparer()
        {
            var values = new[] { new Foo("bar") };

            var collection = new NamedCollection<Foo>(values, f => f.Name, StringComparer.InvariantCulture);

            collection.StringComparer.Should().BeSameAs(StringComparer.InvariantCulture);
        }

        [Fact]
        public void ConstructorSetsDefaultNameToDefaultWhenNotSpecified()
        {
            var values = new[] { new Foo("bar") };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.DefaultName.Should().Be("default");
        }

        [Fact]
        public void ConstructorSetsDefaultNameToDefaultWhenNull()
        {
            var values = new[] { new Foo("bar") };

            var collection = new NamedCollection<Foo>(values, f => f.Name, defaultName: null);

            collection.DefaultName.Should().Be("default");
        }

        [Fact]
        public void ConstructorSetsDefaultNameToDefaultWhenEmptyString()
        {
            var values = new[] { new Foo("bar") };

            var collection = new NamedCollection<Foo>(values, f => f.Name, defaultName: "");

            collection.DefaultName.Should().Be("default");
        }

        [Fact]
        public void DefaultValuePropertyIsNullWhenNoValueHasADefaultName()
        {
            var values = new[] { new Foo("bar") };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.DefaultValue.Should().BeNull();
        }

        [Fact]
        public void DefaultValuePropertyIsTheValueWithANullName()
        {
            var defaultFoo = new Foo(null);
            var values = new[] { new Foo("bar"), defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.DefaultValue.Should().BeSameAs(defaultFoo);
        }

        [Fact]
        public void DefaultValuePropertyIsTheValueWithAnEmptyName()
        {
            var defaultFoo = new Foo("");
            var values = new[] { new Foo("bar"), defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.DefaultValue.Should().BeSameAs(defaultFoo);
        }

        [Fact]
        public void DefaultValuePropertyIsTheValueWithANameEqualToDefaultNameProperty()
        {
            var defaultFoo = new Foo("default");
            var values = new[] { new Foo("bar"), defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.DefaultValue.Should().BeSameAs(defaultFoo);
        }

        [Fact]
        public void NamedValuesPropertyContainsTheValuesWithANonDefaultName()
        {
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { barFoo, bazFoo, new Foo("default") };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.NamedValues.Should().BeEquivalentTo(new[] { barFoo, bazFoo });
        }

        [Fact]
        public void CountPropertyReturnsTheNumberOfNamedValuesWhenDefaultValueIsNull()
        {
            var values = new[] { new Foo("bar"), new Foo("baz") };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.Count.Should().Be(2);
        }

        [Fact]
        public void CountPropertyReturnsTheNumberOfNamedValuesPlusOneWhenDefaultValueIsNotNull()
        {
            var values = new[] { new Foo("bar"), new Foo("baz"), new Foo("default") };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.Count.Should().Be(3);
        }

        [Fact]
        public void IEnumerableGetEnumeratorMethodReturnsTheDefaultValueThenTheNamedValues()
        {
            Foo barFoo = new Foo("bar");
            Foo defaultFoo = new Foo("default");

            var values = new[] { barFoo, defaultFoo };

            IEnumerable collection = new NamedCollection<Foo>(values, f => f.Name);

            var enumerator = collection.GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeSameAs(defaultFoo);
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeSameAs(barFoo);
            enumerator.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void IEnumerableGetEnumeratorMethodReturnsJustTheNamedValuesIfDefaultValueIsNull()
        {
            Foo barFoo = new Foo("bar");

            var values = new[] { barFoo };

            IEnumerable collection = new NamedCollection<Foo>(values, f => f.Name);

            var enumerator = collection.GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeSameAs(barFoo);
            enumerator.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void GetEnumeratorMethodReturnsTheDefaultValueThenTheNamedValues()
        {
            Foo barFoo = new Foo("bar");
            Foo defaultFoo = new Foo("default");

            var values = new[] { barFoo, defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            var enumerator = collection.GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeSameAs(defaultFoo);
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeSameAs(barFoo);
            enumerator.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void GetEnumeratorMethodReturnsJustTheNamedValuesIfDefaultValueIsNull()
        {
            Foo barFoo = new Foo("bar");

            var values = new[] { barFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            var enumerator = collection.GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Should().BeSameAs(barFoo);
            enumerator.MoveNext().Should().BeFalse();
        }

        [Theory]
        [InlineData("default")]
        [InlineData("")]
        [InlineData(null)]
        public void IReadOnlyDictionaryKeysPropertyReturnsTheNamesOfTheValuesOfTheCollection(string defaultValueName)
        {
            Foo barFoo = new Foo("bar");
            Foo defaultFoo = new Foo(defaultValueName);

            var values = new[] { barFoo, defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);
            IReadOnlyDictionary<string, Foo> dictionary = collection;

            dictionary.Keys.Should().Equal(new[] { "default", "bar" });
        }

        [Fact]
        public void IReadOnlyDictionaryValuesPropertyReturnsTheNamedCollection()
        {
            Foo barFoo = new Foo("bar");
            Foo defaultFoo = new Foo("default");

            var values = new[] { barFoo, defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);
            IReadOnlyDictionary<string, Foo> dictionary = collection;

            dictionary.Values.Should().BeSameAs(collection);
        }

        [Theory]
        [InlineData("default")]
        [InlineData("")]
        [InlineData(null)]
        public void IReadOnlyDictionaryGetEnumeratorReturnsTheExpectedKeyValuePairs(string defaultValueName)
        {
            Foo barFoo = new Foo("bar");
            Foo defaultFoo = new Foo(defaultValueName);

            var values = new[] { barFoo, defaultFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);
            IReadOnlyDictionary<string, Foo> dictionary = collection;

            var enumerator = dictionary.GetEnumerator();

            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Key.Should().Be("default");
            enumerator.Current.Value.Should().BeSameAs(defaultFoo);
            enumerator.MoveNext().Should().BeTrue();
            enumerator.Current.Key.Should().Be("bar");
            enumerator.Current.Value.Should().BeSameAs(barFoo);
            enumerator.MoveNext().Should().BeFalse();
        }

        [Theory]
        [InlineData("default")]
        [InlineData(null)]
        [InlineData("")]
        public void TryGetValueMethodRetrievesTheDefaultValueWhenGivenADefaultName(string name)
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.TryGetValue(name, out var value).Should().Be(true);
            value.Should().BeSameAs(defaultFoo);
        }

        [Fact]
        public void TryGetValueMethodRetrievesANonDefaultValueWhenGivenAMatchingName()
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.TryGetValue("bar", out var value).Should().Be(true);
            value.Should().BeSameAs(barFoo);

            collection.TryGetValue("baz", out value).Should().Be(true);
            value.Should().BeSameAs(bazFoo);
        }

        [Fact]
        public void TryGetValueMethodReturnsFalseWhenGivenADefaultNameAndThereIsNoDefaultValue()
        {
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            foreach (var defaultName in new[] { "default", null, "" })
            {
                collection.TryGetValue(defaultName, out var value).Should().Be(false);
            }
        }

        [Fact]
        public void TryGetValueMethodReturnsFalseWhenGivenANonDefaultNameThatIsNotFound()
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.TryGetValue("qux", out var value).Should().Be(false);
        }

        [Theory]
        [InlineData("default")]
        [InlineData(null)]
        [InlineData("")]
        public void IndexerRetrievesTheDefaultValueWhenGivenADefaultName(string name)
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            var value = collection[name];
            value.Should().BeSameAs(defaultFoo);
        }

        [Fact]
        public void IndexerRetrievesANonDefaultValueWhenGivenAMatchingName()
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            var value = collection["bar"];
            value.Should().BeSameAs(barFoo);

            value = collection["baz"];
            value.Should().BeSameAs(bazFoo);
        }

        [Fact]
        public void IndexerThrowsWhenGivenADefaultNameAndThereIsNoDefaultValue()
        {
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            foreach (var defaultName in new[] { "default", null, "" })
            {
                Action action = () => { var dummy = collection[defaultName]; };
                action.Should().ThrowExactly<KeyNotFoundException>()
                    .WithMessage("The named collection does not have a default value.");
            }
        }

        [Fact]
        public void IndexerThrowsWhenGivenANonDefaultNameThatIsNotFound()
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            Action action = () => { var dummy = collection["qux"]; };
            action.Should().ThrowExactly<KeyNotFoundException>()
                .WithMessage("The given name was not present in the named collection: qux.");
        }

        [Theory]
        [InlineData("default")]
        [InlineData(null)]
        [InlineData("")]
        public void ContainsMethodReturnsTrueWhenGivenADefaultNameAndThereIsADefaultValue(string name)
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.Contains(name).Should().Be(true);
        }

        [Fact]
        public void ContainsMethodReturnsTrueWhenGivenAMatchingName()
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.Contains("bar").Should().Be(true);

            collection.Contains("baz").Should().Be(true);
        }

        [Fact]
        public void ContainsMethodReturnsFalseWhenGivenADefaultNameAndThereIsNoDefaultValue()
        {
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            foreach (var defaultName in new[] { "default", null, "" })
            {
                collection.Contains(defaultName).Should().Be(false);
            }
        }

        [Fact]
        public void ContainsMethodReturnsFalseWhenGivenANonDefaultNameThatIsNotFound()
        {
            var defaultFoo = new Foo("default");
            var barFoo = new Foo("bar");
            var bazFoo = new Foo("baz");
            var values = new[] { defaultFoo, barFoo, bazFoo };

            var collection = new NamedCollection<Foo>(values, f => f.Name);

            collection.Contains("qux").Should().Be(false);
        }

        [Fact]
        public void IsDefaultNameMethodReturnsTrueGivenNull()
        {
            var collection = new NamedCollection<Foo>(Enumerable.Empty<Foo>(), f => f.Name);

            collection.IsDefaultName(null).Should().BeTrue();
        }

        [Fact]
        public void IsDefaultNameMethodReturnsTrueGivenEmptyString()
        {
            var collection = new NamedCollection<Foo>(Enumerable.Empty<Foo>(), f => f.Name);

            collection.IsDefaultName("").Should().BeTrue();
        }

        [Fact]
        public void IsDefaultNameMethodReturnsTrueGivenDefaultName()
        {
            var collection = new NamedCollection<Foo>(Enumerable.Empty<Foo>(), f => f.Name);

            collection.IsDefaultName("default").Should().BeTrue();
        }

        [Fact]
        public void IsDefaultNameMethodeReturnsFalseGivenAnyOtherString()
        {
            var collection = new NamedCollection<Foo>(Enumerable.Empty<Foo>(), f => f.Name);

            collection.IsDefaultName("literally anything else").Should().BeFalse();
        }

        public class Foo
        {
            public Foo(string name) => Name = name;
            public string Name { get; }
        }
    }
}
