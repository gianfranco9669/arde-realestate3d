#if UNITY_EDITOR
using System;
using System.Reflection;
using ARDE.RealEstate3D.Core;
using ARDE.RealEstate3D.Data;
using ARDE.RealEstate3D.Hotspots;
using ARDE.RealEstate3D.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ARDE.RealEstate3D.EditorTools
{
    public static class MainSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/MainScene.unity";
        private const string SeaViewScenePath = "Assets/ModernArchViz_SeaView/SeaViewArchViz/Scenes/SampleScene.unity";

        private static readonly string[] SeaViewRootNames =
        {
            "HouseExterior",
            "Living Area",
            "Laundry Room",
            "Study",
            "Bathrooms",
            "Bedrooms",
            "Hallways",
            "Global Light Switches",
            "Global Wall Plugs",
            "Area Lights",
            "Reflection Probes",
            "LED_PointLights"
        };

        private static readonly Color SoftBlack = new Color(0.055f, 0.052f, 0.047f, 1f);
        private static readonly Color WarmWhite = new Color(0.965f, 0.945f, 0.895f, 1f);
        private static readonly Color Beige = new Color(0.78f, 0.70f, 0.56f, 1f);
        private static readonly Color Champagne = new Color(0.82f, 0.70f, 0.50f, 1f);
        private static readonly Color Stone = new Color(0.55f, 0.53f, 0.49f, 1f);

        [MenuItem("ARDE/Crear MainScene MVP 0.3")]
        public static void CreateMainScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(0.82f, 0.78f, 0.68f);
            RenderSettings.skybox = null;

            Camera camera = CreateCamera();
            CreateLighting();
            GameObject environmentRoot = TryCreateSeaViewEnvironment();
            bool usingSeaViewEnvironment = environmentRoot != null;

            if (!usingSeaViewEnvironment)
            {
                environmentRoot = CreateApartmentPlaceholders();
                Debug.LogWarning($"No se encontró o no se pudo integrar Modern ArchViz: Sea View en {SeaViewScenePath}. Se generó el placeholder premium como fallback.");
            }

            Canvas canvas = CreateCanvas();
            CreateEventSystem();

            GameObject appRoot = new GameObject("ARDE Experience Manager");
            ExperienceManager manager = appRoot.AddComponent<ExperienceManager>();
            appRoot.AddComponent<IdleResetManager>();
            CameraPointController cameraController = appRoot.AddComponent<CameraPointController>();

            GameObject navigation = CreateNavigation(canvas.transform, manager);
            GameObject home = CreateHomeScreen(canvas.transform, manager);
            GameObject experience = CreateExperienceOverlay(canvas.transform, cameraController);
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
            SetField(cameraController, "cameraPoints", CreateCameraPoints(environmentRoot, usingSeaViewEnvironment));

            CreateHotspots(environmentRoot, infoPanel, usingSeaViewEnvironment);

            navigation.transform.SetAsLastSibling();
            infoPanel.transform.SetAsLastSibling();
            qrPanel.transform.SetAsLastSibling();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();
            Debug.Log($"MainScene MVP 0.3 creada en {ScenePath}");
        }

        [MenuItem("ARDE/Crear MainScene MVP 0.2")]
        public static void CreateMainSceneMvp02()
        {
            CreateMainScene();
        }

        private static Camera CreateCamera()
        {
            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.88f, 0.85f, 0.78f);
            camera.fieldOfView = 43f;
            camera.transform.SetPositionAndRotation(new Vector3(0f, 4.6f, -8.8f), Quaternion.Euler(34f, 0f, 0f));
            return camera;
        }

        private static void CreateLighting()
        {
            GameObject sunObject = new GameObject("Key Warm Directional Light");
            Light sun = sunObject.AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.color = new Color(1f, 0.88f, 0.68f);
            sun.intensity = 1.55f;
            sun.transform.rotation = Quaternion.Euler(42f, -28f, 0f);

            CreatePointLight("Living Warm Fill", new Vector3(0.2f, 2.6f, -1.6f), 6.0f, 1.45f);
            CreatePointLight("Kitchen Accent Fill", new Vector3(3.2f, 2.4f, 1.6f), 4.2f, 0.95f);
            CreatePointLight("Bedroom Warm Fill", new Vector3(-3.2f, 2.4f, 0.6f), 4.0f, 0.9f);
        }

        private static void CreatePointLight(string name, Vector3 position, float range, float intensity)
        {
            GameObject lightObject = new GameObject(name);
            lightObject.transform.position = position;
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.82f, 0.58f);
            light.range = range;
            light.intensity = intensity;
        }


        private static GameObject TryCreateSeaViewEnvironment()
        {
            SceneAsset seaViewSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SeaViewScenePath);
            if (seaViewSceneAsset == null)
            {
                return null;
            }

            Scene targetScene = SceneManager.GetActiveScene();
            Scene sourceScene = EditorSceneManager.OpenScene(SeaViewScenePath, OpenSceneMode.Additive);

            GameObject environmentRoot = new GameObject("SeaView Environment");
            SceneManager.MoveGameObjectToScene(environmentRoot, targetScene);

            int copiedObjects = 0;
            foreach (string rootName in SeaViewRootNames)
            {
                GameObject sourceRoot = FindRootObject(sourceScene, rootName);
                if (sourceRoot == null)
                {
                    Debug.LogWarning($"No se encontró '{rootName}' en {SeaViewScenePath}. Revisar jerarquía del asset Sea View.");
                    continue;
                }

                GameObject copy = UnityEngine.Object.Instantiate(sourceRoot);
                copy.name = rootName;
                SceneManager.MoveGameObjectToScene(copy, targetScene);
                copy.transform.SetParent(environmentRoot.transform, true);
                copiedObjects++;
            }

            EditorSceneManager.CloseScene(sourceScene, true);

            if (copiedObjects == 0)
            {
                UnityEngine.Object.DestroyImmediate(environmentRoot);
                return null;
            }

            Debug.Log($"SeaView Environment creado con {copiedObjects} grupos desde Modern ArchViz: Sea View.");
            return environmentRoot;
        }

        private static GameObject FindRootObject(Scene scene, string rootName)
        {
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                if (rootObject.name == rootName)
                {
                    return rootObject;
                }
            }

            return null;
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
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static GameObject CreateNavigation(Transform parent, ExperienceManager manager)
        {
            GameObject nav = CreatePanel("Navegacion Principal", parent, new Color(0.98f, 0.96f, 0.90f, 0.94f));
            Anchor(nav.GetComponent<RectTransform>(), new Vector2(0f, 0.895f), Vector2.one);

            HorizontalLayoutGroup layout = nav.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(48, 48, 18, 18);
            layout.spacing = 12f;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = true;
            layout.childAlignment = TextAnchor.MiddleCenter;

            AddNavButton(nav.transform, "Inicio", ExperienceSection.Home, manager);
            AddNavButton(nav.transform, "Recorrido", ExperienceSection.Experience, manager);
            AddNavButton(nav.transform, "Unidades", ExperienceSection.Units, manager);
            AddNavButton(nav.transform, "Amenities", ExperienceSection.Amenities, manager);
            AddNavButton(nav.transform, "Contacto", ExperienceSection.Contact, manager);
            return nav;
        }

        private static void AddNavButton(Transform parent, string label, ExperienceSection section, ExperienceManager manager)
        {
            Button button = CreateButton(label, parent, new Color(0.88f, 0.83f, 0.72f, 0.95f), 26, SoftBlack);
            LayoutElement layoutElement = button.gameObject.AddComponent<LayoutElement>();
            layoutElement.minWidth = 330f;
            layoutElement.preferredWidth = 340f;
            NavigationButton navigationButton = button.gameObject.AddComponent<NavigationButton>();
            navigationButton.Configure(manager, section);
        }

        private static GameObject CreateHomeScreen(Transform parent, ExperienceManager manager)
        {
            GameObject screen = CreateScreen("HomeScreen", parent, WarmWhite);
            CreatePanel("Home Ambient Panel", screen.transform, new Color(0.91f, 0.87f, 0.78f, 1f), new Vector2(0.58f, 0.08f), new Vector2(1f, 0.895f));
            CreatePanel("Home Hero Card", screen.transform, new Color(0.055f, 0.052f, 0.047f, 0.96f), new Vector2(0.08f, 0.18f), new Vector2(0.58f, 0.78f));
            CreatePanel("Home Champagne Line", screen.transform, Champagne, new Vector2(0.105f, 0.715f), new Vector2(0.31f, 0.722f));

            CreateText("RESIDENCIAS NORTE", screen.transform, 68, FontStyle.Bold, TextAnchor.UpperLeft, new Vector2(0.105f, 0.60f), new Vector2(0.55f, 0.71f), WarmWhite);
            CreateText("Experiencia inmobiliaria interactiva", screen.transform, 34, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.108f, 0.54f), new Vector2(0.53f, 0.60f), Champagne);
            CreateText("Explorá unidades, amenities y recorridos desde una experiencia 3D para sala de ventas y web.", screen.transform, 28, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.108f, 0.43f), new Vector2(0.50f, 0.53f), new Color(0.88f, 0.84f, 0.76f));

            Button start = CreateButton("Iniciar recorrido", screen.transform, Champagne, 31, SoftBlack);
            Anchor(start.GetComponent<RectTransform>(), new Vector2(0.108f, 0.31f), new Vector2(0.35f, 0.39f));
            NavigationButton startNavigation = start.gameObject.AddComponent<NavigationButton>();
            startNavigation.Configure(manager, ExperienceSection.Experience);

            CreateSecondaryHomeButton(screen.transform, manager, "Ver unidades", ExperienceSection.Units, 0.108f);
            CreateSecondaryHomeButton(screen.transform, manager, "Amenities", ExperienceSection.Amenities, 0.255f);
            CreateSecondaryHomeButton(screen.transform, manager, "Contacto", ExperienceSection.Contact, 0.402f);

            CreateStatCard(screen.transform, "1, 2 y 3", "ambientes", new Vector2(0.63f, 0.61f), new Vector2(0.79f, 0.73f));
            CreateStatCard(screen.transform, "Dic 2027", "entrega", new Vector2(0.80f, 0.61f), new Vector2(0.96f, 0.73f));
            CreateStatCard(screen.transform, "Av. Libertador", "2450", new Vector2(0.63f, 0.46f), new Vector2(0.96f, 0.58f));
            CreatePanel("Home Visual Window", screen.transform, new Color(0.72f, 0.69f, 0.62f, 0.55f), new Vector2(0.66f, 0.20f), new Vector2(0.94f, 0.42f));
            CreatePanel("Home Visual Frame A", screen.transform, Champagne, new Vector2(0.675f, 0.215f), new Vector2(0.685f, 0.405f));
            CreatePanel("Home Visual Frame B", screen.transform, Champagne, new Vector2(0.675f, 0.395f), new Vector2(0.925f, 0.405f));
            CreateText("SHOWROOM DIGITAL", screen.transform, 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.70f, 0.27f), new Vector2(0.90f, 0.35f), new Color(0.18f, 0.16f, 0.12f));
            return screen;
        }


        private static void CreateSecondaryHomeButton(Transform parent, ExperienceManager manager, string label, ExperienceSection section, float minX)
        {
            Button button = CreateButton(label, parent, new Color(0.10f, 0.095f, 0.085f, 0.95f), 22, WarmWhite);
            Anchor(button.GetComponent<RectTransform>(), new Vector2(minX, 0.22f), new Vector2(minX + 0.13f, 0.275f));
            NavigationButton navigationButton = button.gameObject.AddComponent<NavigationButton>();
            navigationButton.Configure(manager, section);
        }

        private static void CreateStatCard(Transform parent, string value, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject card = CreatePanel($"Dato {label}", parent, new Color(0.98f, 0.96f, 0.90f, 0.96f), anchorMin, anchorMax);
            CreatePanel("Dato Line", card.transform, Champagne, new Vector2(0.06f, 0.12f), new Vector2(0.08f, 0.88f));
            CreateText(value, card.transform, 32, FontStyle.Bold, TextAnchor.UpperLeft, new Vector2(0.14f, 0.45f), new Vector2(0.92f, 0.82f), SoftBlack);
            CreateText(label, card.transform, 21, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.14f, 0.18f), new Vector2(0.92f, 0.48f), Stone);
        }

        private static GameObject CreateExperienceOverlay(Transform parent, CameraPointController controller)
        {
            GameObject screen = CreateScreen("ExperienceScreen", parent, new Color(0f, 0f, 0f, 0f));
            Image image = screen.GetComponent<Image>();
            image.raycastTarget = false;

            GameObject header = CreatePanel("Experience Header", screen.transform, new Color(0.98f, 0.96f, 0.90f, 0.88f));
            Anchor(header.GetComponent<RectTransform>(), new Vector2(0.035f, 0.74f), new Vector2(0.42f, 0.875f));
            CreatePanel("Experience Header Line", header.transform, Champagne, new Vector2(0.045f, 0.14f), new Vector2(0.055f, 0.84f));
            CreateText("Recorrido 3D", header.transform, 36, FontStyle.Bold, TextAnchor.UpperLeft, new Vector2(0.09f, 0.50f), new Vector2(0.94f, 0.86f), SoftBlack);
            CreateText("Elegí un ambiente o tocá los puntos destacados", header.transform, 22, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.09f, 0.18f), new Vector2(0.94f, 0.48f), Stone);

            GameObject tourMenu = CreatePanel("Menu Ambientes", screen.transform, new Color(0.98f, 0.96f, 0.90f, 0.92f));
            Anchor(tourMenu.GetComponent<RectTransform>(), new Vector2(0.18f, 0.055f), new Vector2(0.82f, 0.145f));
            HorizontalLayoutGroup layout = tourMenu.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 12, 12);
            layout.spacing = 12f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;

            string[] labels = { "General", "Living", "Cocina", "Dormitorio", "Baño", "Balcón" };
            for (int i = 0; i < labels.Length; i++)
            {
                Button button = CreateButton(labels[i], tourMenu.transform, new Color(0.88f, 0.83f, 0.72f, 0.95f), 22, new Color(0.12f, 0.10f, 0.08f));
                CameraPointButton cameraButton = button.gameObject.AddComponent<CameraPointButton>();
                cameraButton.Configure(controller, i);
            }

            return screen;
        }


        private static GameObject CreateUnitsScreen(Transform parent)
        {
            GameObject screen = CreateScreen("UnitsScreen", parent, WarmWhite);
            CreatePanel("Units Accent", screen.transform, new Color(0.90f, 0.85f, 0.75f, 1f), new Vector2(0f, 0f), new Vector2(0.18f, 0.895f));
            CreateText("Unidades", screen.transform, 58, FontStyle.Bold, TextAnchor.UpperLeft, new Vector2(0.20f, 0.78f), new Vector2(0.66f, 0.88f), SoftBlack);
            CreateText("Disponibilidad comercial para sala de ventas, con tipologías y valores destacados.", screen.transform, 27, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.20f, 0.715f), new Vector2(0.76f, 0.78f), Stone);

            GameObject list = CreatePanel("UnitsList", screen.transform, new Color(0f, 0f, 0f, 0f));
            Anchor(list.GetComponent<RectTransform>(), new Vector2(0.16f, 0.12f), new Vector2(0.90f, 0.68f));
            VerticalLayoutGroup layout = list.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(18, 18, 10, 10);
            layout.spacing = 18f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            UnitsController unitsController = screen.AddComponent<UnitsController>();
            SetField(unitsController, "listContainer", list.transform);
            return screen;
        }


        private static GameObject CreateAmenitiesScreen(Transform parent)
        {
            GameObject screen = CreateScreen("AmenitiesScreen", parent, WarmWhite);
            CreatePanel("Amenities Accent", screen.transform, new Color(0.90f, 0.85f, 0.75f, 1f), new Vector2(0.82f, 0f), new Vector2(1f, 0.895f));
            CreateText("Amenities", screen.transform, 58, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.78f), new Vector2(0.8f, 0.88f), SoftBlack);
            CreateText("Espacios pensados para vivir mejor todos los días.", screen.transform, 28, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0.2f, 0.72f), new Vector2(0.8f, 0.78f), Stone);

            GameObject grid = CreatePanel("AmenitiesGrid", screen.transform, new Color(0f, 0f, 0f, 0f));
            Anchor(grid.GetComponent<RectTransform>(), new Vector2(0.12f, 0.17f), new Vector2(0.88f, 0.68f));
            GridLayoutGroup layout = grid.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(500f, 172f);
            layout.spacing = new Vector2(28f, 28f);
            layout.childAlignment = TextAnchor.MiddleCenter;

            AmenitiesController controller = screen.AddComponent<AmenitiesController>();
            SetField(controller, "gridContainer", grid.transform);
            return screen;
        }


        private static GameObject CreateContactScreen(Transform parent)
        {
            GameObject screen = CreateScreen("ContactScreen", parent, WarmWhite);
            CreatePanel("Contact Dark Panel", screen.transform, SoftBlack, new Vector2(0.08f, 0.16f), new Vector2(0.52f, 0.76f));
            CreatePanel("Contact Gold Line", screen.transform, Champagne, new Vector2(0.105f, 0.69f), new Vector2(0.31f, 0.697f));
            CreateText("Coordiná una visita", screen.transform, 54, FontStyle.Bold, TextAnchor.UpperLeft, new Vector2(0.105f, 0.58f), new Vector2(0.48f, 0.68f), WarmWhite);
            CreateText("Escaneá el QR con tu celular o tocá WhatsApp para hablar con un asesor de Residencias Norte.", screen.transform, 28, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.108f, 0.43f), new Vector2(0.47f, 0.55f), new Color(0.88f, 0.84f, 0.76f));
            CreateText("Av. Libertador 2450 · Entrega diciembre 2027", screen.transform, 23, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.108f, 0.34f), new Vector2(0.47f, 0.40f), Champagne);
            CreateQRPlaceholder(screen.transform, new Vector2(0.63f, 0.32f), new Vector2(0.82f, 0.60f));
            CreateText("Escaneá el QR con tu celular", screen.transform, 25, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.58f, 0.62f), new Vector2(0.87f, 0.68f), SoftBlack);
            Button whatsapp = CreateButton("Abrir WhatsApp", screen.transform, new Color(0.12f, 0.68f, 0.33f), 29);
            Anchor(whatsapp.GetComponent<RectTransform>(), new Vector2(0.61f, 0.20f), new Vector2(0.84f, 0.29f));
            whatsapp.gameObject.AddComponent<WhatsAppButton>();
            return screen;
        }


        private static InfoPanelController CreateInfoPanel(Transform parent)
        {
            GameObject panel = CreatePanel("InfoPanel", parent, new Color(0.98f, 0.96f, 0.90f, 0.98f));
            Anchor(panel.GetComponent<RectTransform>(), new Vector2(0.705f, 0.13f), new Vector2(0.965f, 0.83f));
            CreatePanel("InfoPanel Accent", panel.transform, Champagne, new Vector2(0f, 0f), new Vector2(0.018f, 1f));
            Text title = CreateText("Título", panel.transform, 36, FontStyle.Bold, TextAnchor.UpperLeft, new Vector2(0.08f, 0.78f), new Vector2(0.88f, 0.93f), SoftBlack);
            Text description = CreateText("Descripción", panel.transform, 24, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.08f, 0.52f), new Vector2(0.92f, 0.76f), new Color(0.23f, 0.21f, 0.18f));
            Text details = CreateText("Datos", panel.transform, 23, FontStyle.Normal, TextAnchor.UpperLeft, new Vector2(0.08f, 0.28f), new Vector2(0.92f, 0.50f), Stone);
            Button consult = CreateButton("Consultar", panel.transform, Champagne, 26, SoftBlack);
            Anchor(consult.GetComponent<RectTransform>(), new Vector2(0.08f, 0.10f), new Vector2(0.55f, 0.19f));
            Button plan = CreateButton("Ver plano", panel.transform, new Color(0.86f, 0.81f, 0.70f, 1f), 23, SoftBlack);
            Anchor(plan.GetComponent<RectTransform>(), new Vector2(0.58f, 0.10f), new Vector2(0.86f, 0.19f));
            Button close = CreateButton("×", panel.transform, new Color(0.18f, 0.17f, 0.15f, 1f), 28);
            Anchor(close.GetComponent<RectTransform>(), new Vector2(0.88f, 0.88f), new Vector2(0.96f, 0.96f));

            InfoPanelController controller = panel.AddComponent<InfoPanelController>();
            SetField(controller, "panelRoot", panel);
            SetField(controller, "titleText", title);
            SetField(controller, "descriptionText", description);
            SetField(controller, "detailsText", details);
            SetField(controller, "consultButton", consult);
            SetField(controller, "closeButton", close);
            return controller;
        }


        private static QRPanelController CreateQRPanel(Transform parent)
        {
            GameObject panel = CreatePanel("QRWhatsAppModal", parent, new Color(0f, 0f, 0f, 0.68f));
            Anchor(panel.GetComponent<RectTransform>(), Vector2.zero, Vector2.one);
            GameObject card = CreatePanel("ModalCard", panel.transform, WarmWhite);
            Anchor(card.GetComponent<RectTransform>(), new Vector2(0.30f, 0.12f), new Vector2(0.70f, 0.84f));
            CreatePanel("Modal Top Line", card.transform, Champagne, new Vector2(0.12f, 0.88f), new Vector2(0.88f, 0.895f));
            Text title = CreateText("Escaneá el QR con tu celular", card.transform, 36, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.08f, 0.78f), new Vector2(0.92f, 0.88f), SoftBlack);
            Text body = CreateText("Un asesor comercial te enviará disponibilidad, planos y valores actualizados.", card.transform, 24, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0.12f, 0.68f), new Vector2(0.88f, 0.76f), Stone);
            CreateQRPlaceholder(card.transform, new Vector2(0.30f, 0.34f), new Vector2(0.70f, 0.66f));
            Text message = CreateText("Mensaje precargado: Hola, quiero recibir información sobre Residencias Norte.", card.transform, 21, FontStyle.Normal, TextAnchor.MiddleCenter, new Vector2(0.12f, 0.23f), new Vector2(0.88f, 0.32f), new Color(0.24f, 0.20f, 0.15f));
            Button whatsapp = CreateButton("Abrir WhatsApp", card.transform, new Color(0.12f, 0.68f, 0.33f), 26);
            Anchor(whatsapp.GetComponent<RectTransform>(), new Vector2(0.17f, 0.09f), new Vector2(0.62f, 0.19f));
            Button close = CreateButton("Cerrar", card.transform, SoftBlack, 24);
            Anchor(close.GetComponent<RectTransform>(), new Vector2(0.66f, 0.09f), new Vector2(0.84f, 0.19f));

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
            GameObject root = new GameObject("Departamento Placeholder Premium");
            Material floor = CreateMaterial(new Color(0.78f, 0.74f, 0.66f));
            Material wall = CreateMaterial(new Color(0.90f, 0.87f, 0.80f));
            Material wood = CreateMaterial(new Color(0.42f, 0.30f, 0.20f));
            Material dark = CreateMaterial(new Color(0.13f, 0.13f, 0.12f));
            Material fabric = CreateMaterial(new Color(0.40f, 0.41f, 0.40f));
            Material glass = CreateMaterial(new Color(0.62f, 0.75f, 0.78f));
            Material white = CreateMaterial(new Color(0.94f, 0.92f, 0.88f));
            Material champagne = CreateMaterial(Champagne);
            Material plant = CreateMaterial(new Color(0.28f, 0.40f, 0.30f));

            CreateCube("Piso porcelanato claro", root.transform, new Vector3(0f, -0.05f, 0f), new Vector3(10.5f, 0.1f, 7.4f), floor);
            CreateCube("Piso balcón", root.transform, new Vector3(0f, -0.03f, -4.3f), new Vector3(4.7f, 0.08f, 1.35f), CreateMaterial(new Color(0.58f, 0.58f, 0.54f)));

            CreateCube("Pared fondo", root.transform, new Vector3(0f, 1.55f, 3.65f), new Vector3(10.5f, 3.1f, 0.12f), wall);
            CreateCube("Pared izquierda", root.transform, new Vector3(-5.25f, 1.55f, 0f), new Vector3(0.12f, 3.1f, 7.4f), wall);
            CreateCube("Pared derecha", root.transform, new Vector3(5.25f, 1.55f, 0f), new Vector3(0.12f, 3.1f, 7.4f), wall);
            CreateCube("Muro dormitorio", root.transform, new Vector3(-1.35f, 1.35f, 1.15f), new Vector3(0.12f, 2.7f, 4.15f), wall);
            CreateCube("Muro baño", root.transform, new Vector3(-3.25f, 1.35f, 2.05f), new Vector3(3.9f, 2.7f, 0.12f), wall);

            CreateCube("Ventanal living", root.transform, new Vector3(1.45f, 1.55f, -3.62f), new Vector3(3.6f, 2.1f, 0.08f), glass);
            CreateCube("Marco ventanal superior", root.transform, new Vector3(1.45f, 2.65f, -3.67f), new Vector3(3.75f, 0.08f, 0.10f), dark);
            CreateCube("Marco ventanal lateral A", root.transform, new Vector3(-0.45f, 1.55f, -3.67f), new Vector3(0.08f, 2.18f, 0.10f), dark);
            CreateCube("Marco ventanal lateral B", root.transform, new Vector3(3.35f, 1.55f, -3.67f), new Vector3(0.08f, 2.18f, 0.10f), dark);
            CreateCube("Ventana dormitorio", root.transform, new Vector3(-3.45f, 1.65f, -3.62f), new Vector3(1.8f, 1.5f, 0.08f), glass);
            CreateCube("Marco ventana dormitorio", root.transform, new Vector3(-3.45f, 2.43f, -3.67f), new Vector3(1.95f, 0.08f, 0.10f), dark);
            CreateCube("Baranda balcón frente", root.transform, new Vector3(0f, 0.95f, -4.95f), new Vector3(4.7f, 1.35f, 0.08f), glass);
            CreateCube("Baranda balcón lateral izquierda", root.transform, new Vector3(-2.35f, 0.95f, -4.3f), new Vector3(0.08f, 1.35f, 1.3f), glass);
            CreateCube("Baranda balcón lateral derecha", root.transform, new Vector3(2.35f, 0.95f, -4.3f), new Vector3(0.08f, 1.35f, 1.3f), glass);

            CreateCube("Sofá living", root.transform, new Vector3(0.4f, 0.45f, -1.15f), new Vector3(2.3f, 0.75f, 0.82f), fabric);
            CreateCube("Respaldo sofá", root.transform, new Vector3(0.4f, 0.95f, -0.78f), new Vector3(2.3f, 0.9f, 0.18f), fabric);
            CreateCube("Mesa baja", root.transform, new Vector3(0.4f, 0.22f, -2.05f), new Vector3(1.25f, 0.22f, 0.72f), wood);
            CreateCube("Alfombra living", root.transform, new Vector3(0.4f, 0.02f, -2.05f), new Vector3(3.1f, 0.04f, 1.9f), CreateMaterial(new Color(0.62f, 0.57f, 0.48f)));
            CreateCube("Cuadro living", root.transform, new Vector3(3.85f, 1.75f, 3.56f), new Vector3(1.15f, 0.78f, 0.06f), champagne);
            CreateCube("Maceta living", root.transform, new Vector3(-4.45f, 0.32f, -2.55f), new Vector3(0.42f, 0.64f, 0.42f), dark);
            CreateCube("Planta living", root.transform, new Vector3(-4.45f, 0.98f, -2.55f), new Vector3(0.72f, 0.85f, 0.72f), plant);
            CreateCube("Luminaria living", root.transform, new Vector3(0.4f, 2.75f, -1.45f), new Vector3(0.85f, 0.10f, 0.85f), champagne);

            CreateCube("Mesada cocina", root.transform, new Vector3(3.05f, 0.48f, 2.35f), new Vector3(3.2f, 0.95f, 0.70f), dark);
            CreateCube("Alacenas cocina", root.transform, new Vector3(3.05f, 2.15f, 2.65f), new Vector3(3.2f, 0.75f, 0.35f), wood);
            CreateCube("Barra desayunadora", root.transform, new Vector3(2.15f, 0.55f, 0.55f), new Vector3(2.2f, 1.1f, 0.45f), wood);
            CreateCube("Detalle barra piedra", root.transform, new Vector3(2.15f, 1.15f, 0.55f), new Vector3(2.35f, 0.10f, 0.55f), white);
            CreateCube("Banqueta 1", root.transform, new Vector3(1.55f, 0.45f, -0.05f), new Vector3(0.38f, 0.9f, 0.38f), dark);
            CreateCube("Banqueta 2", root.transform, new Vector3(2.65f, 0.45f, -0.05f), new Vector3(0.38f, 0.9f, 0.38f), dark);

            CreateCube("Cama dormitorio", root.transform, new Vector3(-3.35f, 0.35f, 0.15f), new Vector3(2.25f, 0.55f, 2.0f), CreateMaterial(new Color(0.56f, 0.58f, 0.62f)));
            CreateCube("Cabecera cama", root.transform, new Vector3(-3.35f, 0.95f, 1.18f), new Vector3(2.25f, 1.05f, 0.16f), fabric);
            CreateCube("Mesa de luz", root.transform, new Vector3(-4.75f, 0.34f, 0.25f), new Vector3(0.45f, 0.68f, 0.45f), wood);
            CreateCube("Placard", root.transform, new Vector3(-2.05f, 1.15f, 2.85f), new Vector3(1.9f, 2.3f, 0.45f), wood);
            CreateCube("Cuadro dormitorio", root.transform, new Vector3(-4.75f, 1.75f, 1.95f), new Vector3(0.08f, 0.75f, 1.05f), champagne);
            CreateCube("Lámpara mesa de luz", root.transform, new Vector3(-4.75f, 0.88f, 0.25f), new Vector3(0.25f, 0.40f, 0.25f), champagne);

            CreateCube("Ducha baño", root.transform, new Vector3(-4.55f, 0.95f, 2.82f), new Vector3(0.9f, 1.9f, 0.9f), glass);
            CreateCube("Vanitory baño", root.transform, new Vector3(-3.35f, 0.45f, 3.12f), new Vector3(0.85f, 0.9f, 0.55f), white);
            CreateCube("Inodoro baño", root.transform, new Vector3(-2.35f, 0.32f, 2.85f), new Vector3(0.55f, 0.65f, 0.55f), white);
            CreateCube("Espejo baño", root.transform, new Vector3(-3.35f, 1.45f, 3.52f), new Vector3(0.95f, 0.80f, 0.05f), glass);

            return root;
        }

        private static void CreateHotspots(GameObject environmentRoot, InfoPanelController panel, bool usingSeaViewEnvironment)
        {
            Transform hotspotParent = new GameObject("ARDE Hotspots").transform;

            if (usingSeaViewEnvironment)
            {
                CreateSeaViewHotspots(environmentRoot.transform, hotspotParent, panel);
                return;
            }

            CreateHotspot(hotspotParent, panel, "Hotspot Living comedor", new Vector3(0.4f, 1.35f, -1.85f), new HotspotData("Living comedor", "Espacio principal con iluminación natural, salida al balcón y distribución integrada.", new[] { "Superficie: 18 m²", "Piso: porcelanato", "Orientación: norte" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Cocina integrada", new Vector3(3.0f, 1.55f, 1.45f), new HotspotData("Cocina integrada", "Cocina moderna con muebles bajo mesada, alacena y espacio para lavarropas.", new[] { "Mesada: granito", "Muebles: melamina premium", "Conexión: eléctrica/gas según unidad" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Dormitorio principal", new Vector3(-3.45f, 1.35f, 0.10f), new HotspotData("Dormitorio principal", "Ambiente cómodo con placard empotrado y ventana al contrafrente.", new[] { "Superficie: 12 m²", "Placard: incluido", "Ventilación natural" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Baño completo", new Vector3(-3.35f, 1.38f, 2.85f), new HotspotData("Baño completo", "Baño moderno con revestimientos de primera línea y grifería cromada.", new[] { "Ducha", "Vanitory", "Extractor / ventilación" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Balcón", new Vector3(0f, 1.25f, -4.45f), new HotspotData("Balcón", "Balcón al frente con vista abierta, ideal para expansión del living.", new[] { "Superficie: 4 m²", "Baranda vidriada", "Orientación norte" }));
        }

        private static void CreateSeaViewHotspots(Transform environmentRoot, Transform hotspotParent, InfoPanelController panel)
        {
            Bounds living = GetChildBoundsOrFallback(environmentRoot, "Living Area");
            Bounds bedrooms = GetChildBoundsOrFallback(environmentRoot, "Bedrooms");
            Bounds bathrooms = GetChildBoundsOrFallback(environmentRoot, "Bathrooms");

            CreateHotspot(hotspotParent, panel, "Hotspot Living comedor", HotspotPoint(living, new Vector3(0f, 0f, -0.15f)), new HotspotData("Living comedor", "Área social principal del entorno Sea View, ideal para presentar amplitud, iluminación y relación con los ventanales.", new[] { "Ambiente: Living Area", "Recorrido guiado", "Vista comercial" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Cocina integrada", HotspotPoint(living, new Vector3(0.42f, 0f, 0.28f)), new HotspotData("Cocina integrada", "Sector de cocina/barra integrado al espacio social, preparado para mostrar terminaciones y equipamiento.", new[] { "Zona: Living Area", "Barra / apoyo", "Integración social" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Dormitorio principal", HotspotPoint(bedrooms, Vector3.zero), new HotspotData("Dormitorio principal", "Ambiente privado del asset Sea View, pensado para destacar confort, guardado y luz natural.", new[] { "Grupo: Bedrooms", "Uso privado", "Escala real" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Baño completo", HotspotPoint(bathrooms, Vector3.zero), new HotspotData("Baño completo", "Sector de baños del modelo real, útil para mostrar revestimientos, artefactos y terminaciones.", new[] { "Grupo: Bathrooms", "Terminaciones", "Equipamiento" }));
            CreateHotspot(hotspotParent, panel, "Hotspot Balcón", HotspotPoint(living, new Vector3(0f, 0f, -0.48f)), new HotspotData("Balcón / vista", "Punto cercano a ventanales y vista exterior para presentar expansión visual y conexión con el entorno.", new[] { "Ventanales", "Vista exterior", "Expansión" }));
        }

        private static Vector3 HotspotPoint(Bounds bounds, Vector3 normalizedOffset)
        {
            Vector3 center = bounds.center;
            center.x += bounds.extents.x * normalizedOffset.x;
            center.z += bounds.extents.z * normalizedOffset.z;
            center.y = bounds.min.y + Mathf.Max(1.35f, bounds.size.y * 0.45f);
            return center;
        }

        private static void CreateHotspot(Transform parent, InfoPanelController panel, string name, Vector3 position, HotspotData data)
        {
            Material hotspotMaterial = CreateMaterial(new Color(0.95f, 0.68f, 0.18f));
            GameObject hotspotObject = new GameObject(name);
            hotspotObject.transform.SetParent(parent, false);
            hotspotObject.transform.position = position;

            SphereCollider collider = hotspotObject.AddComponent<SphereCollider>();
            collider.radius = 0.42f;
            collider.isTrigger = true;

            GameObject visualRoot = new GameObject("Hotspot Visual");
            visualRoot.transform.SetParent(hotspotObject.transform, false);

            GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            halo.name = "Halo";
            halo.transform.SetParent(visualRoot.transform, false);
            halo.transform.localPosition = new Vector3(0f, -0.22f, 0f);
            halo.transform.localScale = new Vector3(0.34f, 0.012f, 0.34f);
            halo.GetComponent<Renderer>().sharedMaterial = hotspotMaterial;
            UnityEngine.Object.DestroyImmediate(halo.GetComponent<Collider>());

            GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stem.name = "Stem";
            stem.transform.SetParent(visualRoot.transform, false);
            stem.transform.localPosition = new Vector3(0f, -0.02f, 0f);
            stem.transform.localScale = new Vector3(0.055f, 0.22f, 0.055f);
            stem.GetComponent<Renderer>().sharedMaterial = hotspotMaterial;
            UnityEngine.Object.DestroyImmediate(stem.GetComponent<Collider>());

            GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dot.name = "Dot";
            dot.transform.SetParent(visualRoot.transform, false);
            dot.transform.localPosition = new Vector3(0f, 0.24f, 0f);
            dot.transform.localScale = Vector3.one * 0.22f;
            dot.GetComponent<Renderer>().sharedMaterial = hotspotMaterial;
            UnityEngine.Object.DestroyImmediate(dot.GetComponent<Collider>());

            Hotspot hotspot = hotspotObject.AddComponent<Hotspot>();
            hotspot.Configure(data, panel, visualRoot.transform);
        }

        private static CameraPoint[] CreateCameraPoints(GameObject environmentRoot, bool usingSeaViewEnvironment)
        {
            if (!usingSeaViewEnvironment)
            {
                return new[]
                {
                    new CameraPoint("General", CreatePoint("CameraPoint General", new Vector3(0f, 4.6f, -8.8f), Quaternion.Euler(34f, 0f, 0f))),
                    new CameraPoint("Living", CreatePoint("CameraPoint Living", new Vector3(0.2f, 2.25f, -5.15f), Quaternion.Euler(15f, 0f, 0f))),
                    new CameraPoint("Cocina", CreatePoint("CameraPoint Cocina", new Vector3(4.35f, 1.95f, -0.75f), Quaternion.Euler(15f, -42f, 0f))),
                    new CameraPoint("Dormitorio", CreatePoint("CameraPoint Dormitorio", new Vector3(-4.35f, 1.85f, -2.55f), Quaternion.Euler(14f, 25f, 0f))),
                    new CameraPoint("Baño", CreatePoint("CameraPoint Baño", new Vector3(-4.85f, 1.75f, 1.75f), Quaternion.Euler(13f, 55f, 0f))),
                    new CameraPoint("Balcón", CreatePoint("CameraPoint Balcón", new Vector3(0f, 1.95f, -6.35f), Quaternion.Euler(10f, 0f, 0f)))
                };
            }

            return CreateSeaViewCameraPoints(environmentRoot.transform);
        }

        private static CameraPoint[] CreateSeaViewCameraPoints(Transform environmentRoot)
        {
            Bounds living = GetChildBoundsOrFallback(environmentRoot, "Living Area");
            Bounds hallways = GetChildBoundsOrFallback(environmentRoot, "Hallways");
            Bounds bedrooms = GetChildBoundsOrFallback(environmentRoot, "Bedrooms");
            Bounds bathrooms = GetChildBoundsOrFallback(environmentRoot, "Bathrooms");

            Bounds general = living;
            general.Encapsulate(hallways);

            return new[]
            {
                CreateLookCameraPoint("General", general, new Vector3(0f, 0.30f, -1.25f), 1.35f),
                CreateLookCameraPoint("Living", living, new Vector3(0f, 0.18f, -0.95f), 1.20f),
                CreateLookCameraPoint("Cocina", living, new Vector3(0.58f, 0.16f, -0.18f), 1.10f),
                CreateLookCameraPoint("Dormitorio", bedrooms, new Vector3(0f, 0.16f, -0.95f), 1.10f),
                CreateLookCameraPoint("Baño", bathrooms, new Vector3(0f, 0.14f, -1.10f), 1.00f),
                CreateLookCameraPoint("Balcón", living, new Vector3(0f, 0.15f, -1.35f), 1.10f)
            };
        }

        private static CameraPoint CreateLookCameraPoint(string label, Bounds bounds, Vector3 normalizedOffset, float targetHeight)
        {
            Vector3 target = bounds.center + Vector3.up * targetHeight;
            Vector3 position = bounds.center;
            position.x += bounds.extents.x * normalizedOffset.x;
            position.y = bounds.min.y + Mathf.Max(1.45f, bounds.size.y * normalizedOffset.y);
            position.z += bounds.extents.z * normalizedOffset.z;

            float minDistance = Mathf.Max(2.2f, Mathf.Max(bounds.extents.x, bounds.extents.z) * 0.35f);
            if ((position - target).magnitude < minDistance)
            {
                position += new Vector3(0f, 0f, -minDistance);
            }

            Quaternion rotation = Quaternion.LookRotation(target - position, Vector3.up);
            return new CameraPoint(label, CreatePoint($"CameraPoint {label}", position, rotation));
        }

        private static Bounds GetChildBoundsOrFallback(Transform environmentRoot, string childName)
        {
            Transform child = environmentRoot.Find(childName);
            if (child != null && TryGetRendererBounds(child, out Bounds childBounds))
            {
                return childBounds;
            }

            if (TryGetRendererBounds(environmentRoot, out Bounds rootBounds))
            {
                return rootBounds;
            }

            return new Bounds(Vector3.zero, Vector3.one * 5f);
        }

        private static bool TryGetRendererBounds(Transform root, out Bounds bounds)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            bounds = new Bounds(root.position, Vector3.one);
            bool hasBounds = false;

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return hasBounds;
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

        private static GameObject CreatePanel(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject panel = CreatePanel(name, parent, color);
            Anchor(panel.GetComponent<RectTransform>(), anchorMin, anchorMax);
            return panel;
        }

        private static Text CreateText(string value, Transform parent, int size, FontStyle style, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            Anchor(textObject.GetComponent<RectTransform>(), anchorMin, anchorMax);
            return text;
        }

        private static Button CreateButton(string label, Transform parent, Color color, int fontSize, Color? textColor = null)
        {
            GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.GetComponent<Image>().color = color;
            Button button = buttonObject.GetComponent<Button>();
            CreateText(label, buttonObject.transform, fontSize, FontStyle.Bold, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, textColor ?? Color.white);
            return button;
        }

        private static void CreateQRPlaceholder(Transform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject qr = CreatePanel("QR Placeholder", parent, Color.white);
            Anchor(qr.GetComponent<RectTransform>(), anchorMin, anchorMax);
            CreatePanel("QR Border", qr.transform, new Color(0.08f, 0.07f, 0.06f), new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.08f));
            CreatePanel("QR Block A", qr.transform, new Color(0.08f, 0.07f, 0.06f), new Vector2(0.12f, 0.66f), new Vector2(0.32f, 0.86f));
            CreatePanel("QR Block B", qr.transform, new Color(0.08f, 0.07f, 0.06f), new Vector2(0.68f, 0.66f), new Vector2(0.88f, 0.86f));
            CreatePanel("QR Block C", qr.transform, new Color(0.08f, 0.07f, 0.06f), new Vector2(0.12f, 0.14f), new Vector2(0.32f, 0.34f));
            CreatePanel("QR Pattern", qr.transform, new Color(0.08f, 0.07f, 0.06f), new Vector2(0.42f, 0.32f), new Vector2(0.58f, 0.48f));
            CreateText("QR", qr.transform, 42, FontStyle.Bold, TextAnchor.MiddleCenter, new Vector2(0.36f, 0.48f), new Vector2(0.64f, 0.62f), new Color(0.08f, 0.07f, 0.06f));
        }

        private static GameObject CreateCube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Standard") ?? Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
            if (shader == null)
            {
                throw new InvalidOperationException("No se encontró un shader compatible para generar materiales ARDE.");
            }

            Material material = new Material(shader);
            ApplyMaterialColor(material, color);
            return material;
        }

        private static void ApplyMaterialColor(Material material, Color color)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            material.color = color;
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
