#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using ARDE.RealEstate3D.Data;
using ARDE.RealEstate3D.Hotspots;
using ARDE.RealEstate3D.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.Core
{
    public static class MainSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/MainScene.unity";

        [MenuItem("ARDE/Crear MainScene MVP 0.1")]
        public static void CreateMainScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.78f, 0.78f, 0.78f);

            Camera camera = CreateCamera();
            CreateLight();
            GameObject apartmentRoot = CreateApartmentPlaceholders();

            Canvas canvas = CreateCanvas();
            CreateEventSystem();

            GameObject appRoot = new GameObject("ARDE Experience Manager");
            ExperienceManager manager = appRoot.AddComponent<ExperienceManager>();
            appRoot.AddComponent<IdleResetManager>();
            CameraPointController cameraController = appRoot.AddComponent<CameraPointController>();

            GameObject nav = CreateNavigation(canvas.transform, manager);
            GameObject home = CreateHomeScreen(canvas.transform, manager);
            GameObject experience = CreateExperienceOverlay(canvas.transform);
            GameObject units = CreateUnitsScreen(canvas.transform);
            GameObject amenities = CreateAmenitiesScreen(canvas.transform);
            GameObject contact = CreateContactScreen(canvas.transform);
            InfoPanelController infoPanel = CreateInfoPanel(canvas.transform);
            QRPanelController qrPanel = CreateQRPanel(canvas.transform);

            SetField(manager, "homeScreen", home);
            SetField(manager, "experienceScreen", experience);
            SetField(manager, "unitsScreen", units);
            SetField(manager, "amenitiesScreen", amenities);
            SetField(manager, "contactScreen", contact);
            SetField(manager, "infoPanel", infoPanel);
            SetField(manager, "qrPanel", qrPanel);

            SetField(cameraController, "targetCamera", camera);
            SetField(cameraController, "cameraPoints", CreateCameraPoints());

            CreateHotspots(apartmentRoot.transform, infoPanel);

            nav.transform.SetAsLastSibling();
            qrPanel.transform.SetAsLastSibling();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log($"MainScene MVP creada en {ScenePath}");
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.SetPositionAndRotation(new Vector3(0f, 6f, -9f), Quaternion.Euler(58f, 0f, 0f));
            return camera;
        }

        private static void CreateLight()
        {
            GameObject lightObject = new GameObject("Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        private static void CreateEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            eventSystem.GetComponent<StandaloneInputModule>().forceModuleActive = true;
        }

        private static GameObject CreateNavigation(Transform parent, ExperienceManager manager)
        {
            GameObject nav = CreatePanel("Navegacion Principal", parent, new Color(0.04f, 0.04f, 0.05f, 0.92f));
            RectTransform rect = nav.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.92f);
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            HorizontalLayoutGroup layout = nav.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(32, 32, 12, 12);
            layout.spacing = 18f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            AddNavButton(nav.transform, "Inicio", ExperienceSection.Home, manager);
            AddNavButton(nav.transform, "Recorrido", ExperienceSection.Experience, manager);
            AddNavButton(nav.transform, "Unidades", ExperienceSection.Units, manager);
            AddNavButton(nav.transform, "Amenities", ExperienceSection.Amenities, manager);
            AddNavButton(nav.transform, "Contacto", ExperienceSection.Contact, manager);
            return nav;
        }

        private static void AddNavButton(Transform parent, string label, ExperienceSection section, ExperienceManager manager)
        {
            Button button = CreateButton(label, parent, new Color(0.72f, 0.58f, 0.34f));
            NavigationButton navigationButton = button.gameObject.AddComponent<NavigationButton>();
            navigationButton.Configure(manager, section);
        }

        private static GameObject CreateHomeScreen(Transform parent, ExperienceManager manager)
        {
            GameObject screen = CreateScreen("HomeScreen", parent, new Color(0.02f, 0.025f, 0.03f, 1f));
            CreateText("ARDE Real Estate Experience", screen.transform, 64, TextAnchor.MiddleCenter, new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.72f), Color.white);
            CreateText("Residencias Norte\nAv. Libertador 2450 · Entrega estimada Diciembre 2027\nDesarrollo premium con unidades de 1, 2 y 3 ambientes.", screen.transform, 34, TextAnchor.MiddleCenter, new Vector2(0.16f, 0.28f), new Vector2(0.84f, 0.5f), new Color(0.86f, 0.82f, 0.72f));
            Button start = CreateButton("Iniciar recorrido", screen.transform, new Color(0.72f, 0.58f, 0.34f));
            Anchor(start.GetComponent<RectTransform>(), new Vector2(0.38f, 0.14f), new Vector2(0.62f, 0.24f));
            NavigationButton startNavigation = start.gameObject.AddComponent<NavigationButton>();
            startNavigation.Configure(manager, ExperienceSection.Experience);
            return screen;
        }

        private static GameObject CreateExperienceOverlay(Transform parent)
        {
            GameObject screen = CreateScreen("ExperienceScreen", parent, new Color(0f, 0f, 0f, 0f));
            Image image = screen.GetComponent<Image>();
            image.raycastTarget = false;
            CreateText("Recorrido 3D · Tocá los hotspots para ver detalles", screen.transform, 30, TextAnchor.UpperLeft, new Vector2(0.03f, 0.82f), new Vector2(0.52f, 0.9f), Color.white);
            return screen;
        }

        private static GameObject CreateUnitsScreen(Transform parent)
        {
            GameObject screen = CreateScreen("UnitsScreen", parent, new Color(0.92f, 0.9f, 0.84f, 0.98f));
            CreateText("Unidades disponibles", screen.transform, 52, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.78f), new Vector2(0.8f, 0.88f), new Color(0.07f, 0.07f, 0.08f));
            GameObject list = CreatePanel("UnitsList", screen.transform, new Color(0f, 0f, 0f, 0f));
            Anchor(list.GetComponent<RectTransform>(), new Vector2(0.18f, 0.18f), new Vector2(0.82f, 0.74f));
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 18f;
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;
            UnitsController controller = screen.AddComponent<UnitsController>();
            SetField(controller, "listContainer", list.transform);
            return screen;
        }

        private static GameObject CreateAmenitiesScreen(Transform parent)
        {
            GameObject screen = CreateScreen("AmenitiesScreen", parent, new Color(0.9f, 0.86f, 0.78f, 0.98f));
            CreateText("Amenities", screen.transform, 52, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.78f), new Vector2(0.8f, 0.88f), new Color(0.07f, 0.07f, 0.08f));
            GameObject grid = CreatePanel("AmenitiesGrid", screen.transform, new Color(0f, 0f, 0f, 0f));
            Anchor(grid.GetComponent<RectTransform>(), new Vector2(0.15f, 0.2f), new Vector2(0.85f, 0.72f));
            GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(360f, 150f);
            layout.spacing = new Vector2(24f, 24f);
            AmenitiesController controller = screen.AddComponent<AmenitiesController>();
            SetField(controller, "gridContainer", grid.transform);
            return screen;
        }

        private static GameObject CreateContactScreen(Transform parent)
        {
            GameObject screen = CreateScreen("ContactScreen", parent, new Color(0.04f, 0.04f, 0.05f, 0.98f));
            CreateText("Hablemos de tu próxima inversión", screen.transform, 50, TextAnchor.MiddleCenter, new Vector2(0.14f, 0.74f), new Vector2(0.86f, 0.86f), Color.white);
            CreateText("Escaneá el QR o tocá WhatsApp para coordinar una reunión con un asesor de Residencias Norte.", screen.transform, 32, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.58f), new Vector2(0.8f, 0.7f), new Color(0.86f, 0.82f, 0.72f));
            CreateQRPlaceholder(screen.transform, new Vector2(0.42f, 0.26f), new Vector2(0.58f, 0.54f));
            Button whatsapp = CreateButton("WhatsApp", screen.transform, new Color(0.12f, 0.68f, 0.33f));
            Anchor(whatsapp.GetComponent<RectTransform>(), new Vector2(0.38f, 0.13f), new Vector2(0.62f, 0.22f));
            whatsapp.gameObject.AddComponent<WhatsAppButton>();
            return screen;
        }

        private static InfoPanelController CreateInfoPanel(Transform parent)
        {
            GameObject panel = CreatePanel("InfoPanel", parent, new Color(0.05f, 0.05f, 0.06f, 0.96f));
            Anchor(panel.GetComponent<RectTransform>(), new Vector2(0.68f, 0.08f), new Vector2(1f, 0.92f));
            Text title = CreateText("Título", panel.transform, 40, TextAnchor.UpperLeft, new Vector2(0.08f, 0.76f), new Vector2(0.92f, 0.94f), Color.white);
            Text description = CreateText("Descripción", panel.transform, 28, TextAnchor.UpperLeft, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.74f), new Color(0.88f, 0.88f, 0.88f));
            Text details = CreateText("Datos", panel.transform, 27, TextAnchor.UpperLeft, new Vector2(0.08f, 0.22f), new Vector2(0.92f, 0.44f), new Color(0.83f, 0.75f, 0.58f));
            Button consult = CreateButton("Consultar", panel.transform, new Color(0.72f, 0.58f, 0.34f));
            Anchor(consult.GetComponent<RectTransform>(), new Vector2(0.12f, 0.07f), new Vector2(0.88f, 0.17f));
            InfoPanelController controller = panel.AddComponent<InfoPanelController>();
            SetField(controller, "panelRoot", panel);
            SetField(controller, "titleText", title);
            SetField(controller, "descriptionText", description);
            SetField(controller, "detailsText", details);
            SetField(controller, "consultButton", consult);
            return controller;
        }

        private static QRPanelController CreateQRPanel(Transform parent)
        {
            GameObject panel = CreatePanel("QRWhatsAppModal", parent, new Color(0f, 0f, 0f, 0.72f));
            Anchor(panel.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
            GameObject card = CreatePanel("ModalCard", panel.transform, new Color(0.98f, 0.96f, 0.9f, 1f));
            Anchor(card.GetComponent<RectTransform>(), new Vector2(0.32f, 0.18f), new Vector2(0.68f, 0.78f));
            Text title = CreateText("Contactanos por WhatsApp", card.transform, 38, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.92f), new Color(0.08f, 0.08f, 0.08f));
            Text body = CreateText("Escaneá el QR o tocá el botón.", card.transform, 26, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.64f), new Vector2(0.92f, 0.76f), new Color(0.1f, 0.1f, 0.1f));
            CreateQRPlaceholder(card.transform, new Vector2(0.34f, 0.32f), new Vector2(0.66f, 0.62f));
            Text message = CreateText("Mensaje precargado", card.transform, 22, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.21f), new Vector2(0.92f, 0.3f), new Color(0.2f, 0.2f, 0.2f));
            Button whatsapp = CreateButton("Abrir WhatsApp", card.transform, new Color(0.12f, 0.68f, 0.33f));
            Anchor(whatsapp.GetComponent<RectTransform>(), new Vector2(0.15f, 0.08f), new Vector2(0.62f, 0.18f));
            Button close = CreateButton("Cerrar", card.transform, new Color(0.35f, 0.35f, 0.38f));
            Anchor(close.GetComponent<RectTransform>(), new Vector2(0.66f, 0.08f), new Vector2(0.85f, 0.18f));
            QRPanelController controller = panel.AddComponent<QRPanelController>();
            SetField(controller, "panelRoot", panel);
            SetField(controller, "titleText", title);
            SetField(controller, "bodyText", body);
            SetField(controller, "messageText", message);
            SetField(controller, "whatsappButton", whatsapp);
            SetField(controller, "closeButton", close);
            return controller;
        }

        private static GameObject CreateApartmentPlaceholders()
        {
            GameObject root = new GameObject("Departamento Placeholder");
            CreateCube("Piso", root.transform, new Vector3(0f, -0.05f, 0f), new Vector3(10f, 0.1f, 7f), new Color(0.7f, 0.68f, 0.62f));
            CreateCube("Pared Fondo", root.transform, new Vector3(0f, 1.5f, 3.5f), new Vector3(10f, 3f, 0.15f), new Color(0.86f, 0.84f, 0.78f));
            CreateCube("Pared Izquierda", root.transform, new Vector3(-5f, 1.5f, 0f), new Vector3(0.15f, 3f, 7f), new Color(0.86f, 0.84f, 0.78f));
            CreateCube("Pared Derecha", root.transform, new Vector3(5f, 1.5f, 0f), new Vector3(0.15f, 3f, 7f), new Color(0.86f, 0.84f, 0.78f));
            CreateCube("Divisor Dormitorio", root.transform, new Vector3(-1.2f, 1.3f, 0.9f), new Vector3(0.12f, 2.6f, 4f), new Color(0.82f, 0.8f, 0.74f));
            CreateCube("Mesada Cocina", root.transform, new Vector3(2.8f, 0.45f, 2.3f), new Vector3(3f, 0.9f, 0.7f), new Color(0.25f, 0.25f, 0.25f));
            CreateCube("Cama Placeholder", root.transform, new Vector3(-3.1f, 0.25f, 1f), new Vector3(2.2f, 0.5f, 2.2f), new Color(0.58f, 0.62f, 0.72f));
            CreateCube("Balcón Placeholder", root.transform, new Vector3(0f, 0.05f, -4.2f), new Vector3(4f, 0.1f, 1.3f), new Color(0.62f, 0.65f, 0.66f));
            return root;
        }

        private static void CreateHotspots(Transform parent, InfoPanelController panel)
        {
            CreateHotspot(parent, panel, "Hotspot Living comedor", new Vector3(0.5f, 1.4f, -0.7f), new HotspotData("Living comedor", "Espacio principal con iluminación natural, salida al balcón y distribución integrada.", new[] { "Superficie: 18 m²", "Piso: porcelanato", "Orientación: norte" }));
            CreateHotspot(parent, panel, "Hotspot Cocina integrada", new Vector3(2.8f, 1.6f, 1.8f), new HotspotData("Cocina integrada", "Cocina moderna con muebles bajo mesada, alacena y espacio para lavarropas.", new[] { "Mesada: granito", "Muebles: melamina premium", "Conexión: eléctrica/gas según unidad" }));
            CreateHotspot(parent, panel, "Hotspot Dormitorio principal", new Vector3(-3.1f, 1.4f, 1f), new HotspotData("Dormitorio principal", "Ambiente cómodo con placard empotrado y ventana al contrafrente.", new[] { "Superficie: 12 m²", "Placard: incluido", "Ventilación natural" }));
            CreateHotspot(parent, panel, "Hotspot Baño completo", new Vector3(-3.2f, 1.4f, 2.8f), new HotspotData("Baño completo", "Baño moderno con revestimientos de primera línea y grifería cromada.", new[] { "Ducha", "Vanitory", "Extractor / ventilación" }));
            CreateHotspot(parent, panel, "Hotspot Balcón", new Vector3(0f, 1.2f, -4.2f), new HotspotData("Balcón", "Balcón al frente con vista abierta, ideal para expansión del living.", new[] { "Superficie: 4 m²", "Baranda vidriada", "Orientación norte" }));
        }

        private static void CreateHotspot(Transform parent, InfoPanelController panel, string name, Vector3 position, HotspotData data)
        {
            GameObject hotspotObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hotspotObject.name = name;
            hotspotObject.transform.SetParent(parent, false);
            hotspotObject.transform.position = position;
            hotspotObject.transform.localScale = Vector3.one * 0.35f;
            hotspotObject.GetComponent<Renderer>().sharedMaterial = CreateMaterial(new Color(0.95f, 0.68f, 0.18f));
            Hotspot hotspot = hotspotObject.AddComponent<Hotspot>();
            hotspot.Configure(data, panel);
        }

        private static Transform[] CreateCameraPoints()
        {
            var points = new List<Transform>();
            points.Add(CreatePoint("CameraPoint General", new Vector3(0f, 6f, -9f), Quaternion.Euler(58f, 0f, 0f)));
            points.Add(CreatePoint("CameraPoint Living", new Vector3(0f, 3.2f, -5.2f), Quaternion.Euler(34f, 0f, 0f)));
            return points.ToArray();
        }

        private static Transform CreatePoint(string name, Vector3 position, Quaternion rotation)
        {
            GameObject point = new GameObject(name);
            point.transform.SetPositionAndRotation(position, rotation);
            return point.transform;
        }

        private static GameObject CreateScreen(string name, Transform parent, Color color)
        {
            GameObject screen = CreatePanel(name, parent, color);
            Anchor(screen.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
            return screen;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private static Text CreateText(string value, Transform parent, int size, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            Anchor(textObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
            return text;
        }

        private static Button CreateButton(string label, Transform parent, Color color)
        {
            GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = color;
            Button button = buttonObject.GetComponent<Button>();
            CreateText(label, buttonObject.transform, 28, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Color.white);
            return button;
        }

        private static void CreateQRPlaceholder(Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject qr = CreatePanel("QR Placeholder", parent, Color.white);
            Anchor(qr.GetComponent<RectTransform>(), anchorMin, anchorMax);
            CreateText("QR", qr.transform, 56, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Color.black);
        }

        private static GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = CreateMaterial(color);
            return cube;
        }

        private static Material CreateMaterial(Color color)
        {
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            material.color = color;
            return material;
        }

        private static void Anchor(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null)
            {
                throw new MissingFieldException(target.GetType().Name, fieldName);
            }

            field.SetValue(target, value);
        }
    }
}
#endif
