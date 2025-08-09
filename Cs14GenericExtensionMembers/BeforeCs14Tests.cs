using System.Numerics;
using System.Reflection;
using Shouldly;

namespace Cs14GenericExtensionMembers.Before;

public static class ObjectExtensions
{
    public static TAttribute? GetCustomAttributes<TAttribute>(this object receiver)
        where TAttribute : Attribute
    {
        return receiver.GetType().GetCustomAttribute<TAttribute>();
    }
}

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereWithinRange<T>(this IEnumerable<T> receiver, T min, T max)
        where T : INumber<T>
    {
        return receiver.Where(item => item >= min && item <= max);
    }

    public static IEnumerable<TItem> WhereWithinRangeBy<TItem, TValue>(
        this IEnumerable<TItem> receiver,
        Func<TItem, TValue> projection,
        TValue min,
        TValue max
    )
        where TValue : INumber<TValue>
    {
        return receiver.Where(item => projection(item) >= min && projection(item) <= max);
    }
}

public static class StringExtensions
{
    public static TEnum ParseAsEnum<TEnum>(this string receiver)
        where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>(receiver);
    }
}

public class BeforeCs14Tests
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
    public void ParsesEnumWithoutExtension()
    {
        Enum.Parse<DayOfWeek>("Monday").ShouldBe(DayOfWeek.Monday);
    }

    [Test]
    public void ParsesEnumWithExtension()
    {
        "Monday".ParseAsEnum<DayOfWeek>().ShouldBe(DayOfWeek.Monday);
    }
}
