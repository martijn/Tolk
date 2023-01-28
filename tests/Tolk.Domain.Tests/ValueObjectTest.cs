namespace Tolk.Domain.Tests;

public class ValueObjectTest
{

    [Fact]
    public void EqualTest()
    {
        var collection1 = Phrase.Create("Foo");
        var collection2 = Phrase.Create("Foo");
        
        Assert.Equal(collection1, collection2);
    }
    
    [Fact]
    public void NotEqualTest()
    {
        var collection1 = Phrase.Create("Foo");
        var collection2 = Phrase.Create("Bar");
        
        Assert.NotEqual(collection1, collection2);
    }
}
