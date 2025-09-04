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
	solucionPath = "C:\\Users\\ADR_T\\source\\repos\\temp-migracion" // CAMBIA ESTO a tu ruta

	rutaSalida = "C:\\Users\\ADR_T\\source\\repos" // CAMBIA ESTO a tu ruta de salida deseada
)

var (
	carpetasAIgnorar = []string{
		"bin",
		"obj",
		".vs",
		"Migrationes",
		"node_modules",
		"objects",
		"refs",
		"packages",
		"Properties",
	}
	extensionesParaDiagrama = []string{".sln", ".csproj", ".yml", ".cs", ".cshtml", ".js"}
	contenidoCompleto       []string
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
	archivoSalida := filepath.Join(rutaSalida, fmt.Sprintf("Resumen_Proyecto_%s.md", fechaActual))

	// Eliminar archivo existente si existe
	if _, err := os.Stat(archivoSalida); err == nil {
		err := os.Remove(archivoSalida)
		if err != nil {
			fmt.Printf("Error al eliminar el archivo existente: %v\n", err)
			return
		}
	}

	// --- DIAGRAMA DE ESTRUCTURA DEL PROYECTO ---
	contenidoCompleto = append(contenidoCompleto, "# DIAGRAMA DE ESTRUCTURA DEL PROYECTO")
	contenidoCompleto = append(contenidoCompleto, "---")
	contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("Ruta base: `%s`", solucionPath))
	contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("Generado: %s", time.Now().Format("2006-01-02 15:04:05")))
	contenidoCompleto = append(contenidoCompleto, "")
	contenidoCompleto = append(contenidoCompleto, "```") // Iniciar bloque de código para el diagrama

	// Generar el diagrama de estructura
	generateDirectoryTree(solucionPath, "", 0) // El último parámetro es para la recursión, no para la indentación directa

	contenidoCompleto = append(contenidoCompleto, "```") // Cerrar bloque de código
	contenidoCompleto = append(contenidoCompleto, "")
	contenidoCompleto = append(contenidoCompleto, "---")
	contenidoCompleto = append(contenidoCompleto, "")

	// --- EXTRACCIÓN DE CÓDIGO FUENTE ---
	contenidoCompleto = append(contenidoCompleto, "# EXTRACCIÓN DE CÓDIGO FUENTE")
	contenidoCompleto = append(contenidoCompleto, "---")
	contenidoCompleto = append(contenidoCompleto, "")

	extractCode(solucionPath)

	// Escribir todo el contenido al archivo de salida de una vez
	outputContent := strings.Join(contenidoCompleto, "\n")
	err := ioutil.WriteFile(archivoSalida, []byte(outputContent), 0644)
	if err != nil {
		fmt.Printf("Error al escribir el archivo de salida: %v\n", err)
		return
	}

	fmt.Printf("Proceso completado. Resumen guardado en %s\n", archivoSalida)
}

// generateDirectoryTree recorre recursivamente los directorios y archivos para construir el diagrama.
func generateDirectoryTree(path string, indent string, level int) {
	files, err := ioutil.ReadDir(path)
	if err != nil {
		fmt.Printf("Error al leer directorio %s: %v\n", path, err)
		return
	}

	// Ordenar para que los directorios aparezcan primero
	var dirs, regularFiles []os.FileInfo
	for _, file := range files {
		if file.IsDir() {
			dirs = append(dirs, file)
		} else {
			regularFiles = append(regularFiles, file)
		}
	}
	files = append(dirs, regularFiles...)

	for i, file := range files {
		isLast := (i == len(files)-1)
		marker := "├── "
		indentContinuation := "|   "
		if isLast {
			marker = "└── "
			indentContinuation = "    "
		}

		currentLineOutput := fmt.Sprintf("%s%s%s", indent, marker, file.Name())
		nextLevelIndent := indent + indentContinuation

		if file.IsDir() {
			// Si es una carpeta y no está en la lista de exclusión
			if !contains(carpetasAIgnorar, file.Name()) {
				contenidoCompleto = append(contenidoCompleto, currentLineOutput)
				generateDirectoryTree(filepath.Join(path, file.Name()), nextLevelIndent, level+1)
			}
		} else {
			// Si es un archivo, verificar si su extensión está en la lista de inclusión para el diagrama
			if containsExtension(extensionesParaDiagrama, file.Name()) {
				contenidoCompleto = append(contenidoCompleto, currentLineOutput)
			}
		}
	}
}

// extractCode busca archivos .cs y extrae su contenido.
func extractCode(path string) {
	filepath.Walk(path, func(filePath string, info os.FileInfo, err error) error {
		if err != nil {
			fmt.Printf("Error al acceder a la ruta %s: %v\n", filePath, err)
			return nil // Continuar el recorrido
		}

		if info.IsDir() {
			// Excluir carpetas ignoradas
			for _, ignoredFolder := range carpetasAIgnorar {
				if info.Name() == ignoredFolder {
					return filepath.SkipDir // Saltar esta carpeta y su contenido
				}
			}
			return nil
		}

		// Procesar solo archivos .cs
		if strings.HasSuffix(info.Name(), ".cs") {
			fmt.Printf("Procesando: %s\n", filePath)
			contenidoArchivo, err := ioutil.ReadFile(filePath)
			if err != nil {
				fmt.Printf("Error al leer archivo %s: %v\n", filePath, err)
				return nil
			}

			// Agregar el contenido completo del archivo
			contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("## Archivo: `%s`", filePath))
			contenidoCompleto = append(contenidoCompleto, "---")
			contenidoCompleto = append(contenidoCompleto, "")
			contenidoCompleto = append(contenidoCompleto, "```csharp")
			contenidoCompleto = append(contenidoCompleto, string(contenidoArchivo))
			contenidoCompleto = append(contenidoCompleto, "```")
			contenidoCompleto = append(contenidoCompleto, "")
			contenidoCompleto = append(contenidoCompleto, "---")
			contenidoCompleto = append(contenidoCompleto, "")
		}
		return nil
	})
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

// containsExtension verifica si la extensión de un archivo está en la lista de extensiones permitidas.
func containsExtension(extensions []string, fileName string) bool {
	ext := strings.ToLower(filepath.Ext(fileName))
	for _, allowedExt := range extensions {
		if strings.ToLower(allowedExt) == ext {
			return true
		}
	}
	return false
}
