using QuickNote.Core.Storage;
using System.Text.Json.Serialization;

namespace QuickNote.Core.IntegrationTests;

public class MarkdownWriterTests
{
    [Fact]
    public async Task WritesSingleTodo()
    {
        MarkdownWriter writer = new MdWriterBuilder()
            .WithCheck()
            .WithName()
            .Build();

        List<MarkdownNode> node = new List<MarkdownNode>() { 
            new MarkdownNode(MdSyntax.Check, true.ToString()), 
            new MarkdownNode(MdSyntax.Name, "Todo1") };

        string result = writer.Write(node);

        Assert.Equal("[X] {Todo1}", result);
    }

    [Fact]
    public async Task WriteSingleTodoWithAppointment()
    {
        MarkdownWriter writer = new MdWriterBuilder()
            .WithCheck()
            .WithName()
            .WithAppointment()
            .Build();

        List<MarkdownNode> nodes = new List<MarkdownNode>()
        {
            new MarkdownNode(MdSyntax.Check, true.ToString()),
            new MarkdownNode(MdSyntax.Name, "Todo1"),
            new MarkdownNode(MdSyntax.Appointment, false.ToString())
        };

        string result = writer.Write(nodes);

        Assert.Equal("[X] {Todo1} {False}", result);
    }

    [Fact]
    public async Task WriteSingleTodoWithAllOptions()
    {
        MarkdownWriter writer = new MdWriterBuilder()
            .WithCheck()
            .WithName()
            .WithAppointment()
            .WithEndDate()
            .Build();

        List<MarkdownNode> nodes = new List<MarkdownNode>()
        {
            new MarkdownNode(MdSyntax.Check, true.ToString()),
            new MarkdownNode(MdSyntax.Name, "Todo1"),
            new MarkdownNode(MdSyntax.Appointment, false.ToString()),
            new MarkdownNode(MdSyntax.Enddate, new DateTime(2022, 8, 1, 8,0,0)
            .ToString("yyyy-MM-ddThh-mm-ss.fff"))
        };

        string result = writer.Write(nodes);

        Assert.Equal("[X] {Todo1} {False} {2022-08-01T08:00:00}", result);
    }
}
