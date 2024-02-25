using QuickNote.Core.IntegrationTests.Helper;
using QuickNote.Core.Storage;
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
}