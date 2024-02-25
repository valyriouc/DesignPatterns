using QuickNote.Core.Storage;

namespace QuickNote.Core.IntegrationTests;

public class MarkdownReaderTests
{
    public MarkdownReader reader;

    public MarkdownReaderTests()
    {
        reader = new MdReaderBuilder()
            .WithCheck()
            .WithName()
            .Build();
    }

    [Fact]
    public void ReadsTodoCorrectWithChecked()
    {
        IEnumerable<MarkdownNode> result = reader.Read("[X] {hello}");

        Assert.Equal(3, result.Count());

        foreach (MarkdownNode node in result)
        {
            if (node.Identifier == MdSyntax.Check)
            {
                node.TryConvert<bool>(out bool isChecked);
                Assert.True(isChecked);
            }
            if (node.Identifier == MdSyntax.Name)
            {
                node.TryConvert<string>(out string name);
                Assert.Equal("hello", name);
            }
        }

        MarkdownNode last = result.Last();
        Assert.Equal(MdSyntax.Finished, last.Identifier);
    }

    [Fact]
    public void ReadsTodoCorrectWithNotChecked()
    {
        IEnumerable<MarkdownNode> result = reader.Read("[] {hello}");

        Assert.Equal(3, result.Count());

        foreach (MarkdownNode node in result)
        {
            if (node.Identifier == MdSyntax.Check)
            {
                node.TryConvert<bool>(out bool isChecked);
                Assert.False(isChecked);
            }
            if (node.Identifier == MdSyntax.Name)
            {
                node.TryConvert<string>(out string name);
                Assert.Equal("hello", name);
            }
        }

        MarkdownNode last = result.Last();
        Assert.Equal(MdSyntax.Finished, last.Identifier);
    }

    [Fact]
    public void ReadsTodoWithAppointmentCorrect()
    {
        reader = new MdReaderBuilder()
             .WithCheck()
             .WithName()
             .WithAppointment()
             .Build();

        IEnumerable<MarkdownNode> result = reader.Read("[X] {nice} {true}");

        Assert.Equal(4, result.Count());


        foreach (MarkdownNode node in result)
        {
            if (node.Identifier == MdSyntax.Check)
            {
                node.TryConvert<bool>(out bool isChecked);
                Assert.True(isChecked);
            }
            if (node.Identifier == MdSyntax.Name)
            {
                node.TryConvert<string>(out string name);
                Assert.Equal("nice", name);
            }
            if (node.Identifier == MdSyntax.Appointment)
            {
                node.TryConvert(out bool isAppointment);
                Assert.True(isAppointment);
            }
        }

        MarkdownNode last = result.Last();
        Assert.Equal(MdSyntax.Finished, last.Identifier);
    }

    [Fact]
    public void ReadsTodoWithAppointmentCorrectWithFalseAppointment()
    {
        reader = new MdReaderBuilder()
             .WithCheck()
             .WithName()
             .WithAppointment()
             .Build();

        IEnumerable<MarkdownNode> result = reader.Read("[] {nice} {false}");

        Assert.Equal(4, result.Count());


        foreach (MarkdownNode node in result)
        {
            if (node.Identifier == MdSyntax.Check)
            {
                node.TryConvert<bool>(out bool isChecked);
                Assert.False(isChecked);
            }
            if (node.Identifier == MdSyntax.Name)
            {
                node.TryConvert<string>(out string name);
                Assert.Equal("nice", name);
            }
            if (node.Identifier == MdSyntax.Appointment)
            {
                node.TryConvert(out bool isAppointment);
                Assert.False(isAppointment);
            }
        }

        MarkdownNode last = result.Last();
        Assert.Equal(MdSyntax.Finished, last.Identifier);
    }
}
