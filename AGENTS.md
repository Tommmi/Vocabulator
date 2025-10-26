# Vocabulator - Agent Guide

## Commands
- Build: `dotnet build`
- Run: `dotnet run --project Vocabulator/Vocabulator.csproj`
- Test: No test projects found
- Clean: `dotnet clean`

## Architecture
C# .NET 8.0 console application with layered architecture:
- **Vocabulator**: Main console app (entry point: Program.cs)
- **Vocabulator.Domain.Services**: Business logic (VocabularyService, AiQuestionProcessor)
- **Vocabulator.Domain.Interface**: Domain interfaces (IVocabularyService, ICsvRepo)
- **Vocabulator.Common**: Shared models (Question, Vocable, Word, Sentence, QuestionTemplate)
- **Vocabulator.Infrastructure**: CSV file repository implementation
- **Vocabulator.OpenAi**: OpenAI integration for language processing

## Code Style
- Nullable enabled, ImplicitUsings enabled
- File-scoped namespaces (e.g., `namespace Vocabulator.Common;`)
- Tab indentation in existing code
- Private fields prefixed with `_`
- Constructor parameters use camelCase
- Use `var` for local variables when type is obvious
- German comments may appear in code
