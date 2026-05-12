# ARDE.RealEstate3D

**Producto:** ARDE Real Estate Experience
**MVP:** 0.3
**Proyecto demo:** Residencias Norte · Av. Libertador 2450 · Entrega estimada Diciembre 2027

ARDE.RealEstate3D es una base funcional en Unity para una experiencia inmobiliaria 3D interactiva orientada a web, totems táctiles, salas de venta y presentaciones comerciales. Este entregable MVP 0.3 suma dirección visual premium sobre la base funcional: composición tipo landing/showroom, UI más editorial, iluminación cálida y placeholders más cuidados sin assets externos.

## Stack objetivo

- Unity con C#.
- URP recomendado.
- Build targets: Windows y WebGL.
- UI en castellano.
- Resolución base: 1920x1080.
- Compatible con mouse y pantallas táctiles.
- Sin dependencias externas obligatorias.

## Alcance del MVP 0.3

Incluye la base para:

1. Pantalla inicial premium mockeada.
2. Recorrido 3D guiado por ambientes con transiciones suaves de cámara.
3. Hotspots interactivos para living, cocina, dormitorio, baño y balcón.
4. Panel lateral de información con título, descripción, datos y botón **Consultar**.
5. Modal QR/WhatsApp con mensaje precargado.
6. Sección **Unidades** con cards generadas desde datos serializables.
7. Sección **Amenities** con cards generadas desde datos serializables.
8. Pantalla **Contacto** con QR placeholder y botón WhatsApp.
9. Modo totem con reinicio automático al **HomeScreen** tras 45 segundos sin interacción.
10. Navegación principal: Inicio, Recorrido, Unidades, Amenities y Contacto.
11. Menú de ambientes: General, Living, Cocina, Dormitorio, Baño y Balcón.
12. Departamento placeholder más completo con living, cocina, dormitorio, baño, balcón, ventanas, mobiliario básico y luz cálida.

## Estructura de carpetas

```text
Assets/
  _Project/
    Scenes/
    Scripts/
      Core/
      UI/
      Hotspots/
      Data/
    Prefabs/
      UI/
      Hotspots/
      Environment/
    Materials/
    Models/
    Textures/
    Sprites/
    Fonts/
    Audio/
    Data/
```

### Scripts principales

- `ExperienceManager.cs`: controla la navegación entre pantallas y la apertura de consultas.
- `IdleResetManager.cs`: detecta interacción por mouse, teclado o touch y resetea la experiencia tras 45 segundos de inactividad.
- `CameraPointController.cs`: permite mover suavemente la cámara entre puntos de ambiente y raycastear hotspots.
- `Hotspot.cs`: componente interactivo para seleccionar hotspots 3D.
- `HotspotData.cs`: clase serializable para datos de hotspots.
- `InfoPanelController.cs`: muestra el panel lateral de hotspot y dispara consultas.
- `QRPanelController.cs`: controla el modal QR/WhatsApp.
- `UnitData.cs`: clase serializable para unidades.
- `UnitsController.cs`: genera la lista de unidades.
- `AmenityData.cs`: clase serializable para amenities.
- `AmenitiesController.cs`: genera la grilla de amenities.
- `NavigationButton.cs`: conecta botones de navegación con secciones del `ExperienceManager` y muestra estado activo.
- `CameraPointButton.cs`: conecta botones de ambientes con `CameraPointController` y muestra estado activo.
- `MainSceneBuilder.cs`: herramienta de Editor ubicada en `Assets/_Project/Scripts/Editor` para generar una escena funcional mockeada.

## Cómo abrirlo en Unity

1. Abrir **Unity Hub**.
2. Seleccionar **Add project from disk**.
3. Elegir la carpeta raíz de este repositorio.
4. Usar una versión reciente de Unity compatible con URP. Recomendado: Unity 2022 LTS o superior.
5. Si el proyecto todavía no tiene URP configurado, instalar **Universal RP** desde Package Manager y asignar un URP Asset en `Project Settings > Graphics`.

> Nota: este repositorio entrega código y estructura base. Como las escenas y prefabs de Unity dependen de GUIDs y referencias serializadas generadas por el Editor, se incluye una herramienta para crear la escena desde Unity de forma reproducible.

## Crear la MainScene funcional

1. Abrir el proyecto en Unity.
2. Esperar a que Unity compile los scripts.
3. En la barra superior, ejecutar: **ARDE > Crear MainScene MVP 0.3**.
4. Unity generará y guardará la escena en:

```text
Assets/_Project/Scenes/MainScene.unity
```

La escena creada contiene:

- Cámara principal.
- Luz direccional.
- Canvas 1920x1080 escalable.
- Navegación principal.
- HomeScreen.
- ExperienceScreen.
- UnitsScreen.
- AmenitiesScreen.
- ContactScreen.
- Modal QR/WhatsApp.
- Departamento placeholder construido con primitivas y mobiliario básico.
- Seis puntos de cámara: General, Living, Cocina, Dormitorio, Baño y Balcón.
- Menú de ambientes para navegación guiada tipo tour.
- Cinco hotspots interactivos con visual tipo pin, pulso suave y feedback al seleccionar.
- Managers principales configurados.

## Cómo probar el flujo

1. Abrir `Assets/_Project/Scenes/MainScene.unity`.
2. Presionar **Play**.
3. La app inicia en **HomeScreen**.
4. Tocar o clickear **Iniciar recorrido**.
5. En el recorrido, usar el menú inferior de ambientes para mover la cámara suavemente entre:
   - General.
   - Living.
   - Cocina.
   - Dormitorio.
   - Baño.
   - Balcón.
6. Clickear/tocar los hotspots dorados tipo pin:
   - Living comedor.
   - Cocina integrada.
   - Dormitorio principal.
   - Baño completo.
   - Balcón.
7. Verificar que se abra el panel lateral con título, descripción, datos y botón **Cerrar**.
8. Tocar **Consultar** para abrir el modal QR/WhatsApp.
9. Usar la navegación superior para visitar:
   - Inicio.
   - Recorrido.
   - Unidades.
   - Amenities.
   - Contacto.
10. Dejar la app sin interacción durante 45 segundos y verificar que vuelve automáticamente a **HomeScreen**.

## Datos mockeados incluidos

### Proyecto

- **Nombre:** Residencias Norte.
- **Ubicación:** Av. Libertador 2450.
- **Entrega estimada:** Diciembre 2027.
- **Descripción:** Desarrollo premium con unidades de 1, 2 y 3 ambientes.

### Unidades

| Unidad | Tipología | Superficie | Estado | Precio |
| --- | --- | --- | --- | --- |
| 1A | 2 ambientes | 48 m² | Disponible | USD 78.000 |
| 2B | 3 ambientes | 72 m² | Disponible | USD 112.000 |
| 3C | Monoambiente | 34 m² | Reservada | USD 59.000 |
| 4D | 2 ambientes | 51 m² | Disponible | USD 82.000 |

### Amenities

- SUM.
- Piscina.
- Gimnasio.
- Parrillas.
- Cocheras.
- Seguridad.

## Configuración manual en Inspector

Si preferís armar o extender la escena manualmente:

1. Crear un `GameObject` llamado `ARDE Experience Manager`.
2. Agregar:
   - `ExperienceManager`.
   - `IdleResetManager`.
   - `CameraPointController`.
3. Crear un Canvas con `CanvasScaler`:
   - UI Scale Mode: `Scale With Screen Size`.
   - Reference Resolution: `1920 x 1080`.
   - Match: `0.5`.
4. Crear pantallas como `GameObject` UI y asignarlas en `ExperienceManager`:
   - `HomeScreen`.
   - `ExperienceScreen`.
   - `UnitsScreen`.
   - `AmenitiesScreen`.
   - `ContactScreen`.
5. Crear el panel lateral y asignar sus textos/botón en `InfoPanelController`.
6. Crear el modal y asignar textos/botones en `QRPanelController`.
7. Para cada hotspot 3D:
   - Usar una esfera u otro mesh visible.
   - Agregar `Collider`.
   - Agregar `Hotspot`.
   - Configurar `HotspotData` con título, descripción y detalles.
   - Asignar el `InfoPanelController`.
8. Crear botones de navegación y agregar `NavigationButton`, seleccionando la sección destino.
9. Para unidades y amenities, asignar un contenedor UI a `UnitsController` y `AmenitiesController`. Si no se asignan prefabs de card, los controladores crean cards simples por defecto.


## Dirección visual MVP 0.3

- Estética de sala de ventas digital: blanco cálido, beige, negro suave, gris piedra y dorado champagne.
- HomeScreen con hero comercial, claim, CTAs secundarios y datos destacados del emprendimiento.
- Recorrido con overlay discreto y menú de ambientes tipo pills para mantener foco en el 3D.
- Panel lateral de hotspot diseñado como ficha comercial con fondo claro, línea de acento, botón Consultar, placeholder Ver plano y cierre visible.
- Departamento placeholder enriquecido con marcos de ventana, luminarias, cuadros, planta, alfombra y detalles simples para acercarse a un showroom sin usar assets externos.

## Próximos pasos con assets reales

1. Importar el modelo arquitectónico en `Assets/_Project/Models/` en formato FBX, glTF convertido o paquete propio de Unity.
2. Mantener los managers y pantallas generados por el builder, reemplazando solo el objeto `Departamento Placeholder Premium`.
3. Reposicionar los `CameraPoint` y hotspots para coincidir con el modelo real.
4. Reemplazar materiales placeholder por materiales optimizados para WebGL, manteniendo una paleta sobria y evitando shaders no compatibles.
5. Convertir datos comerciales a ScriptableObjects o JSON cuando el contenido deje de ser mockeado.

## Notas de estabilidad MVP 0.3

- El builder fuerza `Shader.Find("Standard")` para los materiales del departamento placeholder y solo usa `Sprites/Default` o `Unlit/Color` como fallback, evitando shaders URP incompatibles que puedan generar objetos magenta en Unity 2022.3.62f3.
- Las cards de unidades se crean sin prefab externo con `LayoutElement` y `HorizontalLayoutGroup`, mostrando unidad, tipología, superficie, estado, precio y botón **Consultar** a ancho completo.
- La navbar superior usa botones más anchos y padding extra para reducir cortes de texto en 1920x1080.

## Cómo cambiar datos mockeados

- **Hotspots:** editar los `HotspotData` creados en `CreateHotspots` dentro de `Assets/_Project/Scripts/Editor/MainSceneBuilder.cs`. Si la escena ya fue generada, también se pueden ajustar desde cada componente `Hotspot` en el Inspector.
- **Unidades:** editar la lista `units` de `UnitsController` o reemplazarla desde otro script con `SetUnits`.
- **Amenities:** editar la lista `amenities` de `AmenitiesController`.
- Después de modificar datos en el builder, volver a ejecutar **ARDE > Crear MainScene MVP 0.3** para regenerar la escena completa.

## Cómo cambiar teléfono y mensaje WhatsApp

- En el modal, editar `phoneNumber` y `preloadedMessage` en `QRPanelController`.
- En la pantalla Contacto, editar los mismos campos en el componente `WhatsAppButton`.
- El número debe estar en formato internacional sin `+`, por ejemplo `5491112345678`.

## Configurar build WebGL

1. Instalar el módulo **WebGL Build Support** desde Unity Hub si no está disponible.
2. En Unity ir a `File > Build Settings`.
3. Seleccionar **WebGL** y presionar **Switch Platform**.
4. Agregar `Assets/_Project/Scenes/MainScene.unity` a `Scenes In Build`.
5. En `Player Settings`:
   - Resolution: usar 1920x1080 como referencia visual.
   - Desactivar APIs o plugins no necesarios.
   - Revisar compresión según hosting: Brotli o Gzip si el servidor lo soporta.
6. Ejecutar **Build** o **Build And Run**.
7. Para pruebas locales, servir el build desde un servidor HTTP. Evitar abrir `index.html` directo desde disco.

## Configurar build Windows

1. Instalar el módulo **Windows Build Support** desde Unity Hub si hace falta.
2. Ir a `File > Build Settings`.
3. Seleccionar **Windows, Mac, Linux** y elegir target **Windows**.
4. Presionar **Switch Platform**.
5. Agregar `Assets/_Project/Scenes/MainScene.unity` a `Scenes In Build`.
6. En `Player Settings`:
   - Fullscreen Mode: `Full Screen Window` para totems.
   - Default Screen Width: `1920`.
   - Default Screen Height: `1080`.
   - Run In Background: activado si el dispositivo lo requiere.
7. Ejecutar **Build**.

## Roadmap

### MVP 0.1 — Base funcional

- Estructura modular de proyecto.
- UI mockeada en castellano.
- Scene builder para generar MainScene.
- Departamento placeholder con primitivas.
- Hotspots con panel lateral.
- Unidades y amenities mockeadas.
- Contacto QR/WhatsApp.
- Modo totem con reset por inactividad.

### MVP 0.2 — Demo comercial funcional

- Navegación guiada por ambientes con botones visibles y transiciones suaves.
- UI premium básica con colores sobrios, botones grandes, estados activos y textos legibles.
- HomeScreen comercial centrada en Residencias Norte.
- ExperienceScreen con overlay discreto, menú de ambientes y panel lateral menos invasivo.
- Departamento placeholder con living, cocina, dormitorio, baño, balcón, ventanas, barandas y mobiliario básico.
- Cards horizontales de unidades con estado, precio y botón Consultar.
- Amenities con inicial destacada, título y descripción.
- Contacto y QRModal más claros para totem.
- Correcciones técnicas: builder en carpeta Editor, sin `forceModuleActive`, fuente `LegacyRuntime.ttf`, hotspots sin doble disparo y raycast 3D bloqueado cuando el toque está sobre UI.

### MVP 0.3 — Dirección visual premium

- HomeScreen con composición tipo landing premium y datos destacados del proyecto.
- Navbar y menús con estilo más editorial y comercial.
- Recorrido con overlay discreto y ambiente tipo showroom.
- Panel lateral de hotspots como ficha comercial clara.
- Placeholders 3D enriquecidos con marcos, luminarias, cuadros, planta y detalles simples.
- Preparación visual para reemplazar primitivas por modelos reales.

### Versión 1.0 — Producto demo premium

- Visual final URP con iluminación, materiales y postprocesado.
- Integración con CRM o formulario de leads.
- Multi-proyecto / multi-emprendimiento.
- Selector de pisos y unidades.
- Filtros por precio, tipología, estado y superficie.
- Analytics de interacciones.
- Soporte multilenguaje.
- Modo offline para salas de venta.
- Panel de administración de contenido.
