namespace Tolk.FunctionApp.Tests;

public static class MockEvents
{
    public const string PhraseCreatedEvent = """
        {
            "Type": "PhraseCreatedEvent",
            "Name": "New Phrase",
            "id": "00000000-0000-0000-0000-000000000000"
        }
        """;

    public const string ProjectCreatedEvent = """
        {
            "Type": "ProjectCreatedEvent",
            "Name": "New Project",
            "Version": 0,
            "id": "00000000-0000-0000-0000-000000000000"
        }
        """;

    public const string TranslationUpdatedEvent = """
        {
            "Type": "TranslationUpdatedEvent",
            "PhraseKey": "New Phrase",
            "Locale": "nl_NL",
            "Value": "Nieuwe zin",
            "id": "00000000-0000-0000-0000-000000000000"
        }
        """;
}
