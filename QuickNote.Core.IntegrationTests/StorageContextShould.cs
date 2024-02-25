using QuickNote.Core.IntegrationTests.Helper;
using QuickNote.Core.Storage;
using System.Text;
using Xunit.Sdk;

namespace QuickNote.Core.IntegrationTests;

public class StorageContextShould {

    [Fact]
    public async Task ReturnsNoneBecauseNoTodosExist() {

        TestDirectoryBuilder builder = new();

        string basepath = builder.Build();

        MdReaderBuilder rb = new MdReaderBuilder()
            .WithCheck()
            .WithName();

        MdWriterBuilder wb = new MdWriterBuilder()
            .WithCheck()
            .WithName();

        StorageContext context = new StorageContext(basepath, rb, wb);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ReturnsNoneBecauseNoTodosInExistingFile()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithTodoFile(DateTime.Now.ToString("yyyy-MM-dd"))
            .Build();

        MdReaderBuilder rb = new MdReaderBuilder()
            .WithCheck()
            .WithName();

        MdWriterBuilder wr = new MdWriterBuilder()
            .WithCheck()
            .WithName();

        StorageContext context = new StorageContext(basepath, rb, wr);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ReturnsOneTodoOfOneFileWithCorrectName()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[] {Todo1}");
                return sb.ToString();
            })
            .Build();

        MdReaderBuilder rb = new MdReaderBuilder()
            .WithCheck()
            .WithName();

        MdWriterBuilder wr = new MdWriterBuilder()
            .WithCheck()
            .WithName();

        StorageContext context = new StorageContext(basepath, rb, wr);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Single(result);

        Todo first = result.First();

        Assert.False(first.IsFinished);
        Assert.Equal("Todo1", first.Name);
    }

    [Fact]
    public async Task ReturnsOneTodoWithStateChecked()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[X] {Todo1}");
                return sb.ToString();
            })
            .Build();

        MdReaderBuilder rb = new MdReaderBuilder()
            .WithCheck()
            .WithName();

        MdWriterBuilder wr = new MdWriterBuilder()
            .WithCheck()
            .WithName();

        StorageContext context = new StorageContext(basepath, rb, wr);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Single(result);

        Todo first = result.First();

        Assert.True(first.IsFinished);
        Assert.Equal("Todo1", first.Name);
    }

    [Fact]
    public async Task ReturnsMultipleTodosWithCorrectData()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[X] {Todo1}");
                sb.AppendLine("[] {Todo2}");
                return sb.ToString();
            })
            .Build();

        MdReaderBuilder rb = new MdReaderBuilder()
            .WithCheck()
            .WithName();

        MdWriterBuilder wr = new MdWriterBuilder()
            .WithCheck()
            .WithName();

        StorageContext context = new StorageContext(basepath, rb, wr);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Equal(2, result.Count());

        Todo first = result.First();

        Assert.True(first.IsFinished);
        Assert.Equal("Todo1", first.Name);

        Todo last = result.Last();

        Assert.False(last.IsFinished);
        Assert.Equal("Todo2", last.Name);
    }
}