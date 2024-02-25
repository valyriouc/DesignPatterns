namespace QuickNote.Core.IntegrationTests.Helper;

internal class TestDirectoryBuilder {

    private Dictionary<string, Func<string>?> TodoFiles { get; set; }

    public string Basepath { get; init; } 

    public TestDirectoryBuilder() {
        TodoFiles = new Dictionary<string, Func<string>?>();
        Basepath = Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());
    }

    public TestDirectoryBuilder WithTodoFile(string name, Func<string>? contentProvider=null) {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentNullException(nameof(name));
        }

        TodoFiles.Add(name, contentProvider);
        return this;
    }

    public TestDirectoryBuilder WithDateTimeFile(DateTime datetime, Func<string>? contentProvider=null) => 
        WithTodoFile(datetime.ToString("yyyy-MM-dd"), contentProvider);


    public string Build() {
        Directory.CreateDirectory(Basepath);
        foreach (KeyValuePair<string, Func<string>?> file in TodoFiles) {
            string filepath = Path.Combine(Basepath, $"{file.Key}.md");
            File.WriteAllText(filepath, file.Value?.Invoke());
        }

        return Basepath;
    }   
}