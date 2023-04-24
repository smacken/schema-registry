// <copyright file="SchemaType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SchemaRegistry
{
    /// <summary>
    /// Different types of schemas available to validate.
    /// </summary>
    public enum SchemaType
    {
        Json,
        Avro,
        Xml,
        Parquet,
        Unknown
    }
}