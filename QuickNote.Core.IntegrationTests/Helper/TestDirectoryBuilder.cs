using System;
using System.IO;

namespace QuickNote.Core.IntegrationTests.Helper;

internal class TestDirectoryBuilder {

    private HashSet<string> TodoFiles { get; set; }

    public string Basepath => Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString());

    public TestDirectoryBuilder() {
        TodoFiles = new HashSet<string>();
    }

    public TestDirectoryBuilder WithTodoFile(string name) {
        if (string.IsNullOrWhiteSpace(name)) {
            throw new ArgumentNullException(nameof(name));
        }

        TodoFiles.Add(name);
        return this;
    }

    public string Build() {
        foreach (string file in TodoFiles) {
            string filepath = Path.Combine(Basepath, $"{file}.md");
            File.Create(filepath);
        }

        return Basepath;
    }   
}