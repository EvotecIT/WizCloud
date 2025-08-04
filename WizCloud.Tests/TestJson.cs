using System.Text.Json;
using System.Text.Json.Serialization;

namespace WizCloud.Tests;

internal static class TestJson {
    public static readonly JsonSerializerOptions Options = new() {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
}
