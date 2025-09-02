#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Genera y abre un reporte de cobertura de pruebas en formato HTML.
.DESCRIPTION
    Este script ejecuta las pruebas con cobertura, genera un reporte HTML y lo abre en el navegador.
#>

# Parar en caso de error
$ErrorActionPreference = "Stop"

Write-Host "Ejecutando pruebas con cobertura..." -ForegroundColor Green
dotnet test ADR_T.ProductCatalog.Tests/ADR_T.ProductCatalog.Tests.csproj --collect:"XPlat Code Coverage"

# Encontrar el archivo de cobertura más reciente
Write-Host "Buscando archivo de cobertura..." -ForegroundColor Green
$coverageFile = Get-ChildItem -Path ADR_T.ProductCatalog.Tests/TestResults -Recurse -Filter "coverage.cobertura.xml" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $coverageFile) {
    Write-Error "No se encontró el archivo de cobertura."
    exit 1
}

Write-Host "Generando reporte HTML..." -ForegroundColor Green
reportgenerator -reports:"$($coverageFile.FullName)" -targetdir:./coverage-report -reporttypes:HTML

# Abrir el reporte en el navegador
$reportPath = Resolve-Path "./coverage-report/index.html"
Write-Host "Abriendo reporte de cobertura..." -ForegroundColor Green
Start-Process $reportPath

Write-Host "Reporte generado exitosamente en: $reportPath" -ForegroundColor Green