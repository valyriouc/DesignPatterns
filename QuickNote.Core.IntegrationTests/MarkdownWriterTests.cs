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

        List<MarkdownNode> node = new List<MarkdownNode>() { new MarkdownNode(MdSyntax.Check, true.ToString()), new MarkdownNode(MdSyntax.Name, "Todo1") };

        string result = writer.Write(node);

        Assert.Equal("[X] {Todo1}", result);
    }
}
