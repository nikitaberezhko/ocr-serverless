# Актуальное состояние CI/CD для публикации в GitHub Packages

Этот документ описывает текущую реализованную конфигурацию публикации NuGet-пакета проекта `OcrFunction` в GitHub Packages.

## Что уже настроено

1. Публикуется NuGet-пакет из проекта `OcrFunction`.
2. Триггер публикации: `push` тега формата `v*` (например, `v0.0.1`).
3. Версия пакета берется из `OcrFunction.csproj` (`<Version>`), а не из тега.
4. Аутентификация публикации выполняется через `GITHUB_TOKEN`.
5. Подключен SourceLink (`Microsoft.SourceLink.GitHub`), symbol packages (`.snupkg`) не публикуются.
6. Проект и пайплайн работают на .NET 8 (`net8.0` и `8.0.x` SDK в workflow).

## Где находится workflow

- `.github/workflows/publish-github-packages.yml`

## Логика workflow

1. Checkout репозитория (`actions/checkout@v5`).
2. Установка .NET SDK (`actions/setup-dotnet@v4`) с версией `8.0.x`.
3. Восстановление зависимостей:
   - `dotnet restore "OcrFunction/OcrFunction.csproj"`.
4. Сборка в `Release`:
   - `dotnet build "OcrFunction/OcrFunction.csproj" -c Release --no-restore`.
5. Упаковка NuGet-пакета:
   - `dotnet pack "OcrFunction/OcrFunction.csproj" -c Release --no-build -o "./artifacts"`.
6. Сохранение `.nupkg` как артефакта job (`actions/upload-artifact@v4`).
7. Публикация в GitHub Packages:
   - `dotnet nuget push "./artifacts/*.nupkg" --api-key "${{ secrets.GITHUB_TOKEN }}" --source "https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json" --skip-duplicate`.

## Права токена в workflow

- `contents: read`
- `packages: write`

Это минимальный набор прав для чтения репозитория и публикации пакета.

## Текущие параметры пакета в csproj

В `OcrFunction/OcrFunction.csproj` заданы:

- `TargetFramework = net8.0`
- `IsPackable = true`
- `PackageId = OcrFunction`
- `Version = 0.0.1`
- `Authors = nikitaberezhko`
- `Description`, `RepositoryUrl`, `RepositoryType`
- `PublishRepositoryUrl = true`, `EmbedUntrackedSources = true`, `ContinuousIntegrationBuild = true`

## Как запустить публикацию

1. Обновить `<Version>` в `OcrFunction.csproj` (если нужен новый релиз).
2. Закоммитить изменения и отправить в `main`.
3. Создать и запушить тег `v*`:
   - `git tag v0.0.1`
   - `git push origin v0.0.1`
4. Проверить выполнение workflow в GitHub Actions.

## Примечание

Используется правило `--skip-duplicate`: если пакет с такой версией уже есть в registry, шаг публикации не завалит pipeline.
