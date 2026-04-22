using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;
using System.Linq;
using System;

namespace FavTool.FavoriteAssets
{
    public class FavoriteAssets : EditorWindow
    {
        #region Private Fields

        // UI State
        private Vector2 _assetsScrollPosition;
        private Vector2 _detailsScrollPosition;
        private float _containerItemHeight = 32f;
        private float _assetsScrollHeight = 300f;
        private bool _isResizingScroll = false;
        private Rect _resizeHandleRect;
        private const float RESIZE_HANDLE_HEIGHT = 10f;
        private bool _showNewProfileInput = false;
        private string _newProfileName = "";

        // Assets Management
        private List<UnityEngine.Object> _favoriteAssets = new List<UnityEngine.Object>();
        private ReorderableList _reorderableAssetsList;
        private Dictionary<int, AssetEditorState> _assetEditorStates = new Dictionary<int, AssetEditorState>();
        private static Stack<EditorLayoutState> _layoutStateStack = new Stack<EditorLayoutState>();

        // Profile Management
        private string _profilesDirectory;
        private List<string> _availableProfiles = new List<string>();
        private string _currentProfileName = "Default";
        private int _selectedProfileIndex = 0;

        #endregion

        #region Menu Items and Initialization

        [MenuItem("Tools/Favorite Assets")]
        public static void ShowWindow()
        {
            GetWindow<FavoriteAssets>("My Favorite Assets");
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("My Favorite Assets");
            InitializeProfilesDirectory();
            LoadCurrentProfileName();
            LoadAvailableProfiles();
            UpdateSelectedProfileIndex();
            LoadAssets();
            InitializeReorderableList();
        }

        private void InitializeProfilesDirectory()
        {
            _profilesDirectory = Path.Combine(Application.dataPath, "../Library/MyFavoriteAssetProfiles");

            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }
        }

        private void OnDisable()
        {
            SaveAssets();
            SaveCurrentProfileName();
            DestroyAllEditors();
        }

        #endregion

        #region GUI Drawing

        private void OnGUI()
        {
            EditorGUILayout.Space();

            DrawToolbar();
            DrawProfilesGUI();
            DrawHelpBox();
            ProcessAssetDragAndDrop();
            DrawAssetsPanel();
            DrawResizeHandle();
            DrawDivider();
            DrawDetailsPanel();

            HandleEvents();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("SceneLoadIn").image, "Open a new Favorite Assets panel"),
                EditorStyles.toolbarButton, GUILayout.Width(28)))
            {
                CreateInstance<FavoriteAssets>().Show();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawHelpBox()
        {
            EditorGUILayout.HelpBox(
                "You can drag and drop any Unity asset to this window. Grab containers from their left side and drag to reorder them.",
                MessageType.Info);
        }

        private void DrawAssetsPanel()
        {
            EditorGUILayout.LabelField("My Favorite Assets", EditorStyles.boldLabel);

            float scrollPadding = 6f;

            Rect scrollViewRect = EditorGUILayout.GetControlRect(false, _assetsScrollHeight - scrollPadding,
                GUILayout.ExpandWidth(true));

            _assetsScrollPosition = GUI.BeginScrollView(scrollViewRect, _assetsScrollPosition,
                new Rect(0, 0, scrollViewRect.width - 16,
                    Mathf.Max(_containerItemHeight * _favoriteAssets.Count + 10f,
                        _assetsScrollHeight - scrollPadding)));

            if (_reorderableAssetsList != null && _favoriteAssets.Count > 0)
            {
                _reorderableAssetsList.DoList(new Rect(0, 0, scrollViewRect.width - 16,
                    Mathf.Max(_containerItemHeight * _favoriteAssets.Count + 10f,
                        _assetsScrollHeight - scrollPadding)));
            }
            else
            {
                EditorGUI.HelpBox(new Rect(0, 0, scrollViewRect.width - 16, 40),
                    "Drag and drop assets here to pin them.", MessageType.Info);
            }

            GUI.EndScrollView();

            EditorGUILayout.Space(scrollPadding);
        }

        private void DrawResizeHandle()
        {
            Rect scrollViewRect = GUILayoutUtility.GetLastRect();
            float scrollPadding = 6f;

            _resizeHandleRect = new Rect(scrollViewRect.x, scrollViewRect.y + scrollViewRect.height + scrollPadding,
                scrollViewRect.width, RESIZE_HANDLE_HEIGHT);

            Color handleColor = GetResizeHandleColor();
            EditorGUI.DrawRect(_resizeHandleRect, handleColor);
            DrawResizeHandleGripLines();

            if (_resizeHandleRect.Contains(Event.current.mousePosition) || _isResizingScroll)
            {
                EditorGUIUtility.AddCursorRect(_resizeHandleRect, MouseCursor.ResizeVertical);
            }
        }

        private void DrawResizeHandleGripLines()
        {
            Color gripColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            float centerY = _resizeHandleRect.y + _resizeHandleRect.height / 2;

            for (int i = 0; i < 5; i++)
            {
                float x = _resizeHandleRect.x + _resizeHandleRect.width / 2 - 12 + i * 6;
                EditorGUI.DrawRect(new Rect(x, centerY, 4, 1), gripColor);
            }
        }

        private Color GetResizeHandleColor()
        {
            Color baseColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);

            if (_isResizingScroll || _resizeHandleRect.Contains(Event.current.mousePosition))
                return new Color(0.4f, 0.7f, 1.0f, 1.0f);

            return baseColor;
        }

        private void DrawDivider()
        {
            EditorGUILayout.Space(RESIZE_HANDLE_HEIGHT + 2);
            Rect dividerRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(dividerRect, new Color(0.1f, 0.1f, 0.1f));
            EditorGUILayout.Space(2);
        }

        private void DrawDetailsPanel()
        {
            bool hasAnyExpandedEditor = _assetEditorStates.Any(pair => pair.Value.IsExpanded);
            if (hasAnyExpandedEditor)
            {
                EditorGUILayout.LabelField("Asset Details", EditorStyles.boldLabel);

                _detailsScrollPosition = EditorGUILayout.BeginScrollView(_detailsScrollPosition, GUILayout.ExpandHeight(true));

                DrawExpandedAssetDetails();

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawExpandedAssetDetails()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            SaveLayoutSettings();

            foreach (var pair in _assetEditorStates.Where(p => p.Value.IsExpanded))
            {
                EditorGUILayout.Space(5);

                UnityEngine.Object targetAsset = pair.Value.Target;
                if (targetAsset != null)
                    DrawAssetDetailsFoldout(pair.Key, pair.Value, targetAsset);

                DrawInlineEditor(pair.Value);
                EditorGUILayout.Space(5);
            }

            RestoreLayout();
            EditorGUILayout.EndVertical();
        }

        private void DrawAssetDetailsFoldout(int assetId, AssetEditorState editorState, UnityEngine.Object asset)
        {
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            foldoutStyle.fontSize = 12;

            EditorGUILayout.BeginHorizontal();

            bool newFoldedOut = EditorGUILayout.Foldout(editorState.IsFoldedOut, asset.name, true, foldoutStyle);

            if (newFoldedOut != editorState.IsFoldedOut)
            {
                editorState.IsFoldedOut = newFoldedOut;
                SaveAssets();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();

            Rect headerDivider = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(headerDivider, new Color(0.3f, 0.3f, 0.3f));
        }

        private void DrawInlineEditor(AssetEditorState state)
        {
            if (state == null || state.EditorInstance == null) return;

            try
            {
                if (state.IsFoldedOut)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    if (state.ShouldDrawHeader)
                    {
                        EditorGUILayout.BeginFadeGroup(0.9999f);
                        GUILayout.Space(5.0f);
                        state.EditorInstance.DrawHeader();
                        GUILayout.Space(5f);
                        EditorGUILayout.EndFadeGroup();
                    }

                    if (state.ShouldDrawGUI)
                    {
                        EditorGUILayout.BeginVertical();
                        bool inspectorExpanded = InternalEditorUtility.GetIsInspectorExpanded(state.EditorInstance.target);
                        if (!state.ShouldDrawHeader)
                            InternalEditorUtility.SetIsInspectorExpanded(state.EditorInstance.target, true);

                        state.EditorInstance.OnInspectorGUI();

                        if (!state.ShouldDrawHeader)
                            InternalEditorUtility.SetIsInspectorExpanded(state.EditorInstance.target, inspectorExpanded);

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
            }
            catch (Exception ex)
            {
                if (ex is ExitGUIException) throw;
                Debug.LogException(ex);
            }
        }

        private void DrawProfilesGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Current Profile:", GUILayout.Width(100));

            EditorGUI.BeginChangeCheck();
            _selectedProfileIndex =
                EditorGUILayout.Popup(_selectedProfileIndex, _availableProfiles.ToArray(), GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
            {
                HandleProfileChange();
            }

            if (GUILayout.Button("Create Profile", GUILayout.Width(100)))
            {
                _showNewProfileInput = !_showNewProfileInput;
            }

            if (_availableProfiles.Count > 1 && GUILayout.Button("Delete Profile", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Delete Profile",
                        "Are you sure you want to delete the profile '" + _currentProfileName + "'?",
                        "Yes", "No"))
                {
                    DeleteCurrentProfile();
                }
            }

            EditorGUILayout.EndHorizontal();

            if (_showNewProfileInput)
            {
                DrawNewProfileInput();
            }
        }

        private void DrawNewProfileInput()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("New Profile Name:", GUILayout.Width(120));
            _newProfileName = EditorGUILayout.TextField(_newProfileName, GUILayout.Width(150));

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_newProfileName) ||
                                         _availableProfiles.Contains(_newProfileName));
            if (GUILayout.Button("Create", GUILayout.Width(80)))
            {
                CreateNewProfile(_newProfileName);
                _showNewProfileInput = false;
                _newProfileName = "";
            }

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                _showNewProfileInput = false;
                _newProfileName = "";
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Event Handling

        private void HandleEvents()
        {
            Event currentEvent = Event.current;

            HandleResizing(currentEvent);

            if (currentEvent.type == EventType.MouseDrag ||
                currentEvent.type == EventType.DragUpdated ||
                currentEvent.type == EventType.DragPerform)
            {
                Repaint();
            }

            if (currentEvent.type == EventType.MouseUp)
            {
                SaveAssets();
            }
        }

        private void HandleResizing(Event e)
        {
            float scrollPadding = 6f;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (_resizeHandleRect.Contains(e.mousePosition))
                    {
                        _isResizingScroll = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (_isResizingScroll)
                    {
                        float minHeight = 100f;
                        float maxHeight = position.height - 200f;
                        _assetsScrollHeight = Mathf.Clamp(
                            e.mousePosition.y - _resizeHandleRect.y + RESIZE_HANDLE_HEIGHT + scrollPadding + _assetsScrollHeight,
                            minHeight, maxHeight);
                        Repaint();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (_isResizingScroll)
                    {
                        _isResizingScroll = false;
                        SaveAssets();
                        e.Use();
                    }
                    break;
            }
        }

        private void ProcessAssetDragAndDrop()
        {
            Event evt = Event.current;
            Rect dropArea = new Rect(0, 0, position.width, position.height);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    if (!dropArea.Contains(evt.mousePosition)) return;
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        evt.Use();
                    }
                    break;

                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition)) return;
                    DragAndDrop.AcceptDrag();
                    HandleAssetDrop();
                    evt.Use();
                    break;
            }
        }

        private void HandleAssetDrop()
        {
            int duplicateCount = 0;

            foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject != null)
                {
                    string draggedAssetPath = AssetDatabase.GetAssetPath(draggedObject);

                    bool alreadyExists = _favoriteAssets.Any(existingAsset =>
                        existingAsset != null &&
                        AssetDatabase.GetAssetPath(existingAsset) == draggedAssetPath);

                    if (alreadyExists)
                        duplicateCount++;
                    else
                        _favoriteAssets.Add(draggedObject);
                }
            }

            if (duplicateCount > 0)
            {
                string message = duplicateCount == 1
                    ? "One asset was not added because it already exists in the profile."
                    : $"{duplicateCount} assets were not added because they already exist in the profile.";
                EditorUtility.DisplayDialog("Duplicate Asset", message, "OK");
            }

            InitializeReorderableList();
            SaveAssets();
        }

        #endregion

        #region Asset List Management

        private void InitializeReorderableList()
        {
            _reorderableAssetsList = new ReorderableList(_favoriteAssets, typeof(UnityEngine.Object), true, false, false, false);
            _reorderableAssetsList.drawElementCallback = DrawAssetListItem;
            _reorderableAssetsList.elementHeightCallback = (int index) => _containerItemHeight;
            _reorderableAssetsList.headerHeight = 0;
            _reorderableAssetsList.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) => { };
        }

        private void DrawAssetListItem(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index >= _favoriteAssets.Count || _favoriteAssets[index] == null)
                return;

            Rect adjustedRect = new Rect(rect.x, rect.y + 1f, rect.width, rect.height - 2f);
            DrawAssetItemBackground(adjustedRect, index, isActive, isFocused);

            Rect fieldRect = new Rect(
                adjustedRect.x + 8,
                adjustedRect.y + (adjustedRect.height - EditorGUIUtility.singleLineHeight) / 2,
                adjustedRect.width / 3,
                EditorGUIUtility.singleLineHeight
            );

            EditorGUI.BeginChangeCheck();
            UnityEngine.Object newObject = EditorGUI.ObjectField(
                fieldRect, GUIContent.none, _favoriteAssets[index], typeof(UnityEngine.Object), false);

            if (EditorGUI.EndChangeCheck())
                HandleAssetReplacement(index, newObject);

            DrawAssetItemButtons(adjustedRect, index);
        }

        private void DrawAssetItemBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            FavToolSettings settings = FavToolSettings.Instance;
            Color bgColor = index % 2 == 0 ? settings.EvenRowColor : settings.OddRowColor;
            EditorGUI.DrawRect(rect, bgColor);

            if (isActive || isFocused)
            {
                Color borderColor = new Color(0.32f, 0.62f, 0.95f, 1.0f);

                Rect borderRect = new Rect(rect.x, rect.y, rect.width, 1);
                EditorGUI.DrawRect(borderRect, borderColor);
                borderRect.y = rect.y + rect.height - 1;
                EditorGUI.DrawRect(borderRect, borderColor);
                borderRect = new Rect(rect.x, rect.y, 1, rect.height);
                EditorGUI.DrawRect(borderRect, borderColor);
                borderRect.x = rect.x + rect.width - 1;
                EditorGUI.DrawRect(borderRect, borderColor);
            }
        }

        private void DrawAssetItemButtons(Rect adjustedRect, int index)
        {
            UnityEngine.Object asset = _favoriteAssets[index];
            if (asset == null) return;

            float buttonSize = EditorGUIUtility.singleLineHeight * 1.2f;
            float startX = adjustedRect.x + adjustedRect.width / 3 + 20;
            float buttonY = adjustedRect.y + (adjustedRect.height - buttonSize) / 2;

            Rect openButtonRect = new Rect(startX, buttonY, 64, buttonSize);
            Rect inspectButtonRect = new Rect(startX + 70, buttonY, 64, buttonSize);
            Rect deleteButtonRect = new Rect(startX + 140, buttonY, 24, buttonSize);
            Rect moreInfoButtonRect = new Rect(startX + 170, buttonY, 24, buttonSize);

            if (GUI.Button(openButtonRect, new GUIContent(" Open", "Click to open the asset")))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && openButtonRect.Contains(e.mousePosition))
                    OpenAsset(asset);
            }

            if (GUI.Button(inspectButtonRect, new GUIContent("Inspect", "Click to inspect the asset in Inspector")))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && inspectButtonRect.Contains(e.mousePosition))
                    Selection.activeObject = asset;
            }

            if (GUI.Button(deleteButtonRect, new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus").image, "Click to remove this asset")))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && deleteButtonRect.Contains(e.mousePosition))
                {
                    RemoveAsset(index);
                    e.Use();
                }
            }

            DrawMoreInfoButton(moreInfoButtonRect, asset);
        }

        private void DrawMoreInfoButton(Rect buttonRect, UnityEngine.Object asset)
        {
            if (asset == null) return;

            GUIContent buttonContent = new GUIContent();
            int instanceID = asset.GetInstanceID();
            bool hasInlineEditor = _assetEditorStates.ContainsKey(instanceID);

            string iconName = hasInlineEditor && _assetEditorStates[instanceID].IsExpanded
                ? "d_animationvisibilitytoggleoff@2x"
                : "d_animationvisibilitytoggleon@2x";

            buttonContent.image = EditorGUIUtility.IconContent(iconName).image;
            buttonContent.tooltip = "Click for more info";

            if (GUI.Button(buttonRect, buttonContent))
            {
                Event e = Event.current;
                if (e.type == EventType.Used && buttonRect.Contains(e.mousePosition))
                    ToggleAssetDetails(asset, instanceID, hasInlineEditor);
            }
        }

        private void ToggleAssetDetails(UnityEngine.Object asset, int instanceID, bool hasInlineEditor)
        {
            if (!hasInlineEditor)
            {
                AssetEditorState state = new AssetEditorState
                {
                    IsExpanded = true,
                    Target = asset,
                    IsFoldedOut = true
                };
                _assetEditorStates[instanceID] = state;
                UpdateEditors(state);
            }
            else
            {
                _assetEditorStates[instanceID].IsExpanded = !_assetEditorStates[instanceID].IsExpanded;
                if (!_assetEditorStates[instanceID].IsExpanded)
                    DestroyEditor(_assetEditorStates[instanceID]);
                else if (_assetEditorStates[instanceID].EditorInstance == null)
                    UpdateEditors(_assetEditorStates[instanceID]);
            }

            Repaint();
            SaveAssets();
        }

        private void HandleAssetReplacement(int index, UnityEngine.Object newObject)
        {
            if (_favoriteAssets[index] != null)
            {
                int oldInstanceID = _favoriteAssets[index].GetInstanceID();
                if (_assetEditorStates.ContainsKey(oldInstanceID))
                {
                    DestroyEditor(_assetEditorStates[oldInstanceID]);
                    _assetEditorStates.Remove(oldInstanceID);
                }
            }

            _favoriteAssets[index] = newObject;
            SaveAssets();
        }

        private void RemoveAsset(int index)
        {
            UnityEngine.Object asset = _favoriteAssets[index];

            if (asset != null)
            {
                int instanceID = asset.GetInstanceID();
                if (_assetEditorStates.ContainsKey(instanceID))
                {
                    DestroyEditor(_assetEditorStates[instanceID]);
                    _assetEditorStates.Remove(instanceID);
                }
            }

            _favoriteAssets.RemoveAt(index);
            SaveAssets();
        }

        private void OpenAsset(UnityEngine.Object asset)
        {
            if (asset == null) return;

            if (asset is ScriptableObject || asset is Texture || asset is Texture2D ||
                asset is Sprite || asset is Material)
                Selection.activeObject = asset;
            else
                AssetDatabase.OpenAsset(asset);
        }

        #endregion

        #region Editor Management

        private void UpdateEditors(AssetEditorState state)
        {
            if (state == null || state.Target == null) return;

            bool isGameObject = state.Target is GameObject;
            state.ShouldDrawHeader = true;
            state.ShouldDrawGUI = !isGameObject;
            state.ShouldDrawPreview = true;
            state.EditorInstance = Editor.CreateEditor(state.Target);

            Component targetAsComponent = state.Target as Component;
            state.PreviewEditorInstance = targetAsComponent != null
                ? Editor.CreateEditor(targetAsComponent.gameObject)
                : state.EditorInstance;
        }

        private void DestroyAllEditors()
        {
            foreach (var state in _assetEditorStates.Values)
                DestroyEditor(state);
            _assetEditorStates.Clear();
        }

        private void DestroyEditor(AssetEditorState state)
        {
            if (state.EditorInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(state.EditorInstance);
                state.EditorInstance = null;
            }

            if (state.PreviewEditorInstance != null && state.PreviewEditorInstance != state.EditorInstance)
            {
                UnityEngine.Object.DestroyImmediate(state.PreviewEditorInstance);
                state.PreviewEditorInstance = null;
            }
        }

        #endregion

        #region Profile Management

        private void HandleProfileChange()
        {
            SaveAssets();
            _currentProfileName = _availableProfiles[_selectedProfileIndex];
            SaveCurrentProfileName();
            DestroyAllEditors();
            LoadAssets();
            InitializeReorderableList();
        }

        private void LoadAvailableProfiles()
        {
            _availableProfiles.Clear();

            if (Directory.Exists(_profilesDirectory))
            {
                string[] profileFiles = Directory.GetFiles(_profilesDirectory, "*.json");
                foreach (string profilePath in profileFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(profilePath);
                    if (fileName != "CurrentProfile")
                        _availableProfiles.Add(fileName);
                }
            }

            if (_availableProfiles.Count == 0)
                _availableProfiles.Add("Default");
        }

        private void UpdateSelectedProfileIndex()
        {
            _selectedProfileIndex = _availableProfiles.IndexOf(_currentProfileName);
            if (_selectedProfileIndex < 0)
            {
                _selectedProfileIndex = 0;
                _currentProfileName = _availableProfiles[0];
            }
        }

        private void LoadCurrentProfileName()
        {
            string currentProfilePath = Path.Combine(_profilesDirectory, "CurrentProfile.json");

            if (File.Exists(currentProfilePath))
            {
                string jsonData = File.ReadAllText(currentProfilePath);
                ProfileNameData profileNameData = JsonUtility.FromJson<ProfileNameData>(jsonData);

                if (profileNameData != null && !string.IsNullOrEmpty(profileNameData.CurrentProfileName))
                    _currentProfileName = profileNameData.CurrentProfileName;
            }
            else
            {
                _currentProfileName = "Default";
            }
        }

        private void SaveCurrentProfileName()
        {
            string currentProfilePath = Path.Combine(_profilesDirectory, "CurrentProfile.json");
            ProfileNameData profileNameData = new ProfileNameData { CurrentProfileName = _currentProfileName };
            File.WriteAllText(currentProfilePath, JsonUtility.ToJson(profileNameData, true));
        }

        private void CreateNewProfile(string profileName)
        {
            if (string.IsNullOrWhiteSpace(profileName) || _availableProfiles.Contains(profileName))
                return;

            SaveAssets();
            _currentProfileName = profileName;
            _favoriteAssets.Clear();
            _assetEditorStates.Clear();
            SaveAssets();

            _availableProfiles.Add(profileName);
            UpdateSelectedProfileIndex();
            SaveCurrentProfileName();
        }

        private void DeleteCurrentProfile()
        {
            if (_availableProfiles.Count <= 1) return;

            string profilePath = Path.Combine(_profilesDirectory, _currentProfileName + ".json");
            if (File.Exists(profilePath))
                File.Delete(profilePath);

            _availableProfiles.Remove(_currentProfileName);
            _currentProfileName = _availableProfiles[0];
            UpdateSelectedProfileIndex();

            LoadAssets();
            InitializeReorderableList();
            SaveCurrentProfileName();
        }

        #endregion

        #region Data Serialization

        private void SaveAssets()
        {
            AssetProfileData saveData = new AssetProfileData
            {
                AssetPaths = new List<string>(),
                ExpandedAssetPaths = new List<string>(),
                FoldedOutStates = new List<bool>(),
                AssetsScrollHeight = _assetsScrollHeight
            };

            foreach (UnityEngine.Object asset in _favoriteAssets)
            {
                if (asset == null) continue;
                string assetPath = AssetDatabase.GetAssetPath(asset);
                if (string.IsNullOrEmpty(assetPath)) continue;

                saveData.AssetPaths.Add(assetPath);

                int instanceID = asset.GetInstanceID();
                bool isExpanded = _assetEditorStates.ContainsKey(instanceID) &&
                                  _assetEditorStates[instanceID].IsExpanded;
                if (isExpanded)
                {
                    saveData.ExpandedAssetPaths.Add(assetPath);
                    saveData.FoldedOutStates.Add(_assetEditorStates[instanceID].IsFoldedOut);
                }
            }

            string profilePath = Path.Combine(_profilesDirectory, _currentProfileName + ".json");
            File.WriteAllText(profilePath, JsonUtility.ToJson(saveData, true));
        }

        private void LoadAssets()
        {
            _favoriteAssets.Clear();
            _assetEditorStates.Clear();

            string profilePath = Path.Combine(_profilesDirectory, _currentProfileName + ".json");
            if (!File.Exists(profilePath)) return;

            AssetProfileData loadedData = JsonUtility.FromJson<AssetProfileData>(File.ReadAllText(profilePath));
            if (loadedData == null) return;

            if (loadedData.AssetsScrollHeight > 0)
                _assetsScrollHeight = loadedData.AssetsScrollHeight;

            if (loadedData.AssetPaths != null)
            {
                foreach (string assetPath in loadedData.AssetPaths)
                {
                    UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    if (loadedAsset != null)
                        _favoriteAssets.Add(loadedAsset);
                }

                if (loadedData.ExpandedAssetPaths != null)
                    RestoreAssetEditorStates(loadedData);
            }
        }

        private void RestoreAssetEditorStates(AssetProfileData loadedData)
        {
            for (int i = 0; i < loadedData.ExpandedAssetPaths.Count; i++)
            {
                UnityEngine.Object expandedAsset =
                    AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(loadedData.ExpandedAssetPaths[i]);
                if (expandedAsset == null) continue;

                AssetEditorState state = new AssetEditorState
                {
                    IsExpanded = true,
                    Target = expandedAsset,
                    IsFoldedOut = (loadedData.FoldedOutStates != null && i < loadedData.FoldedOutStates.Count)
                        ? loadedData.FoldedOutStates[i]
                        : true
                };

                _assetEditorStates[expandedAsset.GetInstanceID()] = state;
                UpdateEditors(state);
            }
        }

        #endregion

        #region Layout Helper Methods

        private static void SaveLayoutSettings()
        {
            _layoutStateStack.Push(new EditorLayoutState()
            {
                Skin = GUI.skin,
                Color = GUI.color,
                ContentColor = GUI.contentColor,
                BackgroundColor = GUI.backgroundColor,
                Enabled = GUI.enabled,
                IndentLevel = EditorGUI.indentLevel,
                FieldWidth = EditorGUIUtility.fieldWidth,
                LabelWidth = EditorGUIUtility.labelWidth,
                HierarchyMode = EditorGUIUtility.hierarchyMode,
                WideMode = EditorGUIUtility.wideMode
            });
        }

        private static void RestoreLayout()
        {
            EditorLayoutState layoutSettings = _layoutStateStack.Pop();
            GUI.skin = layoutSettings.Skin;
            GUI.color = layoutSettings.Color;
            GUI.contentColor = layoutSettings.ContentColor;
            GUI.backgroundColor = layoutSettings.BackgroundColor;
            GUI.enabled = layoutSettings.Enabled;
            EditorGUI.indentLevel = layoutSettings.IndentLevel;
            EditorGUIUtility.fieldWidth = layoutSettings.FieldWidth;
            EditorGUIUtility.labelWidth = layoutSettings.LabelWidth;
            EditorGUIUtility.hierarchyMode = layoutSettings.HierarchyMode;
            EditorGUIUtility.wideMode = layoutSettings.WideMode;
        }

        #endregion
    }
}
