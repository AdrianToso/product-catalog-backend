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
	proyectoPath = "C:\\Users\\ADR_T\\source\\repos\\temp-migracion"
	rutaSalida   = "C:\\Users\\ADR_T\\source\\repos"
)

var (
	carpetasAIgnorar = []string{
		"bin", "obj", ".vs", "node_modules", "TestResults", "Migrationes", "objects", "refs",
		"packages", "Properties", ".git", "__pycache__", "coverage-report",
	}

	// Extensiones para diagramas y documentación
	extensionesDiagramas = []string{".drawio", ".vsdx", ".xml", ".png", ".jpg", ".jpeg", ".svg"}

	// Extensiones para workflows y pipelines
	extensionesWorkflows = []string{".yml", ".yaml", ".json"}

	// Archivos y extensiones para Docker
	archivosDocker = []string{"Dockerfile", "docker-compose.yml", "docker-compose.yaml", ".dockerignore"}

	contenidoCompleto []string
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
	archivoSalida := filepath.Join(rutaSalida, fmt.Sprintf("Documentacion_Diagramas_%s.md", fechaActual))

	// Eliminar archivo existente si existe
	if _, err := os.Stat(archivoSalida); err == nil {
		err := os.Remove(archivoSalida)
		if err != nil {
			fmt.Printf("Error al eliminar el archivo existente: %v\n", err)
			return
		}
	}

	// --- DIAGRAMA DE ESTRUCTURA DEL PROYECTO ---
	contenidoCompleto = append(contenidoCompleto, "# DOCUMENTACIÓN DE DIAGRAMAS, WORKFLOWS Y DOCKER")
	contenidoCompleto = append(contenidoCompleto, "---")
	contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("Ruta base: `%s`", proyectoPath))
	contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("Generado: %s", time.Now().Format("2006-01-02 15:04:05")))
	contenidoCompleto = append(contenidoCompleto, "")
	contenidoCompleto = append(contenidoCompleto, "## ESTRUCTURA DEL PROYECTO")
	contenidoCompleto = append(contenidoCompleto, "```")

	// Generar el diagrama de estructura
	generateDirectoryTree(proyectoPath, "", 0)

	contenidoCompleto = append(contenidoCompleto, "```")
	contenidoCompleto = append(contenidoCompleto, "")

	// --- DOCUMENTACIÓN DE DIAGRAMAS ---
	contenidoCompleto = append(contenidoCompleto, "## DIAGRAMAS")
	contenidoCompleto = append(contenidoCompleto, "---")
	documentarArchivos(proyectoPath, extensionesDiagramas, "Diagrama")

	// --- DOCUMENTACIÓN DE WORKFLOWS ---
	contenidoCompleto = append(contenidoCompleto, "## WORKFLOWS Y PIPELINES")
	contenidoCompleto = append(contenidoCompleto, "---")
	documentarArchivos(proyectoPath, extensionesWorkflows, "Workflow")

	// --- DOCUMENTACIÓN DE DOCKER ---
	contenidoCompleto = append(contenidoCompleto, "## CONFIGURACIONES DOCKER")
	contenidoCompleto = append(contenidoCompleto, "---")
	documentarArchivos(proyectoPath, archivosDocker, "Docker")

	// Escribir todo el contenido al archivo de salida de una vez
	outputContent := strings.Join(contenidoCompleto, "\n")
	err := ioutil.WriteFile(archivoSalida, []byte(outputContent), 0644)
	if err != nil {
		fmt.Printf("Error al escribir el archivo de salida: %v\n", err)
		return
	}

	fmt.Printf("Proceso completado. Documentación guardada en %s\n", archivoSalida)
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
			// Si es un archivo, verificar si su extensión está en alguna de las listas de interés
			esDiagrama := containsExtension(extensionesDiagramas, file.Name())
			esWorkflow := containsExtension(extensionesWorkflows, file.Name())
			esDocker := contains(archivosDocker, file.Name())

			if esDiagrama || esWorkflow || esDocker {
				contenidoCompleto = append(contenidoCompleto, currentLineOutput)
			}
		}
	}
}

// documentarArchivos busca y documenta archivos específicos
func documentarArchivos(path string, extensiones []string, tipo string) {
	filepath.Walk(path, func(filePath string, info os.FileInfo, err error) error {
		if err != nil {
			fmt.Printf("Error al acceder a la ruta %s: %v\n", filePath, err)
			return nil
		}

		if info.IsDir() {
			// Excluir carpetas ignoradas
			for _, ignoredFolder := range carpetasAIgnorar {
				if info.Name() == ignoredFolder {
					return filepath.SkipDir
				}
			}
			return nil
		}

		// Verificar si el archivo coincide con las extensiones buscadas
		if containsExtension(extensiones, info.Name()) || contains(extensiones, info.Name()) {
			fmt.Printf("Documentando %s: %s\n", tipo, filePath)

			// Para archivos de imagen, solo mencionarlos
			if isImageFile(info.Name()) {
				contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("### Archivo: `%s`", filePath))
				contenidoCompleto = append(contenidoCompleto, "")
				contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("![%s](%s)", info.Name(), filePath))
				contenidoCompleto = append(contenidoCompleto, "")
				contenidoCompleto = append(contenidoCompleto, "---")
				contenidoCompleto = append(contenidoCompleto, "")
				return nil
			}

			// Para archivos de texto, extraer el contenido
			contenidoArchivo, err := ioutil.ReadFile(filePath)
			if err != nil {
				fmt.Printf("Error al leer archivo %s: %v\n", filePath, err)
				return nil
			}

			// Determinar el lenguaje para el bloque de código
			lenguaje := obtenerLenguaje(info.Name())

			contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("### Archivo: `%s`", filePath))
			contenidoCompleto = append(contenidoCompleto, "")
			contenidoCompleto = append(contenidoCompleto, fmt.Sprintf("```%s", lenguaje))
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

// isImageFile verifica si un archivo es una imagen
func isImageFile(fileName string) bool {
	imageExtensions := []string{".png", ".jpg", ".jpeg", ".svg", ".gif", ".bmp"}
	return containsExtension(imageExtensions, fileName)
}

// obtenerLenguaje determina el lenguaje para el bloque de código basado en la extensión del archivo
func obtenerLenguaje(fileName string) string {
	ext := strings.ToLower(filepath.Ext(fileName))
	switch ext {
	case ".yml", ".yaml":
		return "yaml"
	case ".json":
		return "json"
	case ".xml":
		return "xml"
	default:
		return "text"
	}
}
