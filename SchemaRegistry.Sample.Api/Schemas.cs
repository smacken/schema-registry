using SchemaRegistry;

public static class Schemas
{
    public static ISchema ProductSchema = new ValidationSchema
    {
        Subject = "/api/products",
        Version = "1.0.0",
        Schema = @"{
                ""$schema"": ""http://json-schema.org/draft-07/schema#"",
                ""type"": ""object"",
                ""properties"": {
                    ""id"": {
                        ""type"": ""integer""
                    },
                    ""name"": {
                        ""type"": ""string""
                    },
                    ""price"": {
                        ""type"": ""number"",
                        ""minimum"": 0,
                        ""exclusiveMinimum"": true
                    }
                },
                ""required"": [
                    ""id"",
                    ""name"",
                    ""price""
                ]
            }"
    };
}