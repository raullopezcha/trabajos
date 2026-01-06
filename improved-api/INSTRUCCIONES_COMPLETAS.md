# 🎯 Instrucciones Completas: Subir Proyecto a GitHub

## ❓ ¿Dónde se Ejecuta el Comando?

El comando `git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git` se ejecuta en:

### 📍 Ubicación: Terminal/Consola de TU COMPUTADORA LOCAL

**NO en:**
- ❌ GitHub (sitio web)
- ❌ Navegador
- ❌ Editor de texto

**SÍ en:**
- ✅ Terminal (macOS/Linux)
- ✅ CMD (Windows)
- ✅ PowerShell (Windows)
- ✅ Git Bash (Windows)
- ✅ Terminal integrada de VS Code

---

## 🖥️ Cómo Abrir la Terminal

### Windows

**Opción 1: CMD (Símbolo del sistema)**
1. Presiona `Win + R`
2. Escribe `cmd`
3. Presiona Enter

**Opción 2: PowerShell**
1. Presiona `Win + X`
2. Selecciona "Windows PowerShell"

**Opción 3: Git Bash (Recomendado)**
1. Clic derecho en el escritorio o carpeta
2. Selecciona "Git Bash Here"

**Opción 4: Terminal de VS Code**
1. Abre VS Code
2. Presiona `Ctrl + Ñ`
3. O ve a: View → Terminal

### macOS

**Opción 1: Terminal**
1. Presiona `Cmd + Espacio`
2. Escribe "Terminal"
3. Presiona Enter

**Opción 2: Desde Finder**
1. Abre la carpeta del proyecto
2. Clic derecho → Servicios → Nueva Terminal en Carpeta

**Opción 3: Terminal de VS Code**
1. Abre VS Code
2. Presiona `Cmd + J`
3. O ve a: View → Terminal

### Linux

**Opción 1: Atajo de teclado**
1. Presiona `Ctrl + Alt + T`

**Opción 2: Desde el menú**
1. Busca "Terminal" en el menú de aplicaciones

**Opción 3: Terminal de VS Code**
1. Abre VS Code
2. Presiona `Ctrl + Ñ`

---

## 📋 Pasos Completos (Desde Cero)

### Paso 1: Instalar Git (Si no lo tienes)

#### Windows:
```bash
# Descarga desde: https://git-scm.com/download/win
# Ejecuta el instalador
# Verifica:
git --version
```

#### macOS:
```bash
# Instala con Homebrew:
brew install git

# O descarga desde: https://git-scm.com/download/mac
# Verifica:
git --version
```

#### Linux:
```bash
# Ubuntu/Debian:
sudo apt-get install git

# Fedora/RHEL:
sudo dnf install git

# Verifica:
git --version
```

### Paso 2: Configurar Git (Primera vez)

```bash
# Configura tu nombre:
git config --global user.name "Tu Nombre Completo"

# Configura tu email (el mismo de GitHub):
git config --global user.email "tu_email@ejemplo.com"

# Verifica:
git config --list
```

### Paso 3: Crear Cuenta en GitHub

1. Ve a: https://github.com/signup
2. Completa el registro
3. Verifica tu email
4. Inicia sesión

### Paso 4: Descargar el Proyecto a tu Computadora

1. Descarga el archivo `improved-api` (o el ZIP)
2. Descomprímelo en una ubicación conocida
3. Anota la ruta completa

**Ejemplo de rutas:**
- Windows: `C:\Users\TuNombre\Descargas\improved-api`
- macOS: `/Users/tunombre/Descargas/improved-api`
- Linux: `/home/tunombre/Descargas/improved-api`

### Paso 5: Navegar al Directorio del Proyecto

Abre la terminal y ejecuta:

**Windows:**
```cmd
cd C:\Users\TuNombre\Descargas\improved-api
```

**macOS/Linux:**
```bash
cd ~/Descargas/improved-api
```

**Verificar que estás en el lugar correcto:**
```bash
# Ver archivos:
ls -la    # macOS/Linux
dir       # Windows

# Deberías ver:
# - Program.cs
# - README.md
# - Controllers/
# - Services/
# etc.
```

### Paso 6: Verificar Git en el Proyecto

```bash
git status
```

**Si ves:** "On branch master" o "On branch main" → ✅ Git está inicializado

**Si ves:** "not a git repository" → Ejecuta:
```bash
git init
git add .
git commit -m "Initial commit: Professional C# API Gateway"
```

### Paso 7: Crear Repositorio en GitHub

1. **Ve a:** https://github.com/new

2. **Completa:**
   - **Repository name:** `api-gateway-service-layer`
   - **Description:** "Professional C# API Gateway for SAP Service Layer"
   - **Visibility:** Public o Private (tu elección)
   
3. **⚠️ NO marques:**
   - ❌ Add a README file
   - ❌ Add .gitignore
   - ❌ Choose a license

4. **Clic en:** "Create repository"

5. **Copia la URL** que aparece (algo como):
   ```
   https://github.com/TU_USUARIO/api-gateway-service-layer.git
   ```

### Paso 8: Conectar tu Proyecto Local con GitHub

**AQUÍ ES DONDE SE EJECUTA EL COMANDO QUE PREGUNTASTE:**

En la terminal (dentro del directorio del proyecto), ejecuta:

```bash
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git
```

**⚠️ IMPORTANTE:** Reemplaza `TU_USUARIO` con tu usuario real de GitHub.

**Ejemplos reales:**

```bash
# Si tu usuario es "juanperez":
git remote add origin https://github.com/juanperez/api-gateway-service-layer.git

# Si tu usuario es "maria_dev":
git remote add origin https://github.com/maria_dev/api-gateway-service-layer.git

# Si tu usuario es "carlos123":
git remote add origin https://github.com/carlos123/api-gateway-service-layer.git
```

**Verificar que funcionó:**
```bash
git remote -v

# Deberías ver:
# origin  https://github.com/TU_USUARIO/api-gateway-service-layer.git (fetch)
# origin  https://github.com/TU_USUARIO/api-gateway-service-layer.git (push)
```

### Paso 9: Renombrar la Rama a "main"

```bash
git branch -M main
```

### Paso 10: Subir el Código a GitHub

```bash
git push -u origin main
```

### Paso 11: Autenticación

GitHub te pedirá credenciales:

**Username:** Tu usuario de GitHub

**Password:** NO uses tu contraseña de GitHub, usa un **Personal Access Token**

#### Cómo Crear un Personal Access Token:

1. Ve a: https://github.com/settings/tokens
2. Clic en "Generate new token" → "Generate new token (classic)"
3. **Note:** "API Gateway Project"
4. **Expiration:** 90 days (o lo que prefieras)
5. **Select scopes:** Marca ✅ `repo` (Full control of private repositories)
6. Clic en "Generate token"
7. **📋 COPIA EL TOKEN** (se muestra solo una vez, guárdalo en un lugar seguro)

El token se ve así: `ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`

**Cuando Git pida credenciales:**
```
Username: tu_usuario_github
Password: ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (pega el token aquí)
```

### Paso 12: Verificar en GitHub

1. Ve a tu repositorio:
   ```
   https://github.com/TU_USUARIO/api-gateway-service-layer
   ```

2. Deberías ver todos tus archivos:
   - ✅ README.md
   - ✅ Program.cs
   - ✅ Controllers/
   - ✅ Services/
   - ✅ Models/
   - ✅ etc.

---

## 🎬 Ejemplo Completo en Video-Texto

### Escenario: Usuario "juanperez" en Windows

```
1. Abrir PowerShell:
   Win + X → Windows PowerShell

2. Navegar al proyecto:
   PS C:\Users\Juan> cd C:\Users\Juan\Descargas\improved-api
   PS C:\Users\Juan\Descargas\improved-api>

3. Verificar archivos:
   PS C:\Users\Juan\Descargas\improved-api> dir
   
   Directorio: C:\Users\Juan\Descargas\improved-api
   
   Mode                 LastWriteTime         Length Name
   ----                 -------------         ------ ----
   d-----         1/5/2026   2:30 PM                Controllers
   d-----         1/5/2026   2:30 PM                Services
   d-----         1/5/2026   2:30 PM                Models
   -a----         1/5/2026   2:30 PM           1234 Program.cs
   -a----         1/5/2026   2:30 PM           5678 README.md

4. Verificar Git:
   PS C:\Users\Juan\Descargas\improved-api> git status
   On branch master
   nothing to commit, working tree clean

5. Conectar con GitHub:
   PS C:\Users\Juan\Descargas\improved-api> git remote add origin https://github.com/juanperez/api-gateway-service-layer.git

6. Verificar conexión:
   PS C:\Users\Juan\Descargas\improved-api> git remote -v
   origin  https://github.com/juanperez/api-gateway-service-layer.git (fetch)
   origin  https://github.com/juanperez/api-gateway-service-layer.git (push)

7. Renombrar rama:
   PS C:\Users\Juan\Descargas\improved-api> git branch -M main

8. Subir código:
   PS C:\Users\Juan\Descargas\improved-api> git push -u origin main
   
   Username for 'https://github.com': juanperez
   Password for 'https://juanperez@github.com': ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
   
   Enumerating objects: 25, done.
   Counting objects: 100% (25/25), done.
   Delta compression using up to 8 threads
   Compressing objects: 100% (20/20), done.
   Writing objects: 100% (25/25), 420.00 KiB | 5.00 MiB/s, done.
   Total 25 (delta 5), reused 0 (delta 0), pack-reused 0
   To https://github.com/juanperez/api-gateway-service-layer.git
    * [new branch]      main -> main
   Branch 'main' set up to track remote branch 'main' from 'origin'.

9. ¡Listo! Verificar en:
   https://github.com/juanperez/api-gateway-service-layer
```

---

## 🔧 Solución de Problemas

### ❌ Error: "git: command not found"

**Problema:** Git no está instalado

**Solución:**
```bash
# Windows: Descarga desde https://git-scm.com/download/win
# macOS: brew install git
# Linux: sudo apt-get install git
```

### ❌ Error: "not a git repository"

**Problema:** No estás en el directorio correcto o Git no está inicializado

**Solución:**
```bash
# Verifica que estás en el directorio correcto:
pwd  # macOS/Linux
cd   # Windows

# Si estás en el lugar correcto, inicializa Git:
git init
git add .
git commit -m "Initial commit"
```

### ❌ Error: "remote origin already exists"

**Problema:** Ya existe una conexión remota llamada "origin"

**Solución:**
```bash
# Elimina el remoto existente:
git remote remove origin

# Agrega el nuevo:
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git
```

### ❌ Error: "failed to push some refs"

**Problema:** El repositorio remoto tiene cambios que no tienes localmente

**Solución:**
```bash
# Descarga los cambios primero:
git pull origin main --rebase

# Luego intenta de nuevo:
git push -u origin main
```

### ❌ Error: "Permission denied" o "Authentication failed"

**Problema:** Credenciales incorrectas o falta de permisos

**Solución:**
1. Verifica que estás usando el **Personal Access Token** correcto (no tu contraseña)
2. Asegúrate de que el token tiene permisos de `repo`
3. Genera un nuevo token si es necesario: https://github.com/settings/tokens

### ❌ Error: "Repository not found"

**Problema:** La URL del repositorio es incorrecta o no existe

**Solución:**
1. Verifica que el repositorio existe en GitHub
2. Verifica que la URL es correcta (copia y pega desde GitHub)
3. Verifica que tu usuario está escrito correctamente

---

## 📝 Comandos Resumidos (Copiar y Pegar)

```bash
# 1. Navegar al proyecto (AJUSTA LA RUTA)
cd /ruta/a/tu/proyecto/improved-api

# 2. Verificar Git
git status

# 3. Si no está inicializado:
git init
git add .
git commit -m "Initial commit: Professional C# API Gateway"

# 4. Conectar con GitHub (REEMPLAZA TU_USUARIO)
git remote add origin https://github.com/TU_USUARIO/api-gateway-service-layer.git

# 5. Verificar conexión
git remote -v

# 6. Renombrar rama
git branch -M main

# 7. Subir código
git push -u origin main

# 8. Ingresar credenciales cuando se soliciten:
# Username: tu_usuario_github
# Password: ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx (token)
```

---

## 🎯 Checklist Final

Antes de ejecutar los comandos, verifica:

- [ ] ✅ Git está instalado (`git --version`)
- [ ] ✅ Git está configurado (`git config --list`)
- [ ] ✅ Tienes cuenta en GitHub
- [ ] ✅ Has creado el repositorio en GitHub
- [ ] ✅ Has copiado la URL del repositorio
- [ ] ✅ Has generado un Personal Access Token
- [ ] ✅ Estás en el directorio correcto del proyecto
- [ ] ✅ La terminal está abierta
- [ ] ✅ Has reemplazado `TU_USUARIO` con tu usuario real

---

## 🚀 Después de Subir

### 1. Agregar Descripción

1. Ve a tu repositorio en GitHub
2. Clic en ⚙️ junto a "About"
3. Agrega:
   - **Description:** "Professional C# API Gateway for SAP Service Layer with enterprise-grade security"
   - **Website:** (si tienes)
   - **Topics:** `csharp`, `dotnet`, `api-gateway`, `sap`, `service-layer`, `security`, `rest-api`

### 2. Proteger la Rama Main

1. Settings → Branches
2. Add rule
3. Branch name pattern: `main`
4. Marca: ✅ Require pull request reviews before merging

### 3. Agregar Licencia

1. Add file → Create new file
2. Nombre: `LICENSE`
3. Choose a license template → MIT License
4. Commit

### 4. Agregar .github/workflows (CI/CD)

Crea `.github/workflows/dotnet.yml` para automatizar builds y tests.

---

## 📚 Recursos Adicionales

### Documentación
- **Git:** https://git-scm.com/doc
- **GitHub:** https://docs.github.com/
- **GitHub CLI:** https://cli.github.com/

### Tutoriales
- **Git Handbook:** https://guides.github.com/introduction/git-handbook/
- **GitHub Skills:** https://skills.github.com/
- **Learn Git Branching:** https://learngitbranching.js.org/

### Videos
- YouTube: "Git y GitHub para principiantes"
- YouTube: "Cómo subir un proyecto a GitHub"

---

## ✨ Resumen Visual

```
┌──────────────────────────────────────────────────────────┐
│  1. TU COMPUTADORA                                       │
│                                                          │
│  📂 C:\Users\TuNombre\Descargas\improved-api\           │
│     ├── Program.cs                                       │
│     ├── README.md                                        │
│     ├── Controllers/                                     │
│     └── ...                                              │
│                                                          │
│  💻 Terminal/CMD/PowerShell (AQUÍ EJECUTAS COMANDOS)    │
│     > cd C:\Users\TuNombre\Descargas\improved-api       │
│     > git remote add origin https://github.com/...      │
│     > git push -u origin main                           │
│                                                          │
└──────────────────────────────────────────────────────────┘
                          │
                          │ Internet
                          ▼
┌──────────────────────────────────────────────────────────┐
│  2. GITHUB (Nube)                                        │
│                                                          │
│  🌐 https://github.com/TU_USUARIO/api-gateway-...       │
│                                                          │
│     📁 Repositorio (copia de tus archivos)              │
│        ├── Program.cs                                    │
│        ├── README.md                                     │
│        ├── Controllers/                                  │
│        └── ...                                           │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

---

## 🎉 ¡Éxito!

Ahora sabes **exactamente**:
- ✅ **Dónde:** En la terminal de tu computadora
- ✅ **Cuándo:** Después de crear el repositorio en GitHub
- ✅ **Cómo:** Navegando al directorio del proyecto y ejecutando los comandos
- ✅ **Qué:** Reemplazar `TU_USUARIO` con tu usuario real de GitHub

**¡Tu proyecto estará en GitHub en minutos!** 🚀

---

**Última actualización:** 2026-01-05  
**Versión:** 1.0  
**Autor:** API Gateway Developer
