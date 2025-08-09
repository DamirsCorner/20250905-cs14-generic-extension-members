using Shouldly;
using System.Numerics;
using System.Reflection;

namespace Cs14GenericExtensionMembers.After;

public static class ObjectExtensions
{
    extension(object receiver)
    {
        public TAttribute? GetCustomAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return receiver.GetType().GetCustomAttribute<TAttribute>();
        }
    }
}

public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> receiver) where T : INumber<T>
    {
        public IEnumerable<T> WhereWithinRange(T min, T max)
        {
            return receiver.Where(item => item >= min && item <= max);
        }
    }

    extension<TItem>(IEnumerable<TItem> receiver)
    {
        public IEnumerable<TItem> WhereWithinRangeBy<TValue>(
            Func<TItem, TValue> projection,
            TValue min,
            TValue max
        )
            where TValue : INumber<TValue>
        {
            return receiver.Where(item => projection(item) >= min && projection(item) <= max);
        }
    }
}

public static class EnumExtensions
{
    extension<TEnum>(TEnum)
        where TEnum : struct, Enum
    {
        public static TEnum Parse(string value)
        {
            return Enum.Parse<TEnum>(value);
        }
    }
}

public class WithCs14Tests
{
    [Obsolete("Made up for testing.")]
    public class ObsoleteClass { }

    [Test]
    public void GetsMessageFromObsoleteAttribute()
    {
        ObsoleteClass obsolete = new();

        obsolete.GetCustomAttributes<ObsoleteAttribute>()?.Message.ShouldBe("Made up for testing.");
    }

    [Test]
    public void ReturnsItemsWithinRange()
    {
        Enumerable.Range(1, 5).WhereWithinRange(2, 4).ShouldBe(Enumerable.Range(2, 3));
    }

    [Test]
    public void ReturnsItemsWithinValueWithinRange()
    {
        var collection = Enumerable.Range(1, 5).Select(value => (value, $"Item {value}")).ToArray();

        collection
            .WhereWithinRangeBy(((int value, string _) item) => item.value, 2, 4)
            .ShouldBe(collection[1..4]);
    }

    [Test]
    public void ParsesEnum()
    {
        DayOfWeek.Parse("Monday").ShouldBe(DayOfWeek.Monday);
    }

}
