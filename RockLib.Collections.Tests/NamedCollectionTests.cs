using FluentAssertions;
using System;
using System.Collections.Generic;
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

            action.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ConstructorThrowsWhenThereIsMoreThanOneValueWithTheSameName()
        {
            var values = new[] { new Foo("bar"), new Foo("bar") };
            Action action = () => new NamedCollection<Foo>(values, f => f.Name);
            action.Should().ThrowExactly<ArgumentException>();
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
                action.Should().ThrowExactly<KeyNotFoundException>();
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
            action.Should().ThrowExactly<KeyNotFoundException>();
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

        public class Foo
        {
            public Foo(string name) => Name = name;
            public string Name { get; }
        }
    }
}
