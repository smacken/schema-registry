# SchemaValidation Project

SchemaValidation is a C# project that provides schema validation services. The project includes two main classes, the `Registry` class, and the `SchemaRegistryConfiguration` class.

## Registry Class

The `Registry` class implements the `IRegistry` interface and provides schema validation services. The constructor of the `Registry` class accepts an instance of `SchemaRegistryConfiguration` that provides the configuration details for the registry.

The `Registry` class implements two methods:

### RegisterAsync

The `RegisterAsync` method is used to register a schema with the registry. The method accepts an instance of `ISchema` and saves it to the underlying data store.

### ValidateAsync

The `ValidateAsync` method is used to validate a schema against a stream of data. The method accepts a stream of data, the subject of the schema, an optional label, and an optional version. The method first detects the schema type from the input stream, then retrieves the schema from the data store based on the subject, label, and version. If the schema is found, the method validates the input stream against the retrieved schema.

## SchemaRegistryConfiguration Class

The `SchemaRegistryConfiguration` class is used to configure the registry. The class provides methods to add validators and detectors for different schema types. The class also provides a method to configure the data store for the registry.

## Getting Started

To use the SchemaValidation project, create an instance of the `SchemaRegistryConfiguration` class and configure it using the provided methods. Then create an instance of the `Registry` class and pass the configuration to the constructor. You can then use the `RegisterAsync` and `ValidateAsync` methods to register schemas and validate input streams.

```csharp
var config = new SchemaRegistryConfiguration()
    .WithDataStore(new InMemoryDataStore())
    .WithJson();

var registry = new Registry(config);

var schema = new Schema("subject", "schema");
await registry.RegisterAsync(schema);

var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("{\"name\":\"John\"}"));
var result = await registry.ValidateAsync(inputStream, "subject");

Console.WriteLine(result.IsValid); // output: true
```

## Configuration

To configure the `SchemaValidation` library, you can use the `SchemaRegistryConfiguration` class. 

### Adding Detectors and Validators

To add a detector and a validator for a new schema type, you can use the `AddDetector` and `AddValidator` methods, respectively. For example, to add a detector and a validator for JSON schema:

```csharp
var config = new SchemaRegistryConfiguration()
    .AddDetector(new JsonStreamDetector())
    .AddValidator(SchemaType.Json, new JsonSchemaValidator());
```

You can also add detectors and validators for XML schema using the `WithXml()` method:

```csharp
var config = new SchemaRegistryConfiguration()
    .WithXml();
```

### Configuring a Data Store

To configure a data store for the schema registry, use the `WithDataStore` method:

```csharp
var config = new SchemaRegistryConfiguration()
    .WithDataStore(new MyDataStore());
```

This method takes an instance of `IDataStore` as a parameter. 

Once you have created a `SchemaRegistryConfiguration` object, you can pass it to the `Registry` constructor:

```csharp
var registry = new Registry(config);
```

To configure the SchemaValidation project, you can use the `SchemaRegistryConfiguration` class. The configuration class allows you to set up the data store and the validators for different schema types. 

To configure the project to handle Avro files, you can use the `WithAvro` method, which adds the Avro detector and validator to the configuration:

```csharp
public static SchemaRegistryConfiguration WithAvro(this SchemaRegistryConfiguration config)
{
    config.AddDetector(new AvroStreamDetector());
    config.AddValidator(SchemaType.Avro, new AvroSchemaValidator());
    return config;
}
```

Similarly, to configure the project to handle Parquet files, you can use the `WithParquet` method, which adds the Parquet detector and validator to the configuration:

```csharp
public static SchemaRegistryConfiguration WithParquet(this SchemaRegistryConfiguration config)
{
    config.AddDetector(new ParquetStreamDetector());
    config.AddValidator(SchemaType.Parquet, new ParquetSchemaValidator());
    return config;
}
```

To configure the project with these options, you can chain the configuration methods together:

```csharp
var config = new SchemaRegistryConfiguration()
    .WithDataStore(new MyDataStore())
    .WithJson()
    .WithAvro()
    .WithXml()
    .WithParquet();
```

To configure the SchemaValidation with RedisDataStore for storing JSON schemas, you can use the following code:

```csharp
var config = new SchemaRegistryConfiguration().WithJson().WithDataStore(new RedisDataStore());
```

This creates a new `SchemaRegistryConfiguration` instance, adds the JSON detector and validator using `WithJson()`, and sets the data store to use RedisDataStore using `WithDataStore(new RedisDataStore())`. 

Similarly, you can use the following code to configure the SchemaValidation with RedisDataStore for storing Avro or Parquet schemas:

```csharp
var config = new SchemaRegistryConfiguration().WithAvro().WithDataStore(new RedisDataStore());
var config = new SchemaRegistryConfiguration().WithParquet().WithDataStore(new RedisDataStore());
```

Note that you will need to have a Redis instance running on `localhost` to use `RedisDataStore`. You can customize the connection details as required.:w


## Contributing

Contributions to the SchemaValidation project are welcome. To contribute, fork the repository and create a pull request. Please make sure that your code adheres to the project's coding standards and passes all tests before submitting a pull request.

