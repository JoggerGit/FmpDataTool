﻿using System.Text.Json.Serialization;

namespace FmpDataTool.Model
{
    /// <summary>
    /// Stock
    /// </summary>
    public class Stock
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; }
    }
}
