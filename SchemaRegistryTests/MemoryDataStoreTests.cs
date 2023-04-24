using FluentAssertions;
using SchemaRegistry;

namespace SchemaRegistryTests
{
    public class MemoryDataStoreTests
    {
        //unit test MemoryDataStore.UpsertAsync()
        [Fact]
        public void UpsertAsync()
        {
            MemoryDataStore? store = new MemoryDataStore();
            store.UpsertAsync(new ValidationSchema { Subject = "subject", Schema = "schema" }).Wait();
            store.GetAsync("subject").Result.Schema.Should().Be("schema");
        }

        //unit test MemoryDataStore.GetAsync()
        [Fact]
        public void GetAsync()
        {
            MemoryDataStore? store = new MemoryDataStore();
            store.UpsertAsync(new ValidationSchema { Subject = "subject", Schema = "schema" }).Wait();
            store.GetAsync("subject").Result.Schema.Should().Be("schema");
        }

        //unit test MemoryDataStore.GetAsync() for non-existent subject
        [Fact]
        public void GetAsync_NonExistentSubject()
        {
            MemoryDataStore? store = new MemoryDataStore();
            store.GetAsync("subject").Result.Should().BeNull();
        }

        //unit test UpsertAsync() for duplicate subject
        [Fact]
        public void UpsertAsync_DuplicateSubject()
        {
            MemoryDataStore? store = new MemoryDataStore();
            store.UpsertAsync(new ValidationSchema { Subject = "subject", Schema = "schema" }).Wait();
            store.UpsertAsync(new ValidationSchema { Subject = "subject", Schema = "schema2" }).Wait();
            store.GetAsync("subject").Result.Schema.Should().Be("schema2");
        }
    }
}