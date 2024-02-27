using QuickNote.Core.IntegrationTests.Helper;
using QuickNote.Core.Storage;
using System.Text;

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

    [Fact]
    public async Task ReturnsSingleTodosFromMultipleFiles()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[X] {Todo1}");
                return sb.ToString();
            })
            .WithDateTimeFile(DateTime.Now.AddDays(-1).Date, () =>
            {
                StringBuilder sb = new StringBuilder();
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

        IEnumerable<TodoCollection<Todo>> result = context
            .GetAllAsync<Todo>()
            .ToBlockingEnumerable();

        Assert.Equal(2, result.Count());

        TodoCollection<Todo> first = result.First();

        Assert.Equal(DateTime.Now.AddDays(-1).Date, first.Identifier);
        Assert.Single(first.Todos);

        Todo firstTodo = first.Todos.First();
        Assert.False(firstTodo.IsFinished);
        Assert.Equal("Todo2", firstTodo.Name);

        TodoCollection<Todo> last = result.Last();

        Assert.Equal(DateTime.Now.Date, last.Identifier);
        Assert.Single(last.Todos);

        Todo lastTodo = last.Todos.First();
        Assert.True(lastTodo.IsFinished);
        Assert.Equal("Todo1", lastTodo.Name);

    }

    [Fact]
    public async Task ReturnsMultipleTodosFromMultipleFiles()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[X] {Todo1}");
                sb.AppendLine("[] {Todo3}");
                return sb.ToString();
            })
            .WithDateTimeFile(DateTime.Now.AddDays(-1).Date, () =>
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[] {Todo2}");
                sb.AppendLine("[X] {Testing}");
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

        IEnumerable<TodoCollection<Todo>> result = context
            .GetAllAsync<Todo>()
            .ToBlockingEnumerable();

        Assert.Equal(2, result.Count());

        TodoCollection<Todo> first = result.First();

        Assert.Equal(DateTime.Now.AddDays(-1).Date, first.Identifier);
        Assert.Equal(2, first.Todos.Count());

        Todo firstTodo1 = first.Todos.First();
        Assert.False(firstTodo1.IsFinished);
        Assert.Equal("Todo2", firstTodo1.Name);

        Todo lastTodo1 = first.Todos.Last();
        Assert.True(lastTodo1.IsFinished);
        Assert.Equal("Testing", lastTodo1.Name);

        TodoCollection<Todo> last = result.Last();

        Assert.Equal(DateTime.Now.Date, last.Identifier);
        Assert.Equal(2, last.Todos.Count());

        Todo firstTodo2 = last.Todos.First();
        Assert.True(firstTodo2.IsFinished);
        Assert.Equal("Todo1", firstTodo2.Name);

        Todo lastTodo2 = last.Todos.Last();
        Assert.False(lastTodo2.IsFinished);
        Assert.Equal("Todo3", lastTodo2.Name);
    }

    [Fact]
    public async Task UpdateAddsNewTodoEntry()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
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

        List<Todo> todos = new List<Todo>() { new Todo() { IsFinished = true, Name = "Todo1" } };

        await context.UpdateOrCreateAsync(DateTime.Now, todos);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Single(result);

        Todo first = result.First();

        Assert.True(first.IsFinished);
        Assert.Equal("Todo1", first.Name);
    }

    [Fact]
    public async Task UpdateAddsNewTodoEntries()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
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

        List<Todo> todos = new List<Todo>() { 
            new Todo() { IsFinished = true, Name = "Todo1" },
            new Todo() { IsFinished = false, Name = "Todo2" },
            new Todo() { IsFinished = true, Name = "Todo3" } };

        await context.UpdateOrCreateAsync(DateTime.Now, todos);

        IEnumerable<Todo> result = context
            .GetSingleAsync<Todo>(DateTime.Now)
            .ToBlockingEnumerable();

        Assert.Equal(3, result.Count());

        int count = 0;
        foreach (Todo todo in result)
        {
            count += 1;
            if (count % 2 == 0)
            {
                Assert.False(todo.IsFinished);
            }

            Assert.Equal($"Todo{count}", todo.Name);
        }
    }

    [Fact]
    public async Task DeleteReturnsFalseWhenFileDoesNotExist()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
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

        Assert.False(context.TryDelete(DateTime.Now.AddDays(-1)));
    }

    [Fact]
    public async Task DeleteRemovesFileFromFileSystem()
    {
        TestDirectoryBuilder builder = new();

        string basepath = builder
            .WithDateTimeFile(DateTime.Now, () =>
            {
                StringBuilder sb = new StringBuilder();
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

        Assert.True(context.TryDelete(DateTime.Now));
        Assert.False(File.Exists(Path.Combine(basepath, DateTime.Now.ToString("yyyy-MM-dd"))));
    }
}