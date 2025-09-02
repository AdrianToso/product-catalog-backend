#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Verifica que la cobertura de pruebas cumpla con el umbral mínimo del 80%.
.DESCRIPTION
    Este script ejecuta las pruebas con cobertura y verifica explícitamente el porcentaje de cobertura.
    Falla si la cobertura es inferior al 80%.
#>

$ErrorActionPreference = "Stop"

# Limpiar resultados anteriores
Remove-Item -Path ADR_T.ProductCatalog.Tests/TestResults -Recurse -Force -ErrorAction SilentlyContinue

# Ejecutar pruebas con cobertura
Write-Host "Ejecutando pruebas con cobertura..." -ForegroundColor Green
dotnet test ADR_T.ProductCatalog.Tests/ADR_T.ProductCatalog.Tests.csproj --collect:"XPlat Code Coverage"

# Encontrar el archivo de cobertura más reciente
$coverageFile = Get-ChildItem -Path ADR_T.ProductCatalog.Tests/TestResults -Recurse -Filter "coverage.cobertura.xml" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $coverageFile) {
    Write-Error "No se encontró el archivo de cobertura."
    exit 1
}

# Leer y analizar el XML de cobertura
[xml]$coverageReport = Get-Content $coverageFile.FullName
$lineRate = [double]$coverageReport.coverage.'line-rate' * 100

Write-Host "Cobertura actual: $([math]::Round($lineRate, 2))%" -ForegroundColor Cyan

# Verificar umbral
if ($lineRate -lt 80) {
    Write-Host "❌ La cobertura ($([math]::Round($lineRate, 2))%) es inferior al 80% requerido" -ForegroundColor Red
    exit 1
} else {
    Write-Host "✅ La cobertura ($([math]::Round($lineRate, 2))%) cumple con el requisito del 80%" -ForegroundColor Green
    exit 0
}