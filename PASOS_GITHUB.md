# 📝 Pasos Detallados para Subir a GitHub

## 🎯 Objetivo
Subir tu proyecto **API Gateway Service Layer** desde tu computadora local a GitHub.

---

## 📍 ¿Dónde se Ejecutan los Comandos?

Los comandos se ejecutan en la **terminal/consola de tu computadora local**, dentro del directorio del proyecto.

### Según tu Sistema Operativo:

#### 🪟 Windows
- **CMD (Símbolo del sistema)**: `Win + R` → escribe `cmd` → Enter
- **PowerShell**: `Win + X` → selecciona "Windows PowerShell"
- **Git Bash** (recomendado): Clic derecho en el escritorio → "Git Bash Here"
- **Terminal de VS Code**: `Ctrl + Ñ` o `View → Terminal`

#### 🍎 macOS
- **Terminal**: `Cmd + Espacio` → escribe "Terminal" → Enter
- **iTerm2**: Si lo tienes instalado
- **Terminal de VS Code**: `Cmd + J` o `View → Terminal`

#### 🐧 Linux
- **Terminal**: `Ctrl + Alt + T`
- **Konsole, GNOME Terminal, etc.**
- **Terminal de VS Code**: `Ctrl + Ñ` o `View → Terminal`

---

## 🔧 Requisitos Previos

### 1. Instalar Git

#### Windows:
```bash
# Descarga e instala desde:
https://git-scm.com/download/win

# Verifica la instalación:
git --version
```

#### macOS:
```bash
# Usando Homebrew:
brew install git

# O descarga desde:
https://git-scm.com/download/mac

# Verifica:
git --version
```

#### Linux:
```bash
# Ubuntu/Debian:
sudo apt-get update
sudo apt-get install git

# Fedora/RHEL:
sudo dnf install git

# Verifica:
git --version
```

### 2. Crear Cuenta en GitHub

1. Ve a: https://github.com/signup
2. Crea tu cuenta (gratis)
3. Verifica tu email
4. Inicia sesión

### 3. Configurar Git (Primera vez)

```bash
# Configura tu nombre (reemplaza con tu nombre):
git config --global user.name "Tu Nombre"

# Configura tu email (el mismo de GitHub):
git config --global user.email "tu_email@ejemplo.com"

# Verifica la configuración:
git config --list
```

---

## 📥 Paso 1: Descargar el Proyecto a tu Computadora

### Opción A: Desde el Sandbox (si tienes acceso)

```bash
# En tu computadora local, crea una carpeta:
mkdir ~/Proyectos
cd ~/Proyectos

# Copia el proyecto desde el sandbox:
# (Necesitarás descargar el archivo ZIP o usar scp/rsync)
```

### Opción B: Descargar el ZIP

1. Descarga el archivo `ApiGateServiceLayer.zip`
2. Descomprímelo en tu computadora
3. Navega a la carpeta en la terminal

**Ejemplo Windows:**
```cmd
cd C:\Users\TuUsuario\Descargas\improved-api
```

**Ejemplo macOS/Linux:**
```bash
cd ~/Descargas/improved-api
```

---

## 🚀 Paso 2: Crear Repositorio en GitHub

### 2.1 Crear el Repositorio

1. **Ve a GitHub:** https://github.com/new
2. **Completa el formulario:**
   - **Repository name:** `api-gateway-service-layer`
   - **Description:** "Professional C# API Gateway for SAP Service Layer with enterprise-grade security"
   - **Visibility:** 
     - ✅ **Public** (visible para todos)
     - ⚪ **Private** (solo tú puedes verlo)
   - ⚠️ **NO marques:**
     - ❌ Add a README file
     - ❌ Add .gitignore
     - ❌ Choose a license
   
3. **Clic en:** "Create repository"

### 2.2 Copiar la URL del Repositorio

Después de crear el repositorio, GitHub te mostrará una página con instrucciones. Verás algo como:

```
https://github.com/TU_USUARIO/api-gateway-service-layer.git
```

**Ejemplo real:**
- Si tu usuario es `juanperez`, la URL será:
  ```
  https://github.com/juanperez/api-gateway-service-layer.git
  ```

**📋 Copia esta URL** (la necesitarás en el siguiente paso)

---

## 💻 Paso 3: Ejecutar Comandos en la Terminal

### 3.1 Abrir Terminal en el Directorio del Proyecto

#### Opción A: Navegar con comandos

**Windows (CMD/PowerShell):**
```cmd
cd C:\Users\TuUsuario\Descargas\improved-api
```

**macOS/Linux:**
```bash
cd ~/Descargas/improved-api
```

#### Opción B: Abrir terminal desde el explorador

**Windows:**
1. Abre la carpeta `improved-api` en el Explorador de Windows
2. Clic derecho en un espacio vacío
3. Selecciona "Git Bash Here" o "Abrir en Terminal"

**macOS:**
1. Abre la carpeta `improved-api` en Finder
2. Clic derecho → "Servicios" → "Nueva Terminal en Carpeta"

**Linux:**
1. Abre la carpeta `improved-api` en el explorador de archivos
2. Clic derecho → "Abrir en Terminal"

### 3.2 Verificar que Estás en el Directorio Correcto

```bash
# Ver el contenido de la carpeta:
ls -la

# Deberías ver archivos como:
# - Program.cs
# - README.md
# - .gitignore
# - Controllers/
# - Services/
# etc.
```

**Windows (CMD):**
```cmd
dir

# Deberías ver los mismos archivos
```

### 3.3 Verificar el Estado de Git

```bash
# Verifica que Git está inicializado:
git status

# Deberías ver algo como:
# On branch master
# nothing to commit, working tree clean
```

**Si ves un error** "not a git repository":
```bash
# Inicializa Git:
git init

# Agrega todos los archivos:
git add .

# Crea el primer commit:
git commit -m "feat: Professional C# API Gateway with enterprise-grade security"
```

---

## 🔗 Paso 4: Conectar con GitHub (AQUÍ SE EJECUTA EL COMANDO)

### 4.1 Agregar el Repositorio Remoto

**Este es el comando que preguntaste:**

```bash
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git
```

**⚠️ IMPORTANTE:** Reemplaza `TU_USUARIO` con tu usuario real de GitHub.

**Ejemplo real:**
```bash
# Si tu usuario de GitHub es "juanperez":
git remote add origin https://github.com/juanperez/api-gateway-service-layer.git

# Si tu usuario es "maria_dev":
git remote add origin https://github.com/maria_dev/api-gateway-service-layer.git

# Si tu usuario es "carlos123":
git remote add origin https://github.com/carlos123/api-gateway-service-layer.git
```

### 4.2 Verificar que se Agregó Correctamente

```bash
git remote -v

# Deberías ver:
# origin  https://github.com/TU_USUARIO/api-gateway-service-layer.git (fetch)
# origin  https://github.com/TU_USUARIO/api-gateway-service-layer.git (push)
```

### 4.3 Renombrar la Rama a "main"

```bash
git branch -M main
```

**Explicación:** GitHub usa "main" como rama principal, pero Git crea "master" por defecto. Este comando renombra la rama.

---

## 📤 Paso 5: Subir el Código a GitHub

### 5.1 Hacer Push

```bash
git push -u origin main
```

### 5.2 Autenticación

GitHub te pedirá autenticarte. Hay dos opciones:

#### Opción A: Personal Access Token (Recomendado)

1. **Genera un token:**
   - Ve a: https://github.com/settings/tokens
   - Clic en "Generate new token" → "Generate new token (classic)"
   - **Note:** "API Gateway Project"
   - **Expiration:** 90 days (o lo que prefieras)
   - **Scopes:** Marca ✅ `repo` (acceso completo a repositorios)
   - Clic en "Generate token"
   - **📋 COPIA EL TOKEN** (solo se muestra una vez)

2. **Usa el token como contraseña:**
   ```
   Username: tu_usuario_github
   Password: ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (pega el token)
   ```

#### Opción B: GitHub CLI (Más fácil)

```bash
# Instala GitHub CLI:
# Windows: https://cli.github.com/
# macOS: brew install gh
# Linux: sudo apt install gh

# Autentícate:
gh auth login

# Sigue las instrucciones en pantalla
# Selecciona: GitHub.com → HTTPS → Yes → Login with a web browser

# Luego haz push:
git push -u origin main
```

### 5.3 Verificar que se Subió

1. Ve a tu repositorio en GitHub:
   ```
   https://github.com/TU_USUARIO/api-gateway-service-layer
   ```

2. Deberías ver todos tus archivos:
   - README.md
   - Program.cs
   - Controllers/
   - Services/
   - etc.

---

## ✅ Resumen de Comandos Completos

### Desde Cero (si no tienes Git inicializado):

```bash
# 1. Navega al directorio del proyecto
cd /ruta/a/tu/proyecto/improved-api

# 2. Inicializa Git (si no está inicializado)
git init

# 3. Configura Git (primera vez)
git config --global user.name "Tu Nombre"
git config --global user.email "tu_email@ejemplo.com"

# 4. Agrega todos los archivos
git add .

# 5. Crea el primer commit
git commit -m "feat: Professional C# API Gateway with enterprise-grade security"

# 6. Conecta con GitHub (REEMPLAZA TU_USUARIO)
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git

# 7. Renombra la rama a main
git branch -M main

# 8. Sube el código
git push -u origin main
```

### Si Ya Tienes Git Inicializado:

```bash
# 1. Navega al directorio
cd /ruta/a/tu/proyecto/improved-api

# 2. Verifica el estado
git status

# 3. Conecta con GitHub (REEMPLAZA TU_USUARIO)
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git

# 4. Renombra la rama
git branch -M main

# 5. Sube el código
git push -u origin main
```

---

## 🔧 Solución de Problemas Comunes

### Error: "remote origin already exists"

```bash
# Elimina el remoto existente:
git remote remove origin

# Agrega el nuevo:
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git
```

### Error: "failed to push some refs"

```bash
# Descarga los cambios remotos primero:
git pull origin main --rebase

# Luego intenta de nuevo:
git push -u origin main
```

### Error: "Permission denied"

- Verifica que estás usando el **Personal Access Token** correcto
- Asegúrate de que el token tiene permisos de `repo`
- Genera un nuevo token si es necesario

### Error: "git: command not found"

- Git no está instalado
- Instala Git según tu sistema operativo (ver sección "Requisitos Previos")

### Error: "not a git repository"

```bash
# Estás en el directorio equivocado o Git no está inicializado
# Verifica que estás en la carpeta correcta:
pwd  # macOS/Linux
cd   # Windows

# Inicializa Git:
git init
```

---

## 📍 Ejemplo Completo Paso a Paso

### Escenario: Usuario "juanperez" en Windows

```cmd
REM 1. Abrir CMD o PowerShell
REM Win + R → cmd → Enter

REM 2. Navegar al proyecto
cd C:\Users\Juan\Descargas\improved-api

REM 3. Verificar contenido
dir

REM 4. Verificar Git
git status

REM 5. Conectar con GitHub
git remote add origin https://github.com/juanperez/api-gateway-service-layer.git

REM 6. Verificar conexión
git remote -v

REM 7. Renombrar rama
git branch -M main

REM 8. Subir código
git push -u origin main

REM 9. Ingresar credenciales cuando se soliciten:
REM Username: juanperez
REM Password: ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (token)
```

### Escenario: Usuario "maria_dev" en macOS

```bash
# 1. Abrir Terminal
# Cmd + Espacio → "Terminal" → Enter

# 2. Navegar al proyecto
cd ~/Descargas/improved-api

# 3. Verificar contenido
ls -la

# 4. Verificar Git
git status

# 5. Conectar con GitHub
git remote add origin https://github.com/maria_dev/api-gateway-service-layer.git

# 6. Verificar conexión
git remote -v

# 7. Renombrar rama
git branch -M main

# 8. Subir código
git push -u origin main

# 9. Ingresar credenciales cuando se soliciten:
# Username: maria_dev
# Password: ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (token)
```

---

## 🎯 Próximos Pasos Después de Subir

### 1. Verificar en GitHub

```
https://github.com/TU_USUARIO/api-gateway-service-layer
```

### 2. Agregar Descripción y Topics

1. Ve a tu repositorio en GitHub
2. Clic en ⚙️ (Settings) o en "About" → Edit
3. Agrega:
   - **Description:** "Professional C# API Gateway for SAP Service Layer"
   - **Topics:** `csharp`, `dotnet`, `api-gateway`, `sap`, `service-layer`, `security`

### 3. Configurar GitHub Pages (Opcional)

Si quieres publicar la documentación:
1. Settings → Pages
2. Source: Deploy from a branch
3. Branch: main → /docs (si tienes carpeta docs)

### 4. Proteger la Rama Main

1. Settings → Branches
2. Add rule
3. Branch name pattern: `main`
4. Marca: ✅ Require pull request reviews before merging

### 5. Agregar Badges al README

Edita el README.md y agrega al inicio:

```markdown
# API Gateway Service Layer

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![License](https://img.shields.io/badge/license-MIT-green)
![Security](https://img.shields.io/badge/security-enterprise-brightgreen)
![GitHub last commit](https://img.shields.io/github/last-commit/TU_USUARIO/api-gateway-service-layer)
![GitHub issues](https://img.shields.io/github/issues/TU_USUARIO/api-gateway-service-layer)
```

---

## 📚 Comandos Git Útiles para el Futuro

### Ver Estado
```bash
git status
```

### Agregar Cambios
```bash
# Agregar un archivo específico:
git add archivo.cs

# Agregar todos los cambios:
git add .
```

### Hacer Commit
```bash
git commit -m "Descripción del cambio"
```

### Subir Cambios
```bash
git push
```

### Descargar Cambios
```bash
git pull
```

### Ver Historial
```bash
git log --oneline
```

### Crear Rama
```bash
git checkout -b feature/nueva-funcionalidad
```

### Cambiar de Rama
```bash
git checkout main
```

### Ver Ramas
```bash
git branch -a
```

---

## 🆘 Ayuda Adicional

### Documentación Oficial
- **Git:** https://git-scm.com/doc
- **GitHub:** https://docs.github.com/
- **GitHub CLI:** https://cli.github.com/manual/

### Tutoriales Interactivos
- **Learn Git Branching:** https://learngitbranching.js.org/
- **GitHub Skills:** https://skills.github.com/

### Videos (YouTube)
- Busca: "Git y GitHub para principiantes"
- Busca: "Cómo subir proyecto a GitHub"

---

## ✨ Resumen Visual

```
┌─────────────────────────────────────────────────────────────┐
│  TU COMPUTADORA LOCAL                                       │
│                                                             │
│  📁 improved-api/                                           │
│     ├── Program.cs                                          │
│     ├── README.md                                           │
│     ├── Controllers/                                        │
│     └── ...                                                 │
│                                                             │
│  💻 Terminal/CMD/PowerShell                                 │
│     $ cd /ruta/a/improved-api                               │
│     $ git remote add origin https://github.com/...          │
│     $ git push -u origin main                               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ git push
                            ▼
┌─────────────────────────────────────────────────────────────┐
│  ☁️  GITHUB (Nube)                                          │
│                                                             │
│  🌐 https://github.com/TU_USUARIO/api-gateway-service-layer│
│                                                             │
│     📁 Repositorio con todos tus archivos                   │
│        ├── Program.cs                                       │
│        ├── README.md                                        │
│        ├── Controllers/                                     │
│        └── ...                                              │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎉 ¡Listo!

Ahora sabes **exactamente dónde y cómo** ejecutar el comando `git remote add origin`.

**Recuerda:**
1. ✅ Ejecuta los comandos en la **terminal de tu computadora**
2. ✅ Dentro del **directorio del proyecto** (`improved-api/`)
3. ✅ Reemplaza `TU_USUARIO` con tu **usuario real de GitHub**
4. ✅ Usa un **Personal Access Token** como contraseña

**¡Éxito con tu proyecto!** 🚀
