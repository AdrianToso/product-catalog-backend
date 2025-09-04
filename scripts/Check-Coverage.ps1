# Hook de Pre-Commit para validar la cobertura de código por capas y generar un informe HTML.

param (
    # Agrega este parámetro para abrir el informe en el navegador al ejecutar el script manualmente.
    # Ejemplo de uso: ./scripts/Check-Coverage.ps1 -OpenReport
    [switch]$OpenReport
)

# --- CONFIGURACIÓN DE UMBRALES ---
$coreThreshold = 90
$applicationThreshold = 80
$infrastructureThreshold = 50
# ---------------------------------

Write-Host "Ejecutando script de cobertura por capas..."

# Definir rutas basadas en la ubicación del script
try {
    $scriptPath = $MyInvocation.MyCommand.Path
    $solutionDir = (Resolve-Path (Join-Path $scriptPath "../..")).Path
} catch {
    Write-Host "Error: No se pudo determinar la ruta de la solución. Asegúrate de que el script está en la carpeta /scripts." -ForegroundColor Red
    exit 1
}

$testProjectDir = Join-Path $solutionDir "ADR_T.ProductCatalog.Tests"
$testResultsDir = Join-Path $testProjectDir "TestResults"
$htmlReportDir = Join-Path $solutionDir "coverage-report"

# 1. Ejecutar pruebas con cobertura y usar el archivo .runsettings
Write-Host "  -> Paso 1: Ejecutando pruebas y generando informe..."
if (Test-Path $testResultsDir) { Remove-Item -Recurse -Force $testResultsDir }
New-Item -ItemType Directory -Path $testResultsDir | Out-Null

$testOutput = dotnet test `
    --settings "$solutionDir/solution.runsettings" `
    --collect:"XPlat Code Coverage;Format=cobertura" `
    --results-directory "$testResultsDir" `
    --logger "console;verbosity=normal" | Out-String

if ($LASTEXITCODE -ne 0) {
    Write-Host "  -> ERROR: Las pruebas unitarias o de integración fallaron." -ForegroundColor Red
    exit 1
}
Write-Host "  -> OK: Pruebas ejecutadas con éxito."

# 2. Analizar el informe de cobertura XML
Write-Host "  -> Paso 2: Analizando el informe de cobertura..."
$coverageReportPath = $null
$regex = 'Datos adjuntos:\s*(.*coverage\.cobertura\.xml)'
if ($testOutput -match $regex) {
    $coverageReportPath = $matches[1].Trim()
}

if ($null -eq $coverageReportPath -or -not (Test-Path $coverageReportPath)) {
    Write-Host "  -> ERROR: No se pudo extraer la ruta del archivo 'cobertura.xml' desde la salida de la prueba." -ForegroundColor Red
    exit 1
}
Write-Host "  -> OK: Informe XML encontrado en '$coverageReportPath'."

[xml]$coverageXml = Get-Content $coverageReportPath

function Get-CoverageForAssembly($assemblyName, $xmlReport) {
    $package = $xmlReport.coverage.packages.package | Where-Object { $_.name -eq $assemblyName }
    if ($null -ne $package) {
        return [Math]::Round(([double]$package.'line-rate' * 100), 2)
    }
    return 0
}

# 3. Validar los umbrales
Write-Host "  -> Paso 3: Validando umbrales de cobertura..."
$coreCoverage = Get-CoverageForAssembly "ADR_T.ProductCatalog.Core" $coverageXml
$applicationCoverage = Get-CoverageForAssembly "ADR_T.ProductCatalog.Application" $coverageXml
$infrastructureCoverage = Get-CoverageForAssembly "ADR_T.ProductCatalog.Infrastructure" $coverageXml

Write-Host "    - Core: $coreCoverage% (Requerido: $coreThreshold%)"
Write-Host "    - Application: $applicationCoverage% (Requerido: $applicationThreshold%)"
Write-Host "    - Infrastructure: $infrastructureCoverage% (Requerido: $infrastructureThreshold%)"

$commitAllowed = $true
if ($coreCoverage -lt $coreThreshold) { $commitAllowed = $false }
if ($applicationCoverage -lt $applicationThreshold) { $commitAllowed = $false }
if ($infrastructureCoverage -lt $infrastructureThreshold) { $commitAllowed = $false }

# 4. Generar informe HTML sin importar si la validación pasó o no (útil para depurar)
Write-Host "  -> Paso 4: Generando informe HTML de cobertura..."
try {
    if (Test-Path $htmlReportDir) { Remove-Item -Recurse -Force $htmlReportDir }
    & "$solutionDir/.config/reportgenerator" `
        -reports:"$coverageReportPath" `
        -targetdir:"$htmlReportDir" `
        -reporttypes:Html | Out-Null
    Write-Host "  -> OK: Informe HTML generado en la carpeta '$htmlReportDir'." -ForegroundColor Green
} catch {
    Write-Host "  -> ERROR: Falló la generación del informe HTML. Asegúrate de tener 'dotnet-reportgenerator-globaltool' instalado localmente." -ForegroundColor Red
    Write-Host "     Ejecuta: dotnet tool install dotnet-reportgenerator-globaltool --tool-path ./.config"
}

# 5. Abrir el informe si se solicitó
if ($OpenReport) {
    $reportIndexPath = Join-Path $htmlReportDir "index.html"
    if (Test-Path $reportIndexPath) {
        Write-Host "  -> Abriendo el informe en el navegador..."
        Start-Process -FilePath $reportIndexPath
    } else {
        Write-Host "  -> ADVERTENCIA: No se encontró 'index.html' para abrir." -ForegroundColor Yellow
    }
}

# 6. Finalizar
if ($commitAllowed) {
    Write-Host "ÉXITO: Todos los umbrales de cobertura se cumplen." -ForegroundColor Green
    exit 0
} else {
    Write-Host "FALLÓ: Uno o más umbrales de cobertura no se cumplen. Revisa el informe en '$htmlReportDir'." -ForegroundColor Red
    exit 1
}

