package main

import (
	"fmt"
	"io/ioutil"
	"os"
	"path/filepath"
	"strings"
	"time"
)

// Configuración de parámetros del proyecto
const (
	solucionPath = "C:\\Users\\ADR_T\\source\\repos\\temp-migracion" // CAMBIA ESTO a tu ruta de solución
	rutaSalida   = "C:\\Users\\ADR_T\\source\\repos"                 // CAMBIA ESTO a tu ruta de salida deseada
)

var (
	carpetasAIgnorar = []string{
		"bin",
		"obj",
		".vs",
		"Migrationes",
		"packages",
		"Properties",
		"ClientApp",
		"angularui",
		"wwwroot",
		"node_modules", // Se agrega para ignorar las dependencias de Node.js
		"ADR_T.ProductCatalog.Frontend",
		"product-catalog-ui", // Corregido para ignorar la carpeta de la UI con el nombre exacto
	}
	extensionesDeConfiguracion = []string{".json", ".csproj", ".sln", ".config"}
	contenidoCompleto          []string
)

func main() {
	// Asegurar que la carpeta de salida exista
	if _, err := os.Stat(rutaSalida); os.IsNotExist(err) {
		err := os.MkdirAll(rutaSalida, 0755)
		if err != nil {
			fmt.Printf("Error al crear la carpeta de salida: %v\n", err)
			return
		}
	}

	// Generar nombre de archivo con fecha actual y extensión .md
	fechaActual := time.Now().Format("20060102_150405")
	archivoSalida := filepath.Join(rutaSalida, fmt.Sprintf("Resumen_Configuraciones_%s.md", fechaActual))

	// Eliminar archivo existente si existe
	if _, err := os.Stat(archivoSalida); err == nil {
		err := os.Remove(archivoSalida)
		if err != nil {
			fmt.Printf("Error al eliminar el archivo existente: %v\n", err)
			return
		}
	}

	// --- DOCUMENTACIÓN DE CONFIGURACIONES ---
	contenidoCompleto = append(contenidoCompleto, "# DOCUMENTACIÓN DE CONFIGURACIONES IMPORTANTES")
	contenidoCompleto = append(contenidoCompleto, "---")
	contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("Ruta base: `%s`", solucionPath))
	contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("Generado: %s", time.Now().Format("2006-01-02 15:04:05")))
	contenidoCompleto = append(contenidoCompleto, "")

	extractConfigurationFiles(solucionPath)

	// Escribir todo el contenido al archivo de salida de una vez
	outputContent := strings.Join(contenidoCompleto, "\n")
	err := ioutil.WriteFile(archivoSalida, []byte(outputContent), 0644)
	if err != nil {
		fmt.Printf("Error al escribir el archivo de salida: %v\n", err)
		return
	}

	fmt.Printf("Proceso completado. Resumen de configuraciones guardado en %s\n", archivoSalida)
}

// extractConfigurationFiles busca y extrae archivos de configuración clave.
func extractConfigurationFiles(path string) {
	filepath.Walk(path, func(filePath string, info os.FileInfo, err error) error {
		if err != nil {
			fmt.Printf("Error al acceder a la ruta %s: %v\n", filePath, err)
			return nil // Continuar el recorrido
		}

		if info.IsDir() {
			// Excluir carpetas ignoradas
			for _, ignoredFolder := range carpetasAIgnorar {
				// Compara el nombre de la carpeta con las que se deben ignorar
				if strings.EqualFold(info.Name(), ignoredFolder) {
					fmt.Printf("Ignorando carpeta: %s\n", info.Name())
					return filepath.SkipDir // Saltar esta carpeta y su contenido
				}
			}
			return nil
		}

		// Procesar solo archivos de configuración
		if containsExtension(extensionesDeConfiguracion, info.Name()) {
			fmt.Printf("Procesando archivo de configuración: %s\n", filePath)
			contenidoArchivo, err := ioutil.ReadFile(filePath)
			if err != nil {
				fmt.Printf("Error al leer archivo %s: %v\n", filePath, err)
				return nil
			}

			contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("## Archivo: `%s`", filePath))
			contenidoCompleto = append(contenidoCompleto, "---")
			contenidoCompleto = append(contenidoCompleto, "")

			// Determinar el lenguaje del bloque de código
			var codeLang string
			switch filepath.Ext(info.Name()) {
			case ".json":
				codeLang = "json"
			case ".csproj":
				codeLang = "xml"
			case ".sln":
				codeLang = "ini"
			case ".config":
				codeLang = "xml"
			default:
				codeLang = ""
			}

			contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("```%s", codeLang))
			contenidoCompleto = append(contenidoCompleto, string(contenidoArchivo))
			contenidoCompleto = append(contenidoCompleto, "```")
			contenidoCompleto = append(contenidoCompleto, "")
			contenidoCompleto = append(contenidoCompleto, "---")
			contenidoCompleto = append(contenidoCompleto, "")
		}
		return nil
	})
}

// containsExtension verifica si la extensión de un archivo está en la lista de extensiones permitidas.
func containsExtension(extensions []string, fileName string) bool {
	ext := filepath.Ext(fileName)
	for _, allowedExt := range extensions {
		if allowedExt == ext {
			return true
		}
	}
	return false
}

// contains verifica si un string está en un slice de strings.
func contains(slice []string, item string) bool {
	for _, a := range slice {
		if a == item {
			return true
		}
	}
	return false
}
