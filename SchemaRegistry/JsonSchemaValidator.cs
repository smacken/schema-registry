namespace SchemaRegistry
{
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NJsonSchema;
    using NJsonSchema.Validation;

    /// <summary>
    /// JsonSchemaValidator.
    /// </summary>
    public sealed class JsonSchemaValidator : ISchemaValidator
    {
        /// <inheritdoc/>
        public async Task<ValidationResult> ValidateAsync(Stream schema, string knownSchema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (knownSchema == null)
            {
                throw new ArgumentNullException(nameof(knownSchema));
            }

            const int bufferSize = 4096;
            JsonSchema? schemaJson = await JsonSchema.FromJsonAsync(knownSchema);
            long length = schema.CanSeek ? schema.Length : -1;
            if (length <= 0 || length > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(schema));
            }

            int size = (int)length;
            if (size < bufferSize)
            {
                return await ValidateWithFewerAllocations(schema, schemaJson);
            }
            else
            {
                return await ValidateWithParallelProcessing(schema, schemaJson, size);
            }
        }

        private async Task<ValidationResult> ValidateWithFewerAllocations(Stream schema, JsonSchema schemaJson)
        {
            List<ValidationError>? errors = new ();
            byte[]? buffer = ArrayPool<byte>.Shared.Rent(4096);
            Decoder? decoder = Encoding.UTF8.GetDecoder();
            StringBuilder? sb = new ();
            if (schema.Position > 0)
            {
                schema.Position = 0;
            }

            try
            {
                int bytesRead = 0;
                do
                {
                    bytesRead = await schema.ReadAsync(buffer, 0, buffer.Length);
                    char[]? chars = new char[decoder.GetCharCount(buffer, 0, bytesRead)];
                    decoder.GetChars(buffer, 0, bytesRead, chars, 0);
                    sb.Append(chars);
                }
                while (bytesRead > 0);

                ICollection<ValidationError>? result = schemaJson.Validate(sb.ToString());
                errors.AddRange(result);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return new ValidationResult
            {
                Message = string.Join(", ", errors.Select(error => $"{error.Path}: {error.Kind}")),
                IsValid = errors.Count == 0,
            };
        }

        private async Task<ValidationResult> ValidateWithParallelProcessing(Stream schema, JsonSchema schemaJson,
            int bufferSize)
        {
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            ConcurrentBag<ValidationError>? errors = new ();
            Decoder? decoder = Encoding.UTF8.GetDecoder();
            byte[]? buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            List<Task>? tasks = new ();
            if (schema.Position > 0)
            {
                schema.Position = 0;
            }

            try
            {
                while (true)
                {
                    int bytesRead = await schema.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    byte[]? chunk = new byte[bytesRead];
                    Array.Copy(buffer, chunk, bytesRead);

                    Task? task = Task.Run(() =>
                    {
                        char[]? chars = new char[decoder.GetCharCount(chunk, 0, chunk.Length)];
                        decoder.GetChars(chunk, 0, chunk.Length, chars, 0);
                        ICollection<ValidationError>? result = schemaJson.Validate(new string(chars));
                        foreach (ValidationError? error in result)
                        {
                            errors.Add(error);
                        }
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return new ValidationResult
            {
                Message = string.Join(", ", errors.Select(error => $"{error.Path}: {error.Kind}")),
                IsValid = errors.Count == 0
            };
        }
    }
}